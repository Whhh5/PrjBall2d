

using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class GameBattleSkillBall_1012EntityData : GameBattleSkillBallEntityData<GameBattleSkillBall_1012Entity, GameBattleSkillBall_1012StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_BattleSkillBall_1012;

    private CancellationTokenSource _Token;
    private readonly HashSet<int> _EffectEntityIds = new();
    public override void OnPoolDestroy()
    {
        _Token.Cancel();
        base.OnPoolDestroy();
        _Token = null;
        _EffectEntityIds.Clear();
    }
    protected override void OnPoolInit(GameBattleSkillBallEntityUserData userData)
    {
        base.OnPoolInit(userData);
        _Token = new();
    }
    protected override void OnCollision(in GamePhysicsHit hit)
    {
        var starData = GetStarUserData();
        var ownerAtk = Mathf.CeilToInt(GetOwnerEntityAttackDamage() * starData.atkPercentage);
        var atkValue = ownerAtk + starData.damageValue;
        GameAttackMgr.Instance.Attack(_OwnerEntityId, hit.entityId, atkValue, hit.point, starData.damageType);

        var testValue = Random.Range(0f, 1f);
        if (testValue <= starData.effectInvokePercentage)
        {
            var pointInfo = EntityMgr.Instance.GetEntityData<IEntityGridPointInfo>(_OwnerEntityId);
            var targetPos = pointInfo.GetCenterPointWorldPos();
            var proCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(_OwnerEntityId, EnSceneEntityComType.Attribute);
            var curMaxHp = proCom.GetValue(EnEntityAttributeId.LimitHp);
            var addHp = curMaxHp * starData.effectRestoreHealthPercentage;
            GameAttackMgr.Instance.RestoreHealth(_OwnerEntityId, _OwnerEntityId, addHp, targetPos);

            PlayRestoreAnim(hit.point, _OwnerEntityId);
        }
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
public class GameBattleSkillBall_1012Entity : GameBattleSkillBallEntity
{

}