using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;



public class GameBattleSkillBallPart1006EntityData : GameBattleSkillBallPartEntityData<GameBattleSkillBallPart1006Entity, GameBattleSkillBallPart1006StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBallPart1006Entity;

    protected override void OnPoolInit(GameBattleSkillBallPartEntityUserData userData)
    {
        base.OnPoolInit(userData);
        var starData = GetBallPartStarData();
        EventMgr.Instance.Register<EntityBuffAddEventBuff>(EnABBEvent.EVENT_ENTITY_BUFF_Add, (int)starData.testBuffId, 0, OnEvent_EntityBuffAdd);
    }
    public override void OnPoolDestroy()
    {
        var starData = GetBallPartStarData();
        EventMgr.Instance.Unregister<EntityBuffAddEventBuff>(EnABBEvent.EVENT_ENTITY_BUFF_Add, (int)starData.testBuffId, 0, OnEvent_EntityBuffAdd);
        base.OnPoolDestroy();
    }

    public override void GameBallOnCollision(in int entityId, in GamePhysicsHit hit)
    {
    }

    private void OnEvent_EntityBuffAdd(in int nBuffId, in int entityId, in EntityBuffAddEventBuff data)
    {
        var starData = GetBallPartStarData();
        if (data.count < starData.testBuffCount)
            return;
        var testValue = Random.Range(0, 100);
        if (testValue > starData.testProbability0100)
            return;

        BuffMgr.Instance.AddEntityBuff(_OwnerEntityId, entityId, starData.buffData);

        GameEffectMgr.Instance.PlayEffectOnce(EnGameEffectId.Eff_ball_part_1006_1, WorldPos);
        AudioMgr.Instance.Play(EnAudioId.BallPart1006_Effect1);
    }
}
public class GameBattleSkillBallPart1006Entity : GameBattleSkillBallPartEntity
{

}
