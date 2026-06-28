using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SkillTypeSelectItemInfo : IClassPool<ISerializeToIntArrayUserData>, ISerializeToIntArray
{
    public List<int> SerializeToIntArray()
    {
        var result = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref result, _OperationType);
        ISerializeToIntArray.SerializeToInt(ref result, in _OperationInfo);
        ISerializeToIntArray.SerializeToInt(ref result, in AtkStageData);
        return result;
    }

    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _OperationType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _OperationInfo, userData);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out AtkStageData, userData);
    }
    public EnOperationType _OperationType = EnOperationType.None;
    public IOperationInfo _OperationInfo = null;
    public SkillStageInfo AtkStageData = null;
    public EnTypeId GetTypeDefineId() => EnTypeId.SkillTypeSelectItemInfo;


    public void OnPoolDestroy()
    {
        ClassPoolMgr.Instance.Push(_OperationInfo);
        ClassPoolMgr.Instance.Push(AtkStageData);
        AtkStageData = null;
        _OperationInfo = null;
        _OperationType = EnOperationType.None;
    }

    public void OnPoolInit(ISerializeToIntArrayUserData userData)
    {
        DeserializeFromIntArray(userData.arrParams, userData.startIndex, userData.count, userData);
    }

    public void OnPoolEnable()
    {
    }


    public void PoolConstructor()
    {
    }

    public void PoolRelease()
    {
    }

}