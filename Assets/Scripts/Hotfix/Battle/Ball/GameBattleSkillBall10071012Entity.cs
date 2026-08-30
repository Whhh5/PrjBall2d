using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBall10071012EntityData : GameBattleSkillBallEntityData<GameBattleSkillBall10071012Entity, GameBattleSkillBall10071012StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBall10071012Entity;

    protected override void OnCollision(in GamePhysicsHit hit)
    {
        var starData = GetStarUserData();

        var ballAtkValue = GameAttributeUtility.GetValueInt(_EntityID, EnEntityAttributeId.Attack);
        var ownerAtk = Mathf.CeilToInt(GetOwnerEntityAttributeDamage(starData.attId) * starData.attPercentage01);
        var atkValue = ballAtkValue + ownerAtk + starData.damageValue;
        var damageResult = GameAttackMgr.Instance.AttackMonster(_OwnerEntityId, hit.entityId, atkValue, hit.point, starData.damageType);
        PlayHitAudio(in damageResult);

        var sys = GameBattleMgr.Instance.GetSystem<GameBattleLeechSystem>();
        var star = GameBattleBallMgr.Instance.GetBallStar(_SkillBallId);
        var damageValue = starData.leechDamageValue + Mathf.CeilToInt(damageResult.value * starData.leechAttPercentage01);
        for (int i = 0; i < starData.createCount; i++)
        {
            sys.CreateLeechEntity(_SkillBallId, star, in _OwnerEntityId, in hit.entityId, in hit.point, in damageValue);
        }

    }
}
public class GameBattleSkillBall10071012Entity : GameBattleSkillBallEntity
{

}
