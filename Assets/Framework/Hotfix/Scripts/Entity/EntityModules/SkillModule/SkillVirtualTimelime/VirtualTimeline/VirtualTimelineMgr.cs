using Codice.CM.Common;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class VirtualTimelineInfo : IClassPoolDestroy
{
    public int timelineId;
    public int timelineCfgId;
    public float startTime;
    public IVirtualTimeline timeline;
    public bool isStarted = false;

    public void OnPoolDestroy()
    {
        startTime
            = timelineCfgId
            = timelineId
            = -1;
        timeline = null;
        isStarted = false;
    }
}


public class VirtualTimelineMgr : Singleton<VirtualTimelineMgr>
{
    public override EnManagerFuncType FuncType => base.FuncType | EnManagerFuncType.Update;
    private readonly Dictionary<int, int> _TimelineId2Index = new(10);
    private readonly List<VirtualTimelineInfo> _TimelineInfos = new(10);

    public override async UniTask AwakeAsync()
    {
        await base.AwakeAsync();
    }

    public int AddTimeline(in int timelineCfgId, in float startTime)
    {
        var userData = ClassPoolMgr.Instance.Pull<VirtualTimelineUserData>();
        var key = AddTimeline(in timelineCfgId, in userData, in startTime);
        ClassPoolMgr.Instance.Push(userData);
        return key;
    }
    public int AddTimeline(in int timelineCfgId, in VirtualTimelineUserData userData, in float startTime)
    {
        var timelineKey = ABBUtil.GetTempKey();

        var timelineCfg = GameSchedule.Instance.GetTimelineCfg0(timelineCfgId);
        userData.startIndex = 0;
        userData.count = timelineCfg.arrParams.Length;
        userData.arrParams = timelineCfg.arrParams;

        var timeline = TypeDefine.CreateInstance<IVirtualTimeline>(in timelineCfg.nTypeId, userData);
        var info = ClassPoolMgr.Instance.Pull<VirtualTimelineInfo>();
        info.timelineCfgId = timelineCfgId;
        info.timeline = timeline;
        info.startTime = startTime;
        info.timelineId = timelineKey;

        _TimelineId2Index.Add(timelineKey, _TimelineInfos.Count);
        _TimelineInfos.Add(info);

        return timelineKey;
    }
    public VirtualTimelineInfo GetVirtualTimelineInfo(in int timelineKey)
    {
        return _TimelineInfos[_TimelineId2Index[timelineKey]];
    }
    public void RemoveTimeline(in int timelineId)
    {
        if (!_TimelineId2Index.TryGetValue(timelineId, out var index))
        {
            Debug.LogError($"[AbbTimelineMgr] RemoveTimeline failed, timelineID {timelineId} not exists.");
            return;
        }
        var timelineInfo = _TimelineInfos[index];
        var moveTimeInfo = _TimelineInfos[^1];

        _TimelineId2Index[moveTimeInfo.timelineId] = index;

        _TimelineId2Index.Remove(timelineId);
        _TimelineInfos[index] = moveTimeInfo;
        _TimelineInfos.RemoveAt(_TimelineInfos.Count - 1);


        ClassPoolMgr.Instance.Push(in timelineInfo.timeline);
        ClassPoolMgr.Instance.Push(in timelineInfo);
    }

    public override void Update()
    {
        base.Update();

        var timeDelta = ABBUtil.GetTimeDelta();
        var curTime = ABBUtil.GetGameTimeSeconds();
        for (int i = 0; i < _TimelineInfos.Count; i++)
        {
            var timelineInfo = _TimelineInfos[i];
            if (!timelineInfo.isStarted)
            {
                timelineInfo.isStarted = true;
                timelineInfo.timeline.OnVirtualTimelineStart(in timeDelta);
            }

            var timelineId = timelineInfo.timelineId;

            var localTime = curTime - timelineInfo.startTime;
            var totalTime = timelineInfo.timeline.GetVirtualTimelineTotalTime();
            var progress01 = Mathf.Clamp01(localTime / totalTime);

            timelineInfo.timeline.OnVirtualTimelineUpdtae(in localTime, in timeDelta, in progress01);
            if (progress01 == 1)
            {
                timelineInfo.timeline.OnVirtualTimelineEnd(in localTime, in timeDelta);
                ABBEventMgr.Instance.FireExecute(EnABBEvent.OnTimelineEnd, timelineId);

                RemoveTimeline(in timelineInfo.timelineId);
            }
        }
    }




}
