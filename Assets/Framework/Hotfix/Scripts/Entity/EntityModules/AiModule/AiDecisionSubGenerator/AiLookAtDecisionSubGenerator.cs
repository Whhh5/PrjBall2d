

using System.Collections.Generic;

public class AiLookAtDecisionSubGenerator : AiDecisionSubGenerator
{
    public override EnTypeId GetTypeDefineId() => EnTypeId.AiDirectionDecisionSubGenerator;

    public override EnAiDecisionSubGeneratorType GetSubDecisionGeneratorType() => EnAiDecisionSubGeneratorType.LookAt;

    public override void OutputExecutes(in List<int> inputEntityIds, ref AiDecisionGeneratorTestOutputs outputs)
    {
        var targetEntityId = inputEntityIds[0];
        var entityId = AiMgr.Instance.GetAiEntityId(_AiId);
        
        var dirUserData = ClassPoolMgr.Instance.Pull<AiLookAtExecutionUserData>();
        dirUserData.aiId = _AiId;
        dirUserData.lookAtDir = EntityUtility.GetDirection(in entityId, in targetEntityId);

        var exeInfo = ClassPoolMgr.Instance.Pull<AiDecisionGeneratorTestExecutionInfo>();
        exeInfo.userData = dirUserData;
        exeInfo.executionType = EnAiExecutionType.LookAt;
        outputs.outputs.Add(exeInfo);
    }
}