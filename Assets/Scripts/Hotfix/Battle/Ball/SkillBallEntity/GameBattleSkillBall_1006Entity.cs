

using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBall_1006EntityData : GameBattleSkillBallEntityData<GameBattleSkillBall_1006Entity, GameBattleSkillBall_1006StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_BattleSkillBall_1006;

    protected override void OnCollision(in GamePhysicsHit hit)
    {
        var starData = GetStarUserData();

        BuffMgr.Instance.AddEntityBuff(_OwnerEntityId, hit.entityId, starData.buffUserData);

        var ownerAtk = Mathf.CeilToInt(GetOwnerEntityAttackDamage() * starData.atkPercentage);
        var atkValue = ownerAtk + starData.damageValue;
        GameAttackMgr.Instance.Attack(_OwnerEntityId, hit.entityId, atkValue, hit.point, starData.damageType);
    }

}
public class GameBattleSkillBall_1006Entity : GameBattleSkillBallEntity
{

}