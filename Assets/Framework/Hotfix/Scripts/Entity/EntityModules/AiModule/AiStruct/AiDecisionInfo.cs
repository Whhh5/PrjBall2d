
using System.Collections.Generic;

public class AiDecisionInfo : IAiDecisionInfo
{
    public List<int> SerializeToIntArray()
    {
        var result = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref result, in _Decision);
        return result;
    }

    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _Decision, userData);
    }

    private IAiDecision _Decision = null;
    public EnTypeId GetTypeDefineId() => EnTypeId.AiDecisionInfos;

    public void OnPoolDestroy()
    {
        ISerializeToIntArray.Release(ref _Decision);
    }

    public void OnPoolInit(AiCommonUserData userData)
    {
    }

    public void Execute(in List<IAiPerception> perceptions)
    {
        _Decision.Execute(in perceptions);
    }
}
