using System.Collections.Generic;
using UnityEngine;

public class SkillVirtualTimelineNodePlayableInfo : IClassPoolInit<SkillVirtualTimelineUserData>, ISerializeToIntArray
{
    public float startProgress01;
    public ISkillVirtualTimelinePlayableNode timelineNode = null;

    public EnTypeId GetTypeDefineId() => EnTypeId.SkillVirtualTimelineNodePlayableInfo;
    public void OnPoolDestroy()
    {
        ClassPoolMgr.Instance.Push(in timelineNode);
        startProgress01
            = -1;
        timelineNode = null;
    }


    public void OnPoolInit(SkillVirtualTimelineUserData userData)
    {
        DeserializeFromIntArray(userData.arrParams, userData.startIndex, userData.count, userData);
    }

    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out startProgress01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out timelineNode, userData);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out timelineNode, userData);
    }
    public List<int> SerializeToIntArray()
    {
        var resultArr = new List<int>();

        ISerializeToIntArray.SerializeToInt(ref resultArr, in startProgress01);
        ISerializeToIntArray.SerializeToInt(ref resultArr, in timelineNode);

        return resultArr;
    }

}
public class SkillVirtualTimelineEventInfo : IClassPoolInit<SkillVirtualTimelineUserData>, ISerializeToIntArray
{
    public float clickProgress01;
    public IVirtualTimelineEvent timelineEvent = null;

    public EnTypeId GetTypeDefineId() => EnTypeId.SkillVirtualTimelineEventPlayableInfo;

    public void OnPoolDestroy()
    {
        ClassPoolMgr.Instance.Push(timelineEvent);
        clickProgress01
            = -1;
        timelineEvent = null;
    }

    public void OnPoolInit(SkillVirtualTimelineUserData userData)
    {
        DeserializeFromIntArray(userData.arrParams, userData.startIndex, userData.count, userData);
    }

    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out clickProgress01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out timelineEvent, in userData);
    }
    public List<int> SerializeToIntArray()
    {
        var resultArr = new List<int>();

        ISerializeToIntArray.SerializeToInt(ref resultArr, in clickProgress01);
        ISerializeToIntArray.SerializeToInt(ref resultArr, in timelineEvent);

        return resultArr;
    }

}
public class SkillVirtualTimeline : IVirtualTimeline<SkillVirtualTimelineUserData>
{
    private float _TotalTime = 0;
    private SkillVirtualTimelineNodePlayableInfo[] _SkillNodeInfos = null;
    private SkillVirtualTimelineEventInfo[] _TimelineEvents = null;

    private List<int> _CurUpdateNodeIndexs = new(1);
    public EnTypeId GetTypeDefineId() => EnTypeId.VirtualSkillTimeline;

    private int _NextEventIndex = 0;
    private int _NextNodeIndex = 0;
    public void OnPoolDestroy()
    {
        for (int i = 0; i < _CurUpdateNodeIndexs.Count; i++)
        {
            var nodeIndex = _CurUpdateNodeIndexs[i];
            ref var node = ref _SkillNodeInfos[nodeIndex];
            node.timelineNode.OnVirtualTimelineNodeEnd();
        }
        for (int i = 0; i < _SkillNodeInfos?.Length; i++)
            ClassPoolMgr.Instance.Push(_SkillNodeInfos[i]);
        for (int i = 0; i < _TimelineEvents?.Length; i++)
            ClassPoolMgr.Instance.Push(_TimelineEvents[i]);
        _SkillNodeInfos = null;
        _TimelineEvents = null;
        _TotalTime
            = _NextEventIndex
            = _NextNodeIndex
            = 0;
        _CurUpdateNodeIndexs.Clear();
    }


    public void OnPoolInit(SkillVirtualTimelineUserData userData)
    {
        DeserializeFromIntArray(userData.arrParams, userData.startIndex, userData.count, userData);
    }

    public ref readonly float GetVirtualTimelineTotalTime()
    {
        return ref _TotalTime;
    }

    public void OnVirtualTimelineEnd(in float localTime, in float deltaTime)
    {

    }

    public void OnVirtualTimelineStart(in float localTime)
    {

    }

    public void OnVirtualTimelineUpdtae(in float localTime, in float deltaTime, in float progress01)
    {
        // event
        while (_NextEventIndex < _TimelineEvents.Length)
        {
            ref var nextEvent = ref _TimelineEvents[_NextEventIndex];
            if (progress01 < nextEvent.clickProgress01)
                break;
            _NextEventIndex++;
            nextEvent.timelineEvent.OnVirtualTimelineEvent();
        }

        // animation
        UpdateNodes(in localTime, in deltaTime, in progress01);

    }

    private void UpdateNodes(in float localTime, in float deltaTime, in float progress01)
    {
        // start
        while (_NextNodeIndex < _SkillNodeInfos.Length)
        {
            ref var nextNode = ref _SkillNodeInfos[_NextEventIndex];
            if (progress01 < nextNode.startProgress01)
                break;
            _CurUpdateNodeIndexs.Add(_NextNodeIndex++);
            nextNode.timelineNode.OnVirtualTimelineNodeStart();
        }
        // update
        for (int i = 0; i < _CurUpdateNodeIndexs.Count; i++)
        {
            var nodeIndex = _CurUpdateNodeIndexs[i];
            ref var node = ref _SkillNodeInfos[nodeIndex];
            var nodeDuration = node.timelineNode.GetVirtualTimelineNodeDurationTime();
            var nodeStartTime = node.startProgress01 * _TotalTime;

            var nodeLocalTime = localTime - nodeStartTime;
            var nodeProgress01 = Mathf.Clamp01(nodeLocalTime / nodeDuration);
            node.timelineNode.OnVirtualTimelineNodeUpdate(in nodeLocalTime, in deltaTime, in nodeProgress01);

            // end
            if (node.timelineNode.IsVirtualTimelineNodeEnd(in nodeLocalTime, in deltaTime, in nodeProgress01))
            {
                node.timelineNode.OnVirtualTimelineNodeEnd();
            }
        }
    }

    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _TotalTime);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _SkillNodeInfos, in userData);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _TimelineEvents, in userData);
    }

    public List<int> SerializeToIntArray()
    {
        var resultArr = new List<int>();

        ISerializeToIntArray.SerializeToInt(ref resultArr, in _TotalTime);
        ISerializeToIntArray.SerializeToInt(ref resultArr, in _SkillNodeInfos);
        ISerializeToIntArray.SerializeToInt(ref resultArr, in _TimelineEvents);

        return resultArr;
    }
}
