

using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBall1EntityData : GameBattleSkillBallEntityData<GameBattleSkillBall1Entity, GameBattleSkillBall1StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBall1Entity;

    protected override void OnCollision(in GamePhysicsHit hit)
    {
        var starData = GetStarUserData();
        var ownerAtk = Mathf.CeilToInt(GetOwnerEntityAttributeDamage(starData.attId) * starData.atkPercentage);
        var atkValue = ownerAtk + starData.damageValue;
        var damageResult = GameAttackMgr.Instance.AttackMonster(_OwnerEntityId, hit.entityId, atkValue, hit.point, starData.damageType);

        PlayHitAudio(in damageResult);
    }

}
public class GameBattleSkillBall1Entity : GameBattleSkillBallEntity
{

}