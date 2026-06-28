

using System.Collections.Generic;

public interface IAiDecisionInfo : ISerializeToIntArray<AiCommonUserData>
{
    public void Execute(in List<IAiPerception> perceptions);
}