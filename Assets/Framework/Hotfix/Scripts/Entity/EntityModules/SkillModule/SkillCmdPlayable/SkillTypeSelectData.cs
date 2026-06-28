using System.Collections.Generic;

public class SkillTypeSelectDataUserData : IClassPoolUserData
{
    public int[] arrParams = null;

    public void OnPoolDestroy()
    {
        arrParams = null;
    }
}
public class SkillTypeSelectData : ISkillTypeData<SkillPlayableAdapterUserData>, ISerializeToIntArray
{
    public List<int> SerializeToIntArray()
    {
        var result = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref result, _Target);
        ISerializeToIntArray.SerializeToInt(ref result, in _ArrItemInfo);
        return result;
    }
    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _Target);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _ArrItemInfo, userData);
    }
    private EnEntityProperty _Target = EnEntityProperty.None;
    private SkillTypeSelectItemInfo[] _ArrItemInfo = null;
    public EnTypeId GetTypeDefineId() => EnTypeId.SkillTypeSelectData;

    public void OnPoolDestroy()
    {
        //ClassPoolMgr.Instance.Push(propertyInfo);
        foreach (var item in _ArrItemInfo)
            ClassPoolMgr.Instance.Push(item);

        _ArrItemInfo = null;
        _Target = EnEntityProperty.None;
    }
    public void OnPoolInit(SkillPlayableAdapterUserData userData)
    {
        DeserializeFromIntArray(userData.arrParams, userData.startIndex, userData.count, userData);
    }

    public SkillStageInfo CompareResult(int target)
    {
        for (int i = 0; i < _ArrItemInfo.Length; i++)
        {
            var item = _ArrItemInfo[i];
            if (!item._OperationInfo.CompareResult(target))
                continue;
            return item.AtkStageData;
        }
        return null;
    }

}

