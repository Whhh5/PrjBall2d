


using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBall1007EntityData : GameBattleSkillBallEntityData<GameBattleSkillBall1007Entity, GameBattleSkillBall1007StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBall1007Entity;


    private int _LastEntityId;
    private EnDirection _LastEntityDirection;
    public override void Destroy()
    {
        base.Destroy();
        _LastEntityId = -1;
        _LastEntityDirection = EnDirection.None;
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
        var ownerAtk = Mathf.CeilToInt(GetOwnerEntityAttributeDamage(starData.attId) * starData.attPercentage01);
        var atkValue = ownerAtk + starData.damageValue;
        var damageResult = GameAttackMgr.Instance.AttackMonster(_OwnerEntityId, hit.entityId, atkValue, hit.point, starData.damageType);
        PlayHitAudio(in damageResult);
    }

}
public class GameBattleSkillBall1007Entity : GameBattleSkillBallEntity
{

}