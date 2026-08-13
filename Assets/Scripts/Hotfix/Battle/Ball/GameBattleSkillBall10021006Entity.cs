using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class GameBattleSkillBall10021006EntityData : GameBattleSkillBallEntityData<GameBattleSkillBall10021006Entity, GameBattleSkillBall10021006StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBall10021006Entity;

    protected override void OnCollision(in GamePhysicsHit hit)
    {
        var starData = GetStarUserData();
        var ownerAtk = Mathf.CeilToInt(GetOwnerEntityAttributeDamage(starData.attId) * starData.attPercentage01);
        var atkValue = ownerAtk + starData.damageValue;
        var damageResult = GameAttackMgr.Instance.AttackMonster(_OwnerEntityId, hit.entityId, atkValue, hit.point, starData.damageType);
        PlayHitAudio(in damageResult);

        var damageValue = starData.lciclcDamageValue + Mathf.CeilToInt(damageResult.value * starData.lciclcAttPercentage01);
        var star = GameBattleBallMgr.Instance.GetBallStar(_SkillBallId);
        var fireIndex = ABBUtil.GetTempKey();
        var sys = GameBattleMgr.Instance.GetSystem<GameBattleLciclcSystem>();
        for (int i = 0; i < starData.lciclcCount; i++)
        {
            sys.CreateLciclcCreate(fireIndex, damageValue, _SkillBallId, star, _OwnerEntityId, starData.radius, in hit.point);
        }
    }
}
public class GameBattleSkillBall10021006Entity : GameBattleSkillBallEntity
{

}
