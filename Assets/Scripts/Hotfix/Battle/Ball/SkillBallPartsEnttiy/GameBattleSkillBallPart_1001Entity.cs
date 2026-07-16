using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBallPart_1001EntityData : GameBattleSkillBallPartEntityData<GameBattleSkillBallPart_1001Entity, GameBattleSkillBallPart_1001StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBallPart_1001Entity;

    private GamePhysicsHit[] _Hits = new GamePhysicsHit[30];

    protected override void OnPoolInit(GameBattleSkillBallPartEntityUserData userData)
    {
        base.OnPoolInit(userData);
        EventMgr.Instance.Register<ValnerableBuffTriggerEventData>(EnABBEvent.EVENT_BUFF_TRIGGER, (int)EnBuff.Vulnerable, 0, OnEvent_BuffTrigger);
    }
    public override void OnPoolDestroy()
    {
        EventMgr.Instance.Unregister<ValnerableBuffTriggerEventData>(EnABBEvent.EVENT_BUFF_TRIGGER, (int)EnBuff.Vulnerable, 0, OnEvent_BuffTrigger);

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
                GameAttackMgr.Instance.Attack(in _OwnerEntityId, in hit.entityId, in atkDamage, in centerPos, starData.atkDamageType);
            }
        }

        GameEffectMgr.Instance.PlayEffectOnce(EnGameEffectId.Eff_ball_part_1001_1, worldPos, starData.radius / 0.5f);

        GameEffectMgr.Instance.PlayEffectOnce(EnGameEffectId.Eff_ball_part_1001_2, WorldPos);
    }
}
public class GameBattleSkillBallPart_1001Entity : GameBattleSkillBallPartEntity
{

}
