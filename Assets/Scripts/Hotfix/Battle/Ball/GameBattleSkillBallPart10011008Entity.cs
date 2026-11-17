using UnityEngine;

public class GameBattleSkillBallPart10011008EntityData : GameBattleSkillBallPartEntityData<GameBattleSkillBallPart10011008Entity, GameBattleSkillBallPart10011008StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBallPart10011008Entity;

    protected override void OnPoolInit(GameBattleSkillBallPartEntityUserData userData)
    {
        base.OnPoolInit(userData);
        EventMgr.Instance.Register<GameBattleBall10011008AttackEventData>(EnABBEvent.EVENT_BATTLE_BALLL10011008_ATTACK, 0, 0, OnEvent_Attack);
    }
    public override void OnPoolDestroy()
    {
        EventMgr.Instance.Unregister<GameBattleBall10011008AttackEventData>(EnABBEvent.EVENT_BATTLE_BALLL10011008_ATTACK, 0, 0, OnEvent_Attack);

        base.OnPoolDestroy();

    }
    public override void GameBallOnCollision(in int ballEntityId, in GamePhysicsHit hit)
    {
    }

    private void OnEvent_Attack(in int _, in int __, in GameBattleBall10011008AttackEventData data)
    {

        ref readonly var lbPos = ref GameMapMgr.Instance.GetGridLbPosition();
        ref readonly var ruPos = ref GameMapMgr.Instance.GetGridRuPosition();

        var starData = GetBallPartStarData();
        Vector2 point1 = Vector2.zero, point2 = Vector2.zero;
        var swordSystem = GameBattleMgr.Instance.GetSystem<GameBattleSwordSystem>();
        var star = GameBattleBallMgr.Instance.GetBallStar(_SourceBallId);
        for (int i = 0; i < starData.extraSwordCount; i++)
        {
            var lineAngle = Random.Range(0, Mathf.PI);

            GamePhysicsUtility.TryLineCaseRect(in lbPos, in ruPos, in data.point, in lineAngle, ref point1, ref point2);

            if (Random.Range(0, 100) > 50)
            {
                (point2, point1) = (point1, point2);
            }

            var dir = (point2 - point1).normalized;

            point1 += -dir * 4f;
            point2 += dir * 4f;

            swordSystem.CreateSwordEntity(in _SourceBallId, in star, data.damageValue, in _OwnerEntityId, in point1, in point2);

            AudioMgr.Instance.Play(EnAudioId.BallPart1011_Effect1);
        }

        GameEffectMgr.Instance.PlayEffectOnce(EnGameEffectId.Eff_ball_part_1001_2, WorldPos, 2);
    }
}
public class GameBattleSkillBallPart10011008Entity : GameBattleSkillBallPartEntity
{

}
