using System;
using System.Collections.Generic;

public class VirtualTimelineDefaultEvent : IVirtualTimelineEvent<VirtualTimelineUserData>
{
    private int _Value;
    public EnTypeId GetTypeDefineId() => EnTypeId.VirtualTimelineDefaultEvent;

    public void OnPoolDestroy()
    {
        _Value = 0;
    }


    public void OnPoolInit(VirtualTimelineUserData userData)
    {
        DeserializeFromIntArray(userData.arrParams, userData.startIndex, userData.count, userData);
    }

    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _Value);
    }

    public List<int> SerializeToIntArray()
    {
        var result = new List<int>();

        ISerializeToIntArray.SerializeToInt(ref result, in _Value);

        return result;
    }


    public void OnVirtualTimelineEvent()
    {
        ABBUtil.Log($"wjhVirtualTimelineDefaultEvent -> {this.GetHashCode()} -> {_Value}");
    }
}

