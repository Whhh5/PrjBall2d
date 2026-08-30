


using System.Collections.Generic;
using UnityEngine;


public class GameBattleSkillBall1003EntityData : GameBattleSkillBallEntityData<GameBattleSkillBall1003Entity, GameBattleSkillBall1003StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBall1003Entity;


    protected override void OnCollision(in GamePhysicsHit hit)
    {
        var starData = GetStarUserData();

        BuffMgr.Instance.AddEntityBuff(_OwnerEntityId, hit.entityId, starData.buffData);


        var ballAtkValue = GameAttributeUtility.GetValueInt(_EntityID, EnEntityAttributeId.Attack);
        var ownerAtk = Mathf.CeilToInt(GetOwnerEntityAttributeDamage(starData.attId) * starData.attPercentage01);
        var atkValue = ballAtkValue + ownerAtk + starData.damageValue;
        var damageResult = GameAttackMgr.Instance.AttackMonster(_OwnerEntityId, hit.entityId, atkValue, hit.point, starData.damageType);

        PlayHitAudio(in damageResult);
    }

}
public class GameBattleSkillBall1003Entity : GameBattleSkillBallEntity
{

}