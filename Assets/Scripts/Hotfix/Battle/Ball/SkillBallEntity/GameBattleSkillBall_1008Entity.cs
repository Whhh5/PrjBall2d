


using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBall_1008EntityData : GameBattleSkillBallEntityData<GameBattleSkillBall_1008Entity, GameBattleSkillBall_1008StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_BattleSkillBall_1008;


    protected override void OnCollision(in GamePhysicsHit hit)
    {
        var starData = GetStarUserData();
        var ownerAtk = Mathf.CeilToInt(GetOwnerEntityAttackDamage() * starData.atkPercentage);
        var atkValue = ownerAtk + starData.damageValue;
        GameAttackMgr.Instance.Attack(_OwnerEntityId, hit.entityId, atkValue, hit.point, starData.damageType);
    }

}
public class GameBattleSkillBall_1008Entity : GameBattleSkillBallEntity
{

}