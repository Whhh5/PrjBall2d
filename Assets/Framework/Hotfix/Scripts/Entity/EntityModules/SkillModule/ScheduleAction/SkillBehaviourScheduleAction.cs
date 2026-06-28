using System;
using System.Collections.Generic;

public enum EnSkillBehaviourType
{
    None,
    [EditorFieldName("高度")]
    Height,
}

public class SkillBehaviourScheduleAction : ISkillScheduleAction
{
    public List<int> SerializeToIntArray()
    {
        var result = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref result, schedule);
        ISerializeToIntArray.SerializeToInt(ref result, behaviourType);
        ISerializeToIntArray.SerializeToInt(ref result, _SkillBehaviour);
        return result;
    }
    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out schedule);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out behaviourType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _SkillBehaviour, userData);
    }
    public float schedule;
    public EnSkillBehaviourType behaviourType;
    public ISkillBehaviour _SkillBehaviour;
    public EnTypeId GetTypeDefineId() => EnTypeId.SkillBehaviourScheduleAction;
    public EnAtkLinkScheculeType GetScheduleType() => EnAtkLinkScheculeType.Behaviour;


    public void OnPoolDestroy()
    {
        ClassPoolMgr.Instance.Push(_SkillBehaviour);

        schedule = -1;
        behaviourType = EnSkillBehaviourType.None;
        _SkillBehaviour = null;

    }
    public void OnPoolInit(ISerializeToIntArrayUserData userData)
    {
        DeserializeFromIntArray(userData.arrParams, userData.startIndex, userData.count, userData);
    }

    public void GetEventList(ref List<SkillItemEventInfo> eventList)
    {
        var eventData = ClassPoolMgr.Instance.Pull<SkillItemEventInfo>();
        eventData.schedule = schedule;
        eventData.onEvent = ScheduleEvent;
        eventList.Add(eventData);
    }

    public void ScheduleEvent(int entityID, IClassPoolUserData userData)
    {
        _SkillBehaviour.Execute(entityID);
    }


    public void Reset()
    {
        
    }


}
