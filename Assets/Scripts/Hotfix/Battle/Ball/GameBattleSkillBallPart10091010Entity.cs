using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class GameBattleSkillBallPart10091010EntityData : GameBattleSkillBallPartEntityData<GameBattleSkillBallPart10091010Entity, GameBattleSkillBallPart10091010StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBallPart10091010Entity;

    private GamePhysicsHit[] _Hits = new GamePhysicsHit[30];

    public override void OnPoolDestroy()
    {
        EventMgr.Instance.Unregister<GameBattleBall10091010AttackEventData>(EnABBEvent.EVENT_BATTLE_BALLL10091010_ATTACK, 0, 0, OnEvent_Attack);

        base.OnPoolDestroy();

    }
    protected override void OnPoolInit(GameBattleSkillBallPartEntityUserData userData)
    {
        base.OnPoolInit(userData);

        EventMgr.Instance.Register<GameBattleBall10091010AttackEventData>(EnABBEvent.EVENT_BATTLE_BALLL10091010_ATTACK, 0, 0, OnEvent_Attack);
    }

    private void OnEvent_Attack(in int _, in int __, in GameBattleBall10091010AttackEventData data)
    {
        var starData = GetBallPartStarData();


        //AudioMgr.Instance.Play(EnAudioId.Laser);

        var ballPoint = EntityMgr.Instance.GetEntityWorldPos2D(data.ballEntityId);

        ref readonly var gridLbPos = ref GameMapMgr.Instance.GetGridLbPosition();
        ref readonly var gridRuPos = ref GameMapMgr.Instance.GetGridRuPosition();

        for (int j = 0; j < starData.laserCount; j++)
        {
            var angle = Random.Range(0, Mathf.PI);
            var point1 = default(Vector2);
            var point2 = default(Vector2);
            if (!GamePhysicsUtility.TryLineCaseRect(in gridLbPos, in gridRuPos, in ballPoint, in angle, ref point1, ref point2))
                continue;

            var effectEntityId = SceneEntityMgr.Instance.CreateEntityData<GameEffectLaserEntityData>();
            var effectEntityData = EntityMgr.Instance.GetEntityData<GameEffectLaserEntityData>(effectEntityId);
            effectEntityData.SetPoint2(point1, point2);
            effectEntityData.SetLaserWidth(data.width);
            EntityMgr.Instance.SetEntityWorldPos(effectEntityId, ballPoint);
            EntityMgr.Instance.LoadEntity(effectEntityId);

            var count = GamePhysicsUtility.TryLineCast(in point1, in point2, data.width / 2, ref _Hits);
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    ref readonly var atkHit = ref _Hits[i];
                    GameAttackMgr.Instance.AttackMonster(_OwnerEntityId, atkHit.entityId, data.damageValue, atkHit.point, in data.damageType);
                }
            }
        }

    }
    
    public override void GameBallOnCollision(in int ballEntityId, in GamePhysicsHit hit)
    {

    }
}
public class GameBattleSkillBallPart10091010Entity : GameBattleSkillBallPartEntity
{

}
