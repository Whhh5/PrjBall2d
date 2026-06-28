

using System.Collections.Generic;


public interface IAiDecision<T> : IAiDecision, ISerializeToIntArray<T>
    where T : ISerializeToIntArrayUserData
{

}
public interface IAiDecision : ISerializeToIntArray
{
    public EnAiDecisionId GetDecisionId();
    public void Execute(in List<IAiPerception> perceptions);
}