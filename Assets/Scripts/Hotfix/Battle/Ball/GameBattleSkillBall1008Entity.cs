


using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBall1008EntityData : GameBattleSkillBallEntityData<GameBattleSkillBall1008Entity, GameBattleSkillBall1008StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBall1008Entity;


    protected override void OnCollision(in GamePhysicsHit hit)
    {
        var starData = GetStarUserData();
        var ownerAtk = Mathf.CeilToInt(GetOwnerEntityAttributeDamage(starData.attId) * starData.attPercentage01);
        var atkValue = ownerAtk + starData.damageValue;
        var damageResult = GameAttackMgr.Instance.AttackMonster(_OwnerEntityId, hit.entityId, atkValue, hit.point, starData.damageType);
        PlayHitAudio(in damageResult);
    }

}
public class GameBattleSkillBall1008Entity : GameBattleSkillBallEntity
{

}