
using System.Collections.Generic;

public abstract class AiDecisionSubGenerator : IAiDecisionSubGenerator<AiCommonUserData>
{
    public List<int> SerializeToIntArray()
    {
        var result = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref result, in _AiExecuteConditions);
        return result;
    }

    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _AiExecuteConditions, userData);
    }
    private IAiCondition[] _AiExecuteConditions = null;
    public abstract EnTypeId GetTypeDefineId();

    protected int _AiId;
    public abstract EnAiDecisionSubGeneratorType GetSubDecisionGeneratorType();
    public void OnPoolDestroy()
    {
        ISerializeToIntArray.Release(ref _AiExecuteConditions);
        _AiId = -1;
    }

    public void OnPoolInit(AiCommonUserData userData)
    {
        _AiId = userData.aiId;
    }

    public bool TryExecute(in List<int> inputEntityIds, ref List<int> outputEntityIds)
    {
        for (var i = 0; i < inputEntityIds.Count; i++)
        {
            var entityId = inputEntityIds[i];
            if (IsPassEntity(in entityId))
                continue;
            outputEntityIds.Add(entityId);
        }
        return outputEntityIds.Count > 0;
    }

    public abstract void OutputExecutes(in List<int> inputEntityIds, ref AiDecisionGeneratorTestOutputs outputs);

    private bool IsPassEntity(in int entityId)
    {
        for (var j = 0; j < _AiExecuteConditions.Length; j++)
        {
            ref readonly var info = ref _AiExecuteConditions[j];
            if (!info.IsPass(in entityId))
                return false;
        }
        return true;
    }
}
