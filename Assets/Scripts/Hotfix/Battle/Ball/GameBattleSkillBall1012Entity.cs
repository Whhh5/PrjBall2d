

using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class GameBattleSkillBall1012EntityData : GameBattleSkillBallEntityData<GameBattleSkillBall1012Entity, GameBattleSkillBall1012StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBall1012Entity;

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

        var ballAtkValue = GameAttributeUtility.GetValueInt(_EntityID, EnEntityAttributeId.Attack);
        var ownerAtk = Mathf.CeilToInt(GetOwnerEntityAttributeDamage(starData.attId) * starData.attPercentage01);
        var atkValue = ballAtkValue + ownerAtk + starData.damageValue;
        var damageResult = GameAttackMgr.Instance.AttackMonster(_OwnerEntityId, hit.entityId, atkValue, hit.point, starData.damageType);
        PlayHitAudio(in damageResult);

        var testValue = Random.Range(0, 100);
        if (testValue < starData.effectInvokProbability0100)
        {
            var addHp = Mathf.CeilToInt(damageResult.value * starData.effectRestorePercentage01);
            GameEntityUtility.AddAttributeValue(_OwnerEntityId, _OwnerEntityId, starData.effectAddAttId, addHp);

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
public class GameBattleSkillBall1012Entity : GameBattleSkillBallEntity
{

}