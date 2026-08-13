using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameBattleSkillBallPart1007EntityData : GameBattleSkillBallPartEntityData<GameBattleSkillBallPart1007Entity, GameBattleSkillBallPart1007StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBallPart1007Entity;

    public override void GameBallOnCollision(in int ballEntityId, in GamePhysicsHit hit)
    {
        var starData = GetBallPartStarData();
        GameAttributeUtility.AddAddtionalPercentage(ballEntityId, starData.attId, starData.speedPercentage01);

        GameEffectMgr.Instance.PlayEffectOnce(EnGameEffectId.Eff_ball_part_1007_1, WorldPos);
        AudioMgr.Instance.Play(EnAudioId.BallPart1007_Effect1);
    }
}
public class GameBattleSkillBallPart1007Entity : GameBattleSkillBallPartEntity
{

}
