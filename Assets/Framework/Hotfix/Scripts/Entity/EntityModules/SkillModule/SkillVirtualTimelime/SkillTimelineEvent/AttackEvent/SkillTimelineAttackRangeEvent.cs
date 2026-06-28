


using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillTimelineAttackRangeEvent : ISkillTimelineAttackEvent
{
    public List<int> SerializeToIntArray()
    {
        var resultArr = new List<int>();

        ISerializeToIntArray.SerializeToInt(ref resultArr, in _PosOffset.x);
        ISerializeToIntArray.SerializeToInt(ref resultArr, in _PosOffset.y);
        ISerializeToIntArray.SerializeToInt(ref resultArr, in _PosOffset.z);
        ISerializeToIntArray.SerializeToInt(ref resultArr, in _RotOffset.x);
        ISerializeToIntArray.SerializeToInt(ref resultArr, in _RotOffset.y);
        ISerializeToIntArray.SerializeToInt(ref resultArr, in _RotOffset.z);
        ISerializeToIntArray.SerializeToInt(ref resultArr, in _Value);
        ISerializeToIntArray.SerializeToInt(ref resultArr, (int)_CaculateId);

        resultArr.Insert(0, resultArr.Count);

        var childData = SerializeToIntArray2();
        resultArr.Add(childData.Count);
        resultArr.AddRange(childData);

        return resultArr;
    }
    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        var dataCount = ISerializeToIntArray.ParseToValue(in datas, start++, in count, localIndex);
        ISerializeToIntArray.ParseToValue(in datas, in start, in dataCount, ref localIndex, out _PosOffset.x);
        ISerializeToIntArray.ParseToValue(in datas, in start, in dataCount, ref localIndex, out _PosOffset.y);
        ISerializeToIntArray.ParseToValue(in datas, in start, in dataCount, ref localIndex, out _PosOffset.z);
        ISerializeToIntArray.ParseToValue(in datas, in start, in dataCount, ref localIndex, out _RotOffset.x);
        ISerializeToIntArray.ParseToValue(in datas, in start, in dataCount, ref localIndex, out _RotOffset.y);
        ISerializeToIntArray.ParseToValue(in datas, in start, in dataCount, ref localIndex, out _RotOffset.z);
        ISerializeToIntArray.ParseToValue(in datas, in start, in dataCount, ref localIndex, out _Value);
        ISerializeToIntArray.ParseToValue(in datas, in start, in dataCount, ref localIndex, out _CaculateId);

        if (dataCount < count)
        {
            var startIndex = dataCount;
            ISerializeToIntArray.ParseToValue(in datas, start, in count, ref startIndex, out int childCount);
            DiserializeFromIntArray2(datas, start + startIndex, childCount, userData);
        }
    }
    protected abstract List<int> SerializeToIntArray2();
    protected abstract void DiserializeFromIntArray2(int[] datas, int start, int count, ISerializeToIntArrayUserData userData);

    private Vector3 _PosOffset;
    private Vector3 _RotOffset;
    private int _Value;
    private EnAttackCaculateId _CaculateId;
    public abstract EnTypeId GetTypeDefineId();
    protected int _EntityId;
    private int _SkillId;

    public Vector3 GetCaculateCastWorldPos()
    {
        var worldPos = EntityMgr.Instance.GetEntityWorldPosOffset(in _EntityId, in _PosOffset);
        return worldPos;
    }
    public Vector3 GetCaculateCastRotation()
    {
        var rot = EntityMgr.Instance.GetEntityRotationOffset(in _EntityId, in _RotOffset);
        return rot;
    }
    public virtual void OnPoolDestroy()
    {
        _Value
            = _EntityId
            = -1;
        _PosOffset
            = _RotOffset
            = Vector3.zero;
        _CaculateId = EnAttackCaculateId.None;
    }

    public virtual void OnPoolInit(SkillVirtualTimelineUserData userData)
    {
        _EntityId = userData.entityId;
        _SkillId = userData.skillId;
        DeserializeFromIntArray(userData.arrParams, userData.startIndex, userData.count, userData);
    }

    public void OnVirtualTimelineEvent()
    {
        var entityCount = CaculateCastEntityIdCount();
        if (entityCount == 0)
            return;

        for (int i = 0; i < entityCount; i++)
        {
            ref readonly var entityId = ref SkillPhysicsMgr.Instance.GetLastCastEntityId(in i);

            var atkValue = 0;// AttackUtility.CaculateAttackValue(in _EntityId, in entityId, in _Value, in _CaculateId);

            var eventData = ClassPoolMgr.Instance.Pull<EventSkillAttackData>();
            eventData.attackerEntityId = _EntityId;
            eventData.hitEntityId = entityId;
            eventData.value = atkValue;
            ABBEventMgr.Instance.FireExecute(EnABBEvent.EVENT_SKILL_ATTACK, eventData);
            ClassPoolMgr.Instance.Push(eventData);
        }
    }

    protected abstract int CaculateCastEntityIdCount();
}

