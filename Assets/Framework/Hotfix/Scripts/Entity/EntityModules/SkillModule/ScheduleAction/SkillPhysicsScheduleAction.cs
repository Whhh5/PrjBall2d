using System;
using System.Collections.Generic;
using UnityEngine;
using static SkillLinkPlayableAdapter;


public class PhysicsOverlapCallbackCustomData : IPhysicsColliderCallbackCustomData
{
    public int atkValue;
    public int entityID;
}

public class SkillPhysicsScheduleAction : ISkillScheduleAction
{
    public List<int> SerializeToIntArray()
    {
        var result = new List<int>();

        ISerializeToIntArray.SerializeToInt(ref result, in _AtkSchedule);
        ISerializeToIntArray.SerializeToInt(ref result, in _AtkValue);
        ISerializeToIntArray.SerializeToInt(ref result, in _EffectID);
        ISerializeToIntArray.SerializeToInt(ref result, in _BuffInfoList);
        ISerializeToIntArray.SerializeToInt(ref result, in _PhysicsResolve);

        return result;
    }

    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _AtkSchedule);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _AtkValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _EffectID);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _BuffInfoList, userData);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _PhysicsResolve, userData);
    }

    private float _AtkSchedule;
    private int _AtkValue;
    private int _EffectID; // 击中特效
    private IEntityBuffParams[] _BuffInfoList;
    private IPhysicsResolve _PhysicsResolve;
    public EnTypeId GetTypeDefineId() => EnTypeId.SkillPhysicsScheduleAction;
    public EnAtkLinkScheculeType GetScheduleType() => EnAtkLinkScheculeType.Physics;


    public void OnPoolDestroy()
    {
        if (_PhysicsResolve != null)
            ClassPoolMgr.Instance.Push(_PhysicsResolve);
        foreach (var buffInfo in _BuffInfoList)
            ClassPoolMgr.Instance.Push(buffInfo);
        _AtkSchedule = -1;
        _AtkValue = -1;
        _EffectID = -1;
        _PhysicsResolve = null;
        _BuffInfoList = null;
    }

    public void OnPoolInit(ISerializeToIntArrayUserData userData)
    {
        DeserializeFromIntArray(userData.arrParams, userData.startIndex, userData.count, userData);
    }

    public void Reset()
    {
    }

    public void GetEventList(ref List<SkillItemEventInfo> eventList)
    {
        var item1 = ClassPoolMgr.Instance.Pull<SkillItemEventInfo>();
        item1.schedule = _AtkSchedule;
        item1.onEvent = ScheduleEvent;
        eventList.Add(item1);
    }

    private void ScheduleEvent(int entityID, IClassPoolUserData userData)
    {
        var data = new PhysicsOverlapCallbackCustomData()
        {
            atkValue = _AtkValue,
            entityID = entityID,
        };
        var layer = SceneEntityMgr.Instance.GetEntityEnemyLayer(entityID);
        PhysicsMgr.Instance.PhysicsOverlap(in _PhysicsResolve, in entityID, in layer, PhysicsOverlapCallback, data);
    }


    private void PhysicsOverlapCallback(ref EntityPhysicsInfo[] entityIDs, ref int count,
        IPhysicsColliderCallbackCustomData customData)
    {
        var data = customData as PhysicsOverlapCallbackCustomData;
        for (var i = 0; i < count; i++)
        {
            ref var entityInfo = ref entityIDs[i];

            if (_EffectID > 0)
                EffectMgr.Instance.PlayEffectOnce(_EffectID, entityInfo.closestPoint);

            for (var j = 0; j < _BuffInfoList?.Length; j++)
            {
                var buffInfo = _BuffInfoList[j];
                BuffMgr.Instance.AddEntityBuff(data.entityID, entityInfo.entityID, buffInfo);
            }
        }
    }
}