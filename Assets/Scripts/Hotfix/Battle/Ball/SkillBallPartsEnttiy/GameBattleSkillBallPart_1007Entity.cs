using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameBattleSkillBallPart_1007EntityData : GameBattleSkillBallPartEntityData<GameBattleSkillBallPart_1007Entity, GameBattleSkillBallPart_1007StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBallPart_1007Entity;

    public override void GameBallOnCollision(in int ballEntityId, in GamePhysicsHit hit)
    {
        var starData = GetBallPartStarData();
        var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(ballEntityId, EnSceneEntityComType.Attribute);
        attCom.AddAddtionalPercentage(EnEntityAttributeId.MoveSpeed, starData.speedPercentage01);

        GameEffectMgr.Instance.PlayEffectOnce(EnGameEffectId.Eff_ball_part_1007_1, WorldPos);
    }
}
public class GameBattleSkillBallPart_1007Entity : GameBattleSkillBallPartEntity
{

}
