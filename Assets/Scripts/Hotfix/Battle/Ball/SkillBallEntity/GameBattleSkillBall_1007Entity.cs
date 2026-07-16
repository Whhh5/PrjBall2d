


using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBall_1007EntityData : GameBattleSkillBallEntityData<GameBattleSkillBall_1007Entity, GameBattleSkillBall_1007StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_BattleSkillBall_1007;


    private int _LastEntityId;
    private EnDirection _LastEntityDirection;
    public override void Destroy()
    {
        base.Destroy();
        _LastEntityId = -1;
        _LastEntityDirection = EnDirection.None;
    }
    public override bool IsCollision()
    {
        //return base.IsCollision();
        return false;
    }

    public override void Collision(in GamePhysicsHit hit)
    {
        if (_LastEntityId == hit.entityId)
            if (_LastEntityDirection == hit.dir)
                return;

        _LastEntityId = hit.entityId;
        _LastEntityDirection = hit.dir;
        base.Collision(hit);
    }
    protected override void OnCollision(in GamePhysicsHit hit)
    {
        var starData = GetStarUserData();
        var ownerAtk = Mathf.CeilToInt(GetOwnerEntityAttackDamage() * starData.atkPercentage);
        var atkValue = ownerAtk + starData.damageValue;
        GameAttackMgr.Instance.Attack(_OwnerEntityId, hit.entityId, atkValue, hit.point, starData.damageType);
    }

}
public class GameBattleSkillBall_1007Entity : GameBattleSkillBallEntity
{

}