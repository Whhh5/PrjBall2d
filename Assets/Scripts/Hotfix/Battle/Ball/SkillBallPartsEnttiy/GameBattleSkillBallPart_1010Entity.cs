using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameBattleSkillBallPart_1010EntityData : GameBattleSkillBallPartEntityData<GameBattleSkillBallPart_1010Entity, GameBattleSkillBallPart_1010StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBallPart_1010Entity;

    public override void GameBallOnCollision(in int entityId, in GamePhysicsHit hit)
    {
    }
}
public class GameBattleSkillBallPart_1010Entity : GameBattleSkillBallPartEntity
{

}
