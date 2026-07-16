using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBall_1001EntityData : GameBattleSkillBallEntityData<GameBattleSkillBallEntity, GameBattleSkillBall_1001StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_BattleSkillBall_1001;


    protected override void OnCollision(in GamePhysicsHit hit)
    {
        var starData = GetStarUserData();
        BuffMgr.Instance.AddEntityBuff(_OwnerEntityId, hit.entityId, starData.buffData);

        var ownerAtk = Mathf.CeilToInt(GetOwnerEntityAttackDamage() * starData.atkPercentage);
        var atkValue = ownerAtk + starData.damageValue;
        GameAttackMgr.Instance.Attack(_OwnerEntityId, hit.entityId, atkValue, hit.point, starData.damageType);
    }
}
public class GameBattleSkillBall_1001Entity : GameBattleSkillBallEntity
{

}