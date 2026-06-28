using System.Collections.Generic;

public enum EnAiPerceptionMatchInfoType
{
    None = 0,
    AiPerceptionMatchInfo,
}

public interface IAiPerceptionMatchInfo<TTestResult> : IAiPerceptionMatchInfo
    where TTestResult : IAiPerceptionTestResult, new()
{
    bool IAiPerceptionMatchInfo.Test(in List<int> entityIdList, out IAiPerceptionTestResult testResult)
    {
        var testResult2 = ClassPoolMgr.Instance.Pull<TTestResult>();
        var result = Test(in entityIdList, ref testResult2);
        testResult = result ? testResult2 : null;
        if (!result)
            ClassPoolMgr.Instance.Push(testResult2);
        return result;
    }
    public bool Test(in List<int> entityIdList, ref TTestResult testResult);
}
public interface IAiPerceptionMatchInfo : ISerializeToIntArray<AiCommonUserData>
{
    public EnAiPerceptionMatchInfoType GetAiPerceptionMatchInfoType();
    public ref readonly EnAiPerceptionId GetPerceptionId();
    public bool Test(in List<int> entityIdList, out IAiPerceptionTestResult testResult);
    public bool IsPassable(in AiDecisionGeneratorTestInput testInput);
}