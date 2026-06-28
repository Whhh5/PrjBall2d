using System.Collections.Generic;

public class AiDecisionGeneratorInfo : ISerializeToIntArray<AiCommonUserData>
{
    public List<int> SerializeToIntArray()
    {
        var result = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref result, _DecisionLevel);
        ISerializeToIntArray.SerializeToInt(ref result, in _DecisionGenerator);
        return result;
    }
    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _DecisionLevel);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _DecisionGenerator, userData);
    }

    private EnDecisionLevel _DecisionLevel = EnDecisionLevel.None;
    private IAiDecisionGenerator _DecisionGenerator = null;
    public EnTypeId GetTypeDefineId() => EnTypeId.AiDecisionGeneratorInfo;

    public void OnPoolDestroy()
    {
        ISerializeToIntArray.Release(ref _DecisionGenerator);
        ISerializeToIntArray.Release(ref _DecisionLevel);
    }

    public void OnPoolInit(AiCommonUserData userData)
    {
       
    }
    public ref readonly IAiDecisionGenerator GetDecisionGenerator() => ref _DecisionGenerator;
}