


using System.Collections.Generic;
using UnityEngine;


public class GameBattleSkillBall_1003EntityData : GameBattleSkillBallEntityData<GameBattleSkillBall_1003Entity, GameBattleSkillBall_1003StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_BattleSkillBall_1003;


    protected override void OnCollision(in GamePhysicsHit hit)
    {
        var starData = GetStarUserData();

        BuffMgr.Instance.AddEntityBuff(_OwnerEntityId, hit.entityId, starData.buffData);

        var ownerAtk = Mathf.CeilToInt(GetOwnerEntityAttackDamage() * starData.atkPercentage);
        var atkValue = ownerAtk + starData.damageValue;
        GameAttackMgr.Instance.Attack(_OwnerEntityId, hit.entityId, atkValue, hit.point, starData.damageType);

    }

}
public class GameBattleSkillBall_1003Entity : GameBattleSkillBallEntity
{

}