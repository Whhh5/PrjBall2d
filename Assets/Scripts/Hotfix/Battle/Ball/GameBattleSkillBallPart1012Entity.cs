using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;



public class GameBattleSkillBallPart1012EntityData : GameBattleSkillBallPartEntityData<GameBattleSkillBallPart1012Entity, GameBattleSkillBallPart1012StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBallPart1012Entity;
    private float _SliderValue;

    private int _EnergyCount;
    private CancellationTokenSource _Token;
    private readonly HashSet<int> _EffectEntityIds = new();
    public override void OnPoolDestroy()
    {
        _Token.Cancel();
        base.OnPoolDestroy();
        _SliderValue
            = _EnergyCount
            = 0;
        _Token = null;
        _EffectEntityIds.Clear();
    }
    protected override void OnPoolInit(GameBattleSkillBallPartEntityUserData userData)
    {
        base.OnPoolInit(userData);
        _Token = new();
    }
    public override void OnGOCreate(GameEntity entity)
    {
        base.OnGOCreate(entity);
        _GameSkillBallPartEntity.SetSliderValue(_SliderValue);
    }

    public override void GameBallOnCollision(in int ballEntityId, in GamePhysicsHit hit)
    {
        var starData = GetBallPartStarData();
        _EnergyCount += starData.collisionEnergy;

        if (_EnergyCount >= starData.energyMaxCount)
        {
            var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(_OwnerEntityId, EnSceneEntityComType.Attribute);
            var value = attCom.GetValue(starData.testAttId);
            var addValue = Mathf.CeilToInt(value * starData.restorePercentage01);

            GameEntityUtility.AddAttributeValue(_OwnerEntityId, _OwnerEntityId, starData.addAttId, addValue);

            GameEffectMgr.Instance.PlayEffectOnce(EnGameEffectId.Eff_ball_part_1012_1, WorldPos);
            PlayRestoreAnim(WorldPos, _OwnerEntityId);
            _EnergyCount %= starData.energyMaxCount;
        }

        var slider = Mathf.Clamp01(_EnergyCount / (float)starData.energyMaxCount);
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
        AudioMgr.Instance.Play(EnAudioId.BallPart1012_Effect1);

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
            AudioMgr.Instance.Play(EnAudioId.AddHp);
        }
        _EffectEntityIds.Remove(effectEntityId);
        GameEffectMgr.Instance.DestroyEffect(effectEntityId);
    }
}
public class GameBattleSkillBallPart1012Entity : GameBattleSkillBallPartEntity
{
    [SerializeField]
    private SpriteMask2DSlider _Slider;

    public void SetSliderValue(float value)
    {
        _Slider.SetSliderValue(value);
    }
}
