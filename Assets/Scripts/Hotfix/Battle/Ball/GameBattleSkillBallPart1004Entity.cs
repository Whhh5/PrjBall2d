using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameBattleSkillBallPart1004EntityData : GameBattleSkillBallPartEntityData<GameBattleSkillBallPart1004Entity, GameBattleSkillBallPart1004StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBallPart1004Entity;

    public override void GameBallOnCollision(in int ballEntityId, in GamePhysicsHit hit)
    {
        var starData = GetBallPartStarData();
        var testValue = Random.Range(0, 100);
        if (testValue > starData.killProbablity0100)
            return;
        var entityProCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(in hit.entityId, EnSceneEntityComType.Attribute);
        var maxHp = entityProCom.GetValue(EnEntityAttributeId.LimitHp);
        var atkDamage = Mathf.CeilToInt(maxHp * starData.attPercentage01);
        GameAttackMgr.Instance.AttackMonster(in _OwnerEntityId, in hit.entityId, in atkDamage, in hit.point, in starData.atkDamageType);


        var pointInfoCom = EntityMgr.Instance.GetEntityData<IEntityGridPointInfo>(in hit.entityId);
        var centerPos = pointInfoCom.GetCenterPointWorldPos();
        GameEffectMgr.Instance.PlayEffectOnce(EnGameEffectId.Eff_ball_part_1004_1, WorldPos);

        GameEffectMgr.Instance.PlayEffectOnce(EnGameEffectId.Eff_ball_part_1004_2, centerPos);
        AudioMgr.Instance.Play(EnAudioId.BallPart1004_Effect1);
    }
}
public class GameBattleSkillBallPart1004Entity : GameBattleSkillBallPartEntity
{

}
