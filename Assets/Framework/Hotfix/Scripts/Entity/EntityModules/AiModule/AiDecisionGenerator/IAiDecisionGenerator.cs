using System.Collections.Generic;
using dnlib.DotNet;


public class AiDecisionGeneratorTestInput : IClassPoolDestroy
{
    public EnAiPerceptionId testPerceptionId;
    public List<IAiPerceptionTestResult> testResults = new();
    public void OnPoolDestroy()
    {
        foreach (var item in testResults)
            ClassPoolMgr.Instance.Push(item);
        testPerceptionId = EnAiPerceptionId.None;
        testResults.Clear();
    }
}
public class AiDecisionGeneratorTestExecutionInfo : IClassPoolDestroy
{
    public EnAiExecutionType executionType;
    public IClassPoolUserData userData;
    public void OnPoolDestroy()
    {
        ClassPoolMgr.Instance.Push(userData);
        executionType = EnAiExecutionType.None;
        userData = null;
    }
}

public class AiDecisionGeneratorTestOutputs : IClassPoolDestroy
{
    public readonly List<AiDecisionGeneratorTestExecutionInfo> outputs = new(2);
    public void OnPoolDestroy()
    {
        foreach (var item in outputs)
            ClassPoolMgr.Instance.Push(item);
        outputs.Clear();
    }
}
public class AiDecisionGeneratorTestInputs : IClassPoolDestroy
{
    public List<AiDecisionGeneratorTestInput> inputs = new(2);
    public void OnPoolDestroy()
    {
        foreach (var item in inputs)
            ClassPoolMgr.Instance.Push(item);
        inputs.Clear();
    }
}

public interface IAiDecisionGenerator<T> : IAiDecisionGenerator, ISerializeToIntArray<T>
    where T : ISerializeToIntArrayUserData
{
}
public interface IAiDecisionGenerator : ISerializeToIntArray
{
    public EnAiDecisionGeneratorType GetDecisionGeneratorType();
    public ref readonly IAiPerceptionMatchInfo[] GetPerceptionInfos();
    public bool TryGetExecuteCmd(in AiDecisionGeneratorTestInputs inputs, ref AiDecisionGeneratorTestOutputs outputs);
}

public enum EnAiDecisionGeneratorType
{
    None = 0,
    Cmd,
    Idle,
}