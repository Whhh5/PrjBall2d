using System;
using System.Collections.Generic;
using UnityEngine;


public class SkillBuffScheduleAction : ISkillScheduleAction
{
    public List<int> SerializeToIntArray()
    {
        var result = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref result, in _StartSchedule);
        ISerializeToIntArray.SerializeToInt(ref result, in _EndSchedule);
        ISerializeToIntArray.SerializeToInt(ref result, in _BuffDataParams);
        ISerializeToIntArray.SerializeToInt(ref result, in _IsSectionBuff);

        return result;
    }
    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _StartSchedule);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _EndSchedule);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _BuffDataParams, userData);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _IsSectionBuff);
    }
    private float _StartSchedule;
    private bool _IsSectionBuff = true;
    private float _EndSchedule;
    private IEntityBuffParams _BuffDataParams;
    public EnTypeId GetTypeDefineId() => EnTypeId.SkillBuffScheduleAction;

    public EnAtkLinkScheculeType GetScheduleType() => EnAtkLinkScheculeType.Buff;

    private int _AddBuffKey = -1;
    public void OnPoolDestroy()
    {
        RemoveEntityBuff();
        if (_BuffDataParams != null)
            ClassPoolMgr.Instance.Push(_BuffDataParams);
        _StartSchedule
            = _EndSchedule
            = _AddBuffKey
            = -1;
        _BuffDataParams = null;
    }
    public void OnPoolInit(ISerializeToIntArrayUserData userData)
    {
        DeserializeFromIntArray(userData.arrParams, userData.startIndex, userData.count, userData);
    }

    private void ScheduleEvent(int entityID, IClassPoolUserData userData)
    {
        var addKey = BuffMgr.Instance.AddEntityBuff(entityID, entityID, _BuffDataParams);
        if (BuffMgr.Instance.GetBuffType(_BuffDataParams.GetBuff()) != EnBuffType.Time)
            _AddBuffKey = addKey;

    }
    private void ScheduleEvent2(int entityID, IClassPoolUserData userData)
    {
        RemoveEntityBuff();
    }
    public void Reset()
    {
        RemoveEntityBuff();
    }
    private void RemoveEntityBuff()
    {
        if (_AddBuffKey <= 0)
            return;
        BuffMgr.Instance.RemoveEntityBuff(_AddBuffKey);
        _AddBuffKey = -1;
    }

    public void GetEventList(ref List<SkillItemEventInfo> eventList)
    {
        var eventData = ClassPoolMgr.Instance.Pull<SkillItemEventInfo>();
        eventData.schedule = _StartSchedule;
        eventData.onEvent = ScheduleEvent;
        eventList.Add(eventData);

        if (_IsSectionBuff)
        {
            var eventData2 = ClassPoolMgr.Instance.Pull<SkillItemEventInfo>();
            eventData2.schedule = _EndSchedule;
            eventData2.onEvent = ScheduleEvent2;
            eventList.Add(eventData2);
        }
    }
}
