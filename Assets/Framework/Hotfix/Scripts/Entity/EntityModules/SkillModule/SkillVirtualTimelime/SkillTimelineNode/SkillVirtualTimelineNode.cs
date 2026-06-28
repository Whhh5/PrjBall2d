using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillVirtualTimelineNode : IVirtualTimelineNode<VirtualTimelineUserData>
{
    public float _DurationTime = -1;
    private SkillVirtualTimelineEventInfo[] _TimelineEvents = null;
    public abstract EnTypeId GetTypeDefineId();

    private int _NextEventIndex = 0;

    public void OnPoolDestroy()
    {
        _DurationTime
            = -1;
        _NextEventIndex
            = 0;
    }
    public virtual void OnPoolInit(VirtualTimelineUserData userData)
    {
        DeserializeFromIntArray(userData.arrParams, userData.startIndex, userData.count, userData);
    }

    public float GetVirtualTimelineNodeDurationTime()
    {
        return _DurationTime;
    }

    public void OnVirtualTimelineNodeUpdate(in float nodeLocalTime, in float deltaTime, in float nodeProgress01)
    {
        var progress01 = Mathf.Clamp01(nodeLocalTime / _DurationTime);
        while (_NextEventIndex < _TimelineEvents.Length)
        {
            ref var nextEvent = ref _TimelineEvents[_NextEventIndex];
            if (progress01 < nextEvent.clickProgress01)
                break;
            _NextEventIndex++;
            nextEvent.timelineEvent.OnVirtualTimelineEvent();
        }
    }

    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _DurationTime);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _TimelineEvents, userData);
    }

    public List<int> SerializeToIntArray()
    {
        var resultArr = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref resultArr, in _DurationTime);
        ISerializeToIntArray.SerializeToInt(ref resultArr, in _TimelineEvents);
        return resultArr;
    }

    public virtual void OnVirtualTimelineNodeStart()
    {
    }

    public virtual void OnVirtualTimelineNodeEnd()
    {
    }

    public virtual bool IsVirtualTimelineNodeEnd(in float nodeLocalTime, in float deltaTime, in float nodeProgress01)
    {
        return nodeProgress01 == 1;
    }
}