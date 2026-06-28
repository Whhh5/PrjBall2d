using System.Collections.Generic;

public abstract class SkillVirtualTimelinePhysicsEvent : ISkillVirtualTimelineEvent
{
    protected int _TimelineEventId = -1;
    public abstract EnTypeId GetTypeDefineId();

    protected int _SkillId;
    protected int _EntityId;
    // private int _LinkId = -1;

    public abstract EnSkillPhysicsType GetSkillPhysicsType();

    public void OnPoolDestroy()
    {
        _SkillId
            = _EntityId 
                // = _LinkId
            = -1;
    }
    public void OnPoolInit(SkillVirtualTimelineUserData userData)
    {
        _SkillId = userData.skillId;
        _EntityId = userData.entityId;
        DeserializeFromIntArray(userData.arrParams, userData.startIndex, userData.count, userData);
    }

    public abstract void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData);
    public abstract List<int> SerializeToIntArray();

    public abstract void OnVirtualTimelineEvent();
}

