using UnityEngine;

public class AiMoveExecutionUserData : IClassPoolUserData
{
    public int aiId;
    public Vector3 dir;
    public float speed;

    public void OnPoolDestroy()
    {
        speed
            = aiId
            = -1;
        dir = Vector3.zero;
    }
}

public class AiMoveExecution : IAiExecution<AiMoveExecutionUserData>
{
    private int _AiId;
    private Vector3 _dir;
    private float _Speed;

    public EnAiExecutionType GetAiNodeType()
    {
        return EnAiExecutionType.Move;
    }

    public void OnPoolDestroy()
    {
        _Speed
            = _AiId 
            = -1;
        _dir = Vector3.zero;
    }

    public void OnPoolInit(AiMoveExecutionUserData userData)
    {
        _AiId = userData.aiId;
        _dir = userData.dir;
        _Speed = userData.speed;
    }

    public void ExecuteAiNode()
    {
        var entityId = AiMgr.Instance.GetAiEntityId(in _AiId);
        EntityUtility.MoveEntity(in entityId, in _dir, _Speed);
    }
}