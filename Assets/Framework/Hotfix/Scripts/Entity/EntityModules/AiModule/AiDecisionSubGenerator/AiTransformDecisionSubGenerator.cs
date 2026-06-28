
using System.Collections.Generic;
using UnityEngine;

public class AiTransformDecisionSubGenerator : AiDecisionSubGenerator
{
    public override EnTypeId GetTypeDefineId() => EnTypeId.AiTransformDecisionSubGenerate;
    public override EnAiDecisionSubGeneratorType GetSubDecisionGeneratorType() => EnAiDecisionSubGeneratorType.Transform;
    public override void OutputExecutes(in List<int> inputEntityIds, ref AiDecisionGeneratorTestOutputs outputs)
    {
        var entityId = AiMgr.Instance.GetAiEntityId(in _AiId);
        var targetEntityId = inputEntityIds[0];
        
        var dir = EntityUtility.GetDirection(in entityId, in targetEntityId);

        {
            var dis = EntityUtility.GetA2bDisSqr(in entityId, in targetEntityId);
            var exeInfo = ClassPoolMgr.Instance.Pull<AiDecisionGeneratorTestExecutionInfo>();
            exeInfo.executionType = EnAiExecutionType.Move;
            var exeUserData = ClassPoolMgr.Instance.Pull<AiMoveExecutionUserData>();
            exeUserData.aiId = _AiId;
            exeUserData.dir = dir;
            exeUserData.speed = Mathf.Lerp(0.3f, 1, dis);
            exeInfo.userData = exeUserData;
            outputs.outputs.Add(exeInfo);
        }
        {
            var exeInfo = ClassPoolMgr.Instance.Pull<AiDecisionGeneratorTestExecutionInfo>();
            exeInfo.executionType = EnAiExecutionType.LookAt;
            var exeUserData = ClassPoolMgr.Instance.Pull<AiLookAtExecutionUserData>();
            exeUserData.aiId = _AiId;
            exeUserData.lookAtDir = dir;
            exeInfo.userData = exeUserData;
            outputs.outputs.Add(exeInfo);
        }
    }

}