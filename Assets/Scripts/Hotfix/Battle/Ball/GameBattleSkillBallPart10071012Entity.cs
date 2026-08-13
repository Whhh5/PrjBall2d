using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBallPart10071012EntityData : GameBattleSkillBallPartEntityData<GameBattleSkillBallPart10071012Entity, GameBattleSkillBallPart10071012StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBallPart10071012Entity;

    private GamePhysicsHit[] _Hits = new GamePhysicsHit[30];
    protected override void OnPoolInit(GameBattleSkillBallPartEntityUserData userData)
    {
        base.OnPoolInit(userData);
        EventMgr.Instance.Register<GameLeachFinishDieEventData>(EnABBEvent.EVENT_BATTLE_LEECH_FINISH_DIE, 0, 0, OnEvent_LeechDie);
    }
    public override void OnPoolDestroy()
    {
        EventMgr.Instance.Unregister<GameLeachFinishDieEventData>(EnABBEvent.EVENT_BATTLE_LEECH_FINISH_DIE, 0, 0, OnEvent_LeechDie);
        base.OnPoolDestroy();
    }
    public override void GameBallOnCollision(in int ballEntityId, in GamePhysicsHit hit)
    {

    }

    private void OnEvent_LeechDie(in int leechEntityId, in int _, in GameLeachFinishDieEventData data)
    {
        if (data.curStoringValue <= 0)
            return;
        var starData = GetBallPartStarData();
        var atkPoint = EntityMgr.Instance.GetEntityWorldPos2D(data.leechEntityId);
        var damageValue = Mathf.CeilToInt(starData.atkValue
            + GetOwnerEntityAttributeDamage(starData.atkAttId) * starData.attPercentage01
            + data.curStoringValue * starData.atkStoringPercentage01);
        var radius = starData.radius + data.curStoringValue * starData.radiusStoringPercentage01;
        var count = GamePhysicsUtility.SphereCastAll(atkPoint, radius, ref _Hits);
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                ref readonly var hit = ref _Hits[i];

                GameAttackMgr.Instance.AttackMonster(_OwnerEntityId, hit.entityId, damageValue, starData.atkDamageType);
            }
        }

        AudioMgr.Instance.Play(EnAudioId.Blood);
        GameEffectMgr.Instance.PlayEffectOnce(EnGameEffectId.Eff_ball_part_10071012_1, atkPoint, radius * 2);
    }
}

public class GameBattleSkillBallPart10071012Entity : GameBattleSkillBallPartEntity
{

}
