using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameBattleSkillBallPart_1005EntityData : GameBattleSkillBallPartEntityData<GameBattleSkillBallPart_1005Entity, GameBattleSkillBallPart_1005StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBallPart_1005Entity;

    protected override void OnPoolInit(GameBattleSkillBallPartEntityUserData userData)
    {
        base.OnPoolInit(userData);
        EventMgr.Instance.Register<BattleBallEarthquakeEventData>(EnABBEvent.EVENT_BATTLE_BALL_EARTHQUAKE, 0, 0, OnEvent_BallEarthquake);
    }
    public override void OnPoolDestroy()
    {
        EventMgr.Instance.Register<BattleBallEarthquakeEventData>(EnABBEvent.EVENT_BATTLE_BALL_EARTHQUAKE, 0, 0, OnEvent_BallEarthquake);
        base.OnPoolDestroy();
    }

    public override void GameBallOnCollision(in int entityId, in GamePhysicsHit hit)
    {
    }

    private void OnEvent_BallEarthquake(in int _, in int __, in BattleBallEarthquakeEventData data)
    {
        var starData = GetBallPartStarData();
        var entityIds = data.targetEntityIds;
        foreach (var entityId in entityIds)
        {
            BuffMgr.Instance.AddEntityBuff(_OwnerEntityId, entityId, starData.buffData);
        }

        GameEffectMgr.Instance.PlayEffectOnce(EnGameEffectId.Eff_ball_part_1005_1, WorldPos);
    }
}
public class GameBattleSkillBallPart_1005Entity : GameBattleSkillBallPartEntity
{

}
