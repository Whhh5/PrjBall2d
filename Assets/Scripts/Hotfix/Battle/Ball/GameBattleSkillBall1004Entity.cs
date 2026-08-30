

using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBall1004EntityData : GameBattleSkillBallEntityData<GameBattleSkillBall1004Entity, GameBattleSkillBall1004StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBall1004Entity;

    protected override void OnCollision(in GamePhysicsHit hit)
    {
        var starData = GetStarUserData();

        var ballAtkValue = GameAttributeUtility.GetValueInt(_EntityID, EnEntityAttributeId.Attack);
        var ownerAtk = Mathf.CeilToInt(GetOwnerEntityAttributeDamage(starData.attId) * starData.attPercentage01);
        var atkValue = ballAtkValue + ownerAtk + starData.damageValue;
        var damageResult = GameAttackMgr.Instance.AttackMonster(_OwnerEntityId, hit.entityId, atkValue, hit.point, starData.damageType);
        PlayHitAudio(in damageResult);

        GameBattleBallMgr.Instance.DestroyBallEntity(_EntityID);

    }

}
public class GameBattleSkillBall1004Entity : GameBattleSkillBallEntity
{

}