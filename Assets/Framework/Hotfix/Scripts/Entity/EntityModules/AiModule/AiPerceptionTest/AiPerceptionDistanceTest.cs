
using System.Collections.Generic;

public class AiPerceptionDistanceTest : IAiPerceptionTest
{
    public List<int> SerializeToIntArray()
    {
        var result = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref result, in _MinDistance);
        ISerializeToIntArray.SerializeToInt(ref result, in _MaxDistance);
        return result;
    }
    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _MinDistance);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _MaxDistance);
    }
    private float _MinDistance;
    private float _MaxDistance;
    public EnTypeId GetTypeDefineId() => EnTypeId.AiPerceptionDistanceTest;
    public EnAiPerceptionTestType GetAiPerceptionTestType() => EnAiPerceptionTestType.Distance;

    private int _AiId = -1;
    public void OnPoolDestroy()
    {
        ISerializeToIntArray.Release(ref _MinDistance);
        ISerializeToIntArray.Release(ref _MaxDistance);
        _AiId = -1;
    }
    public void OnPoolInit(AiCommonUserData userData)
    {
        _AiId = userData.aiId;
    }
    public bool IsPassable(in int testEntityId)
    {
        var entityId = AiMgr.Instance.GetAiEntityId(in _AiId);
        var disSqr = EntityUtility.GetA2bDisSqr(in entityId, in testEntityId);
        if (disSqr < _MinDistance * _MinDistance)
            return false;
        if (disSqr > _MaxDistance * _MaxDistance)
            return false;
        return true;
    }

}