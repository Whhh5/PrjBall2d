using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBallPart1001EntityData : GameBattleSkillBallPartEntityData<GameBattleSkillBallPart1001Entity, GameBattleSkillBallPart1001StarData>

{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBallPart1001Entity;

    private GamePhysicsHit[] _Hits = new GamePhysicsHit[30];

    protected override void OnPoolInit(GameBattleSkillBallPartEntityUserData userData)
    {
        base.OnPoolInit(userData);
        var starData = GetBallPartStarData();
        EventMgr.Instance.Register<ValnerableBuffTriggerEventData>(EnABBEvent.EVENT_BUFF_TRIGGER, (int)starData.buff, 0, OnEvent_BuffTrigger);
    }
    public override void OnPoolDestroy()
    {
        var starData = GetBallPartStarData();
        EventMgr.Instance.Unregister<ValnerableBuffTriggerEventData>(EnABBEvent.EVENT_BUFF_TRIGGER, (int)starData.buff, 0, OnEvent_BuffTrigger);

        base.OnPoolDestroy();
    }
    public override void GameBallOnCollision(in int entityId, in GamePhysicsHit hit)
    {

    }

    private void OnEvent_BuffTrigger(in int nBuffId, in int _, in ValnerableBuffTriggerEventData data)
    {
        var starData = GetBallPartStarData();

        var targetEntityData = EntityMgr.Instance.GetEntityData<IEntityGridPointInfo>(data.targetEntityId);
        var worldPos = targetEntityData.GetCenterPointWorldPos();

        var entityCount = GamePhysicsUtility.SphereCastAll(in worldPos, in starData.radius, ref _Hits);
        if (entityCount > 0)
        {
            var atkDamage = starData.atkDamage + Mathf.CeilToInt(data.finalDamage * starData.atkVulnerableDamagePercentage);
            for (int i = 0; i < entityCount; i++)
            {
                ref readonly var hit = ref _Hits[i];
                var pointInfo = EntityMgr.Instance.GetEntityData<IEntityGridPointInfo>(in hit.entityId);
                var centerPos = pointInfo.GetCenterPointWorldPos();
                GameAttackMgr.Instance.AttackMonster(in _OwnerEntityId, in hit.entityId, in atkDamage, in centerPos, in starData.atkDamageType);
            }
        }

        GameEffectMgr.Instance.PlayEffectOnce(EnGameEffectId.Eff_ball_part_1001_1, worldPos, starData.radius / 0.5f);
        GameEffectMgr.Instance.PlayEffectOnce(EnGameEffectId.Eff_ball_part_1001_2, WorldPos);
        AudioMgr.Instance.Play(EnAudioId.BallPart1001_Effect1);
    }
}
public class GameBattleSkillBallPart1001Entity : GameBattleSkillBallPartEntity
{

}
