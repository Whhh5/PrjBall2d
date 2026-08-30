

using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBall1006EntityData : GameBattleSkillBallEntityData<GameBattleSkillBall1006Entity, GameBattleSkillBall1006StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBall1006Entity;

    protected override void OnCollision(in GamePhysicsHit hit)
    {
        var starData = GetStarUserData();

        BuffMgr.Instance.AddEntityBuff(_OwnerEntityId, hit.entityId, starData.buffUserData);


        var ballAtkValue = GameAttributeUtility.GetValueInt(_EntityID, EnEntityAttributeId.Attack);
        var ownerAtk = Mathf.CeilToInt(GetOwnerEntityAttributeDamage(starData.attId) * starData.attPercentage01);
        var atkValue = ballAtkValue + ownerAtk + starData.damageValue;
        var damageResult = GameAttackMgr.Instance.AttackMonster(_OwnerEntityId, hit.entityId, atkValue, hit.point, starData.damageType);

        PlayHitAudio(in damageResult);
    }

}
public class GameBattleSkillBall1006Entity : GameBattleSkillBallEntity
{

}