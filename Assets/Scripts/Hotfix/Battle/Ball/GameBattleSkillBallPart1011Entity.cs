using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameBattleSkillBallPart1011EntityData : GameBattleSkillBallPartEntityData<GameBattleSkillBallPart1011Entity, GameBattleSkillBallPart1011StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBallPart1011Entity;

    protected override void OnPoolInit(GameBattleSkillBallPartEntityUserData userData)
    {
        base.OnPoolInit(userData);
        EventMgr.Instance.Register<GameBattleBallLightningEventData>(EnABBEvent.EVENT_BATTLE_BALL_LEIGHTNING, 0, 0, OnEvent_BallLeightning);
    }
    public override void OnPoolDestroy()
    {
        EventMgr.Instance.Unregister<GameBattleBallLightningEventData>(EnABBEvent.EVENT_BATTLE_BALL_LEIGHTNING, 0, 0, OnEvent_BallLeightning);
        base.OnPoolDestroy();
    }
    public override void GameBallOnCollision(in int entityId, in GamePhysicsHit hit)
    {
        GameEffectMgr.Instance.PlayEffectOnce(EnGameEffectId.Eff_ball_part_1011_1, WorldPos);
    }

    private void OnEvent_BallLeightning(in int _, in int __, in GameBattleBallLightningEventData data)
    {
        var starData = GetBallPartStarData();
        BuffMgr.Instance.AddEntityBuff(_OwnerEntityId, data.targetEntityId, starData.buffData);
        AudioMgr.Instance.Play(EnAudioId.BallPart1011_Effect1);
    }
}
public class GameBattleSkillBallPart1011Entity : GameBattleSkillBallPartEntity
{

}
