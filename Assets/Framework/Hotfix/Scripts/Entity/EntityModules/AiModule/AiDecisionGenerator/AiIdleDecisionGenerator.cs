
using System;
using System.Collections.Generic;
using UnityEngine;


public class AiIdleDecisionGenerator : IAiDecisionGenerator<AiCommonUserData>
{
    public EnAiDecisionGeneratorType GetDecisionGeneratorType() => EnAiDecisionGeneratorType.Idle;
    public EnTypeId GetTypeDefineId() => EnTypeId.AiIdleDecisionGenerator;

    public List<int> SerializeToIntArray()
    {
        return null;
    }

    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {

    }

    private readonly IAiPerceptionMatchInfo[] _AiDecisionGeneratorPerceptionInfos = Array.Empty<IAiPerceptionMatchInfo>();
    public ref readonly IAiPerceptionMatchInfo[] GetPerceptionInfos()
    {
        return ref _AiDecisionGeneratorPerceptionInfos;
    }

    private int _AiId;
    public void OnPoolDestroy()
    {
        _AiId = -1;
    }

    public void OnPoolInit(AiCommonUserData userData)
    {
        _AiId = userData.aiId;
    }

    public bool TryGetExecuteCmd(in AiDecisionGeneratorTestInputs inputs, ref AiDecisionGeneratorTestOutputs outputs)
    {
        var curTime = ABBUtil.GetGameTimeSeconds();
        var round = 5f;
        if (Mathf.RoundToInt(curTime / round) % 2 == 0)
            return true;

        var entityId = AiMgr.Instance.GetAiEntityId(in _AiId);
        var entityDir = new Vector3(Mathf.Sin(
                curTime % round / round * Mathf.PI * 2),
                0,
                Mathf.Cos(curTime % round / round * Mathf.PI * 2 * 2));

        var entityForward = EntityMgr.Instance.GetEntityForword(in entityId);
        var dir = EntityUtility.CalculateMoveDir(in entityId, entityDir, entityForward);
        {
            var userData = ClassPoolMgr.Instance.Pull<AiMoveExecutionUserData>();
            userData.aiId = _AiId;
            userData.dir = dir;
            userData.speed = 0.3f;

            var exeInfo = ClassPoolMgr.Instance.Pull<AiDecisionGeneratorTestExecutionInfo>();
            exeInfo.executionType = EnAiExecutionType.Move;
            exeInfo.userData = userData;

            outputs.outputs.Add(exeInfo);
        }
        {
            var userData2 = ClassPoolMgr.Instance.Pull<AiLookAtExecutionUserData>();
            userData2.aiId = _AiId;
            userData2.lookAtDir = dir;

            var exeInfo = ClassPoolMgr.Instance.Pull<AiDecisionGeneratorTestExecutionInfo>();
            exeInfo.executionType = EnAiExecutionType.LookAt;
            exeInfo.userData = userData2;

            outputs.outputs.Add(exeInfo);
        }
        return true;
    }

}