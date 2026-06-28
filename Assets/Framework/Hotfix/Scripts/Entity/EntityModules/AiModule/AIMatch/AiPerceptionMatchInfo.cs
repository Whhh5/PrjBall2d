
using System.Collections.Generic;

public enum EnPerceptionMatchType
{
    None,
    Contains,
}
public class AiPerceptionMatchInfo : IAiPerceptionMatchInfo<AiPerceptionTestResult>
{
    public EnAiPerceptionMatchInfoType GetAiPerceptionMatchInfoType() => EnAiPerceptionMatchInfoType.AiPerceptionMatchInfo;
    public List<int> SerializeToIntArray()
    {
        var result = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref result, _PerceptionId);
        ISerializeToIntArray.SerializeToInt(ref result, _MatchType);
        ISerializeToIntArray.SerializeToInt(ref result, in _PerceptionTestData);
        return result;
    }
    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _PerceptionId);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _MatchType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _PerceptionTestData, userData);
    }

    private EnAiPerceptionId _PerceptionId;
    private EnPerceptionMatchType _MatchType;
    private IAiPerceptionTest[] _PerceptionTestData;
    public EnTypeId GetTypeDefineId() => EnTypeId.AiPerceptionMatchInfo;

    public void OnPoolDestroy()
    {
        ISerializeToIntArray.Release(ref _PerceptionTestData);
        ISerializeToIntArray.Release(ref _PerceptionId);
        ISerializeToIntArray.Release(ref _MatchType);
    }

    public void OnPoolInit(AiCommonUserData userData)
    {

    }

    public ref readonly EnAiPerceptionId GetPerceptionId() => ref _PerceptionId;

    public bool IsPassable(in AiDecisionGeneratorTestInput testInput)
    {
        var result = TestMathPass(testInput.testResults.Count);
        return result;
    }
    public bool Test(in List<int> entityIdList, ref AiPerceptionTestResult testResult)
    {
        for (int i = 0; i < entityIdList.Count; i++)
        {
            var entityId = entityIdList[i];
            if (!TestEntityPass(in entityId))
                continue;
            testResult.entityIds.Add(entityId);
        }
        return testResult.entityIds.Count > 0;
    }
    private bool TestEntityPass(in int entityId)
    {
        for (int i = 0; i < _PerceptionTestData.Length; i++)
        {
            if (!_PerceptionTestData[i].IsPassable(in entityId))
                return false;
        }
        return true;
    }

    private bool TestMathPass(int count)
    {
        switch (_MatchType)
        {
            case EnPerceptionMatchType.Contains:
                return count != 0;
            case EnPerceptionMatchType.None:
            default:
                break;
        }

        return true;
    }
}
