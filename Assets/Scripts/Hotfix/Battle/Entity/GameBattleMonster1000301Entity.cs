using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameBattleMonster1000301EntityData : GameBattleMonsterEntityData
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleMonster1000301Entity;

    protected override void OnPoolInit(GameBattleMonsterEntityUserData userData)
    {
        base.OnPoolInit(userData);

        EventMgr.Instance.Register<GameSkillMonsterAttack1EventData>(EnABBEvent.EVENT_SKILL_EVENT_SCHEDULE, _EntityID, (int)EnSkillEventScheduleId.Attack, OnEvent_SkillEventSchedule_Attack);
    }
    public override void OnPoolDestroy()
    {
        EventMgr.Instance.Unregister<GameSkillMonsterAttack1EventData>(EnABBEvent.EVENT_SKILL_EVENT_SCHEDULE, _EntityID, (int)EnSkillEventScheduleId.Attack, OnEvent_SkillEventSchedule_Attack);

        base.OnPoolDestroy();
    }

    protected override Vector2 GetLocalCenterPointPos()
    {
        return new Vector2(1f, -1f);
    }

    private void OnEvent_SkillEventSchedule_Attack(in int nEntityId, in int nEventId, in GameSkillMonsterAttack1EventData data)
    {
        Debug.Log("attack event");
    }

}
public class GameBattleMonster1000301Entity : GameBattleMonsterEntity<GameBattleMonster1000301EntityData>
{

}