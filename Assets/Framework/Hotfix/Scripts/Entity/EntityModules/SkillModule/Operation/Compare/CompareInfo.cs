




using System.Collections.Generic;

public class CompareLessInfo : CompareInfo
{
    public override EnTypeId GetTypeDefineId() => EnTypeId.CompareLessInfo;
    public override EnOperationCompareType GetCompareType() => EnOperationCompareType.Less;

    public override bool CompareResult(int target)
    {
        return target < value;
    }
}
public class CompareEqualInfo : CompareInfo
{
    public override EnTypeId GetTypeDefineId() => EnTypeId.CompareEqualInfo;
    public override EnOperationCompareType GetCompareType() => EnOperationCompareType.Equal;

    public override bool CompareResult(int target)
    {
        return target == value;
    }

}
public class CompareGreaterInfo : CompareInfo
{
    public override EnTypeId GetTypeDefineId() => EnTypeId.CompareGreaterInfo;
    public override EnOperationCompareType GetCompareType() => EnOperationCompareType.Greater;

    public override bool CompareResult(int target)
    {
        return target > value;
    }

}



public abstract class CompareInfo : ICompareInfo
{
    public List<int> SerializeToIntArray()
    {
        var result = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref result, in value);
        return result;
    }

    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out value);
    }
    public int value;
    public abstract EnOperationCompareType GetCompareType();
    public abstract EnTypeId GetTypeDefineId();
    public abstract bool CompareResult(int target);

    public void OnPoolDestroy()
    {
        value = -1;
    }
    public void OnPoolInit(ISerializeToIntArrayUserData userData)
    {
        DeserializeFromIntArray(userData.arrParams, userData.startIndex, userData.count, userData);
    }

}