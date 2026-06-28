using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffectScheduleAction : ISkillScheduleAction
{
    public List<int> SerializeToIntArray()
    {
        var resultArr = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref resultArr, in _EffectID);
        ISerializeToIntArray.SerializeToInt(ref resultArr, in _Schedule);
        ISerializeToIntArray.SerializeToInt(ref resultArr, in _OffsetPos);
        ISerializeToIntArray.SerializeToInt(ref resultArr, in _OffsetRot);
        ISerializeToIntArray.SerializeToInt(ref resultArr, in _OffsetScale);
        return resultArr;
    }

    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _EffectID);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _Schedule);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _OffsetPos);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _OffsetRot);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _OffsetScale);
    }
    private int _EffectID;
    private float _Schedule;
    private Vector3 _OffsetPos;
    private Vector3 _OffsetRot;
    private Vector3 _OffsetScale;

    public EnTypeId GetTypeDefineId() => EnTypeId.SkillEffectScheduleAction;
    public EnAtkLinkScheculeType GetScheduleType() => EnAtkLinkScheculeType.Effect;
    public void OnPoolDestroy()
    {
        _Schedule
            = _EffectID
            = -1;
    }

    public void OnPoolInit(ISerializeToIntArrayUserData userData)
    {
        DeserializeFromIntArray(userData.arrParams, userData.startIndex, userData.count, userData);
    }

    public void Reset()
    {
    }

    public void ScheduleEvent(int entityID, IClassPoolUserData userData)
    {
        var pos = EntityMgr.Instance.GetEntityWorldPosOffset(in entityID, _OffsetPos);
        var rot = EntityMgr.Instance.GetEntityRotationOffset(entityID, _OffsetRot);
        var scale = EntityMgr.Instance.GetEntityScaleOffset(in entityID, _OffsetScale);

        EffectMgr.Instance.PlayEffectOnce(_EffectID, pos, rot, scale);
    }
    public void GetEventList(ref List<SkillItemEventInfo> eventList)
    {
        var eventData = ClassPoolMgr.Instance.Pull<SkillItemEventInfo>();
        eventData.schedule = _Schedule;
        eventData.onEvent = ScheduleEvent;
        eventList.Add(eventData);
    }

}