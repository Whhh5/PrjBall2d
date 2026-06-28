

using System.Collections.Generic;
using UnityEngine;

public class AiCmdDecisionGenerator : IAiDecisionGenerator<AiCommonUserData>
{
    public EnAiDecisionGeneratorType GetDecisionGeneratorType() => EnAiDecisionGeneratorType.Cmd;
    public List<int> SerializeToIntArray()
    {
        var result = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref result, _CmdType);
        ISerializeToIntArray.SerializeToInt(ref result, in _SubDecisionGenerators);
        ISerializeToIntArray.SerializeToInt(ref result, in _AiPerceptionMatchInfos);
        return result;
    }

    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _CmdType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _SubDecisionGenerators, userData);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _AiPerceptionMatchInfos, userData);
    }
    private EnEntityCmd _CmdType = EnEntityCmd.None;
    private IAiDecisionSubGenerator[] _SubDecisionGenerators = null;
    private IAiPerceptionMatchInfo[] _AiPerceptionMatchInfos = null;
    public EnTypeId GetTypeDefineId() => EnTypeId.AiComAtkDecisionGenerator;

    private int _AiId;
    public void OnPoolDestroy()
    {
        ISerializeToIntArray.Release(ref _AiPerceptionMatchInfos);
        ISerializeToIntArray.Release(ref _CmdType);
        ISerializeToIntArray.Release(ref _SubDecisionGenerators);
        _AiId = -1;
    }
    public void OnPoolInit(AiCommonUserData userData)
    {
        _AiId = userData.aiId;
    }
    public ref readonly IAiPerceptionMatchInfo[] GetPerceptionInfos()
    {
        return ref _AiPerceptionMatchInfos;
    }

    public bool TryGetExecuteCmd(in AiDecisionGeneratorTestInputs inputs, ref AiDecisionGeneratorTestOutputs outputs)
    {
        var perInfos = inputs.inputs[0].testResults[0] as AiPerceptionTestResult;

        var sourceList = new List<int>(perInfos.entityIds);
        for (var i = 0; i < _SubDecisionGenerators.Length; i++)
        {
            ref readonly var subCmdGen = ref _SubDecisionGenerators[i];
            var outputEntityIds = new List<int>(sourceList.Count);
            if (subCmdGen.TryExecute(in sourceList, ref outputEntityIds))
            {
                sourceList = outputEntityIds;
                continue;
            }
            subCmdGen.OutputExecutes(sourceList, ref outputs);
            return true;
        }

        {
            var exeInfo2 = ClassPoolMgr.Instance.Pull<AiDecisionGeneratorTestExecutionInfo>();
            exeInfo2.executionType = EnAiExecutionType.Cmd;
            var exeUserData2 = ClassPoolMgr.Instance.Pull<AiCmdExecutionUserData>();
            exeUserData2.cmd = _CmdType;
            exeUserData2.aiId = _AiId;
            exeInfo2.userData = exeUserData2;
            outputs.outputs.Add(exeInfo2);
        }

        return true;
    }

}