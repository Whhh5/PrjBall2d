
using UnityEngine;

public class AiLookAtExecutionUserData : IClassPoolUserData
{
    public int aiId;
    public Vector3 lookAtDir;

    public void OnPoolDestroy()
    {
        aiId
            = -1;
        lookAtDir = Vector3.zero;
    }
}

public class AiLookAtExecution : IAiExecution<AiLookAtExecutionUserData>
{
    private int _AiId;
    private Vector3 _LookAtDir;

    public EnAiExecutionType GetAiNodeType()
    {
        return EnAiExecutionType.LookAt;
    }

    public void OnPoolDestroy()
    {
        _AiId
            = -1;
        _LookAtDir = Vector3.zero;
    }

    public void OnPoolInit(AiLookAtExecutionUserData userData)
    {
        _AiId = userData.aiId;
        _LookAtDir = userData.lookAtDir;
    }

    public void ExecuteAiNode()
    {
        var entityId = AiMgr.Instance.GetAiEntityId(in _AiId);
        EntityUtility.RotationEntity(in entityId, in _LookAtDir);
    }
}