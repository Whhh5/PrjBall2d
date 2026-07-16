using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;



public class GameBattleSkillBallPart_1012EntityData : GameBattleSkillBallPartEntityData<GameBattleSkillBallPart_1012Entity, GameBattleSkillBallPart_1012StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBallPart_1012Entity;
    private float _SliderValue;

    private int _CollisionCount;
    private CancellationTokenSource _Token;
    private readonly HashSet<int> _EffectEntityIds = new();
    public override void OnPoolDestroy()
    {
        _Token.Cancel();
        base.OnPoolDestroy();
        _SliderValue
            = _CollisionCount
            = 0;
        _Token = null;
        _EffectEntityIds.Clear();
    }
    protected override void OnPoolInit(GameBattleSkillBallPartEntityUserData userData)
    {
        base.OnPoolInit(userData);
        _Token = new();
    }
    public override void OnGOCreate()
    {
        base.OnGOCreate();
        _GameSkillBallPartEntity.SetSliderValue(_SliderValue);
    }

    public override void GameBallOnCollision(in int ballEntityId, in GamePhysicsHit hit)
    {
        _CollisionCount++;
        var starData = GetBallPartStarData();

        if (_CollisionCount >= starData.collisionCount)
        {
            var pointInfo = EntityMgr.Instance.GetEntityData<IEntityGridPointInfo>(_OwnerEntityId);
            var targetPos = pointInfo.GetCenterPointWorldPos();
            var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(_OwnerEntityId, EnSceneEntityComType.Attribute);
            var value = attCom.GetValue(EnEntityAttributeId.LimitHp);
            var addValue = Mathf.CeilToInt(value * starData.restoreHealthPercentage01);
            GameAttackMgr.Instance.RestoreHealth(_OwnerEntityId, _OwnerEntityId, addValue, targetPos);

            GameEffectMgr.Instance.PlayEffectOnce(EnGameEffectId.Eff_ball_part_1012_1, WorldPos);
            PlayRestoreAnim(WorldPos, _OwnerEntityId);
            _CollisionCount %= starData.collisionCount;
        }

        var slider = Mathf.Clamp01(_CollisionCount / (float)starData.collisionCount);
        SetSliderValue(slider);
    }

    public void SetSliderValue(float value)
    {
        _SliderValue = value;
        if (IsLoadSuccess)
            _GameSkillBallPartEntity.SetSliderValue(value);
    }

    private async void PlayRestoreAnim(Vector2 startPos, int entityId)
    {
        var pointInfo = EntityMgr.Instance.GetEntityData<IEntityGridPointInfo>(entityId);
        var targetPos = pointInfo.GetCenterPointWorldPos();
        var offsetDis = Random.Range(-1f, 1f);
        var effectEntityId = GameEffectMgr.Instance.PlayEffect(EnGameEffectId.Eff_Hit_Xixue_trail, startPos);
        _EffectEntityIds.Add(effectEntityId);

        var isCancel = await DOTween.To(() => 0f, slider =>
        {
            if (EntityMgr.Instance.IsValid(entityId))
                targetPos = pointInfo.GetCenterPointWorldPos();

            var toPos = Vector2.Lerp(startPos, targetPos, slider);
            var dir = (targetPos - startPos).normalized;
            var offset = Mathf.Sin(Mathf.Lerp(0, Mathf.PI, slider)) * offsetDis * new Vector2(-dir.y, dir.x);

            EntityMgr.Instance.SetEntityWorldPos(effectEntityId, toPos + offset);

        }, 1f, 0.5f)
            .ToUniTask(cancellationToken: _Token.Token).SuppressCancellationThrow();
        if (!isCancel)
        {
            GameEffectMgr.Instance.PlayEffectOnce(EnGameEffectId.Eff_Hero_Hpadd, targetPos);
        }
        _EffectEntityIds.Remove(effectEntityId);
        GameEffectMgr.Instance.DestroyEffect(effectEntityId);
    }
}
public class GameBattleSkillBallPart_1012Entity : GameBattleSkillBallPartEntity
{
    [SerializeField]
    private SpriteMask2DSlider _Slider;

    public void SetSliderValue(float value)
    {
        _Slider.SetSliderValue(value);
    }
}
