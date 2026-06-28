using System.Collections.Generic;


public class AiDefaultDecision : IAiDecision<AiCommonUserData>
{
    public List<int> SerializeToIntArray()
    {
        var result = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref result, in _DecisionInfos);
        return result;
    }

    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _DecisionInfos, userData);
    }


    private AiDecisionGeneratorInfo[] _DecisionInfos = null;

    public EnTypeId GetTypeDefineId() => EnTypeId.AiDefaultDecision;

    public EnAiDecisionId GetDecisionId() => EnAiDecisionId.Default;

    private readonly List<AiDecisionGeneratorTestOutputs> _ExeCmdInfos = new(10);

    private int _AiId;
    public void OnPoolDestroy()
    {
        ISerializeToIntArray.Release(ref _DecisionInfos);
        _AiId = -1;
    }
    public void OnPoolInit(AiCommonUserData userData)
    {
        _AiId = userData.aiId;
    }

    public void Execute(in List<IAiPerception> perceptions)
    {
        for (var i = 0; i < _DecisionInfos.Length; i++)
        {
            var output = ClassPoolMgr.Instance.Pull<AiDecisionGeneratorTestOutputs>();
            var input = ClassPoolMgr.Instance.Pull<AiDecisionGeneratorTestInputs>();
            var exeResult = ExecutePerceptionTest(in i, in perceptions, in input, ref output);
            if (exeResult)
            {
                _ExeCmdInfos.Add(output);
            }
            else
            {
                ClassPoolMgr.Instance.Push(output);
            }
            ClassPoolMgr.Instance.Push(input);
        }

        for (var i = 0; i < _ExeCmdInfos.Count; i++)
        {
            var cmdOutputInfo = _ExeCmdInfos[i];
            for (var j = 0; j < cmdOutputInfo.outputs.Count; j++)
            {
                var cmdInfo = cmdOutputInfo.outputs[j];
                AiUtility.ExecuteAiDecision(cmdInfo.executionType, cmdInfo.userData);
            }
            if (i == 0)
                break;
        }

        foreach (var item in _ExeCmdInfos)
            ClassPoolMgr.Instance.Push(item);
        _ExeCmdInfos.Clear();
    }

    private bool ExecutePerceptionTest(
        in int index,
        in List<IAiPerception> perceptions,
        in AiDecisionGeneratorTestInputs shareTestParams,
        ref AiDecisionGeneratorTestOutputs exeCmdInfos
        )
    {
        var decisionInfo = _DecisionInfos[index];

        ref readonly var generator = ref decisionInfo.GetDecisionGenerator();
        ref readonly var perceptionInfos = ref generator.GetPerceptionInfos();

        var matchPerceptions = new List<IAiPerception>[perceptionInfos.Length];

        for (var k = 0; k < perceptionInfos.Length; k++)
        {
            ref readonly var perceptionInfo = ref perceptionInfos[k];
            var perceptionId = perceptionInfo.GetPerceptionId();
            var indexs = perceptions.FindAll(item => item.GetPerceptionId() == perceptionId);
            matchPerceptions[k] = indexs;
        }

        for (var k = 0; k < perceptionInfos.Length; k++)
        {
            ref readonly var perceptionInfo = ref perceptionInfos[k];
            ref readonly var perceptionList = ref matchPerceptions[k];
            var testResults = ClassPoolMgr.Instance.Pull<AiDecisionGeneratorTestInput>();
            testResults.testPerceptionId = perceptionInfo.GetPerceptionId();
            shareTestParams.inputs.Add(testResults);

            for (var j = 0; j < perceptionList.Count; j++)
            {
                var mathPerception = perceptionList[j];
                var resultEntityIdList = mathPerception.GetResultEntityList();
                if (!perceptionInfo.Test(resultEntityIdList, out var testResult))
                    continue;
                testResults.testResults.Add(testResult);
            }

            if (!perceptionInfo.IsPassable(in testResults))
                return false;
        }
        if (!generator.TryGetExecuteCmd(in shareTestParams, ref exeCmdInfos))
            return false;
        return true;
    }



}