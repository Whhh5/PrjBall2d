using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameReboundSpeedSystem : IBattleSystem
{
    public void OnDisable()
    {
        EventMgr.Instance.Unregister<GameSkillBallReflectEventData>(EnABBEvent.EVENT_BATTLE_BALL_REFLECT, 0, 0, OnEvent_BallReflect);
    }

    public void OnEnable()
    {
        EventMgr.Instance.Register<GameSkillBallReflectEventData>(EnABBEvent.EVENT_BATTLE_BALL_REFLECT, 0, 0, OnEvent_BallReflect);
    }

    public void OnReset()
    {
    }

    public void OnUpdate()
    {
    }

    private void OnEvent_BallReflect(in int entityId, in int _, in GameSkillBallReflectEventData data)
    {
        var ballEntityData = EntityMgr.Instance.GetEntityData<GameBattleSkillBallEntityData>(entityId);
        var ownerEntityId = ballEntityData.GetOwnerEntityId();
        var reboundValue = GameAttributeUtility.GetMiscValue(ownerEntityId, EnMiscId.ReboundSpeed);
        if (Mathf.Approximately(reboundValue, 0))
            return;
        GameAttributeUtility.AddAddtionalPercentage(entityId, EnEntityAttributeId.MoveSpeed, reboundValue / 100);
    }
}
