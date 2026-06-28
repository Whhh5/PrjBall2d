using System;
using System.Collections.Generic;

public interface ISkillScheduleAction : IClassPoolInit<ISerializeToIntArrayUserData>, ISerializeToIntArray
{
    public EnAtkLinkScheculeType GetScheduleType();
    public void Reset();
    public void GetEventList(ref List<SkillItemEventInfo> eventList);
}