
using System.Collections.Generic;

public interface IAiDecisionSubGenerator<T> : IAiDecisionSubGenerator, ISerializeToIntArray<T>
    where T : ISerializeToIntArrayUserData
{

}
public interface IAiDecisionSubGenerator : ISerializeToIntArray
{
    public EnAiDecisionSubGeneratorType GetSubDecisionGeneratorType();
    public bool TryExecute(in List<int> inputEntityIds, ref List<int> outputEntityIds);
    public void OutputExecutes(in List<int> inputEntityIds, ref AiDecisionGeneratorTestOutputs outputs);
}
