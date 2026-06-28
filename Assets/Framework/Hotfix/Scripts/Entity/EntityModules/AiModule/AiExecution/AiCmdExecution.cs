

public class AiCmdExecutionUserData : IClassPoolUserData
{
    public EnEntityCmd cmd;
    public int aiId;
    public void OnPoolDestroy()
    {
        cmd = EnEntityCmd.None;
        aiId = -1;
    }
}
public class AiCmdExecution : IAiExecution<AiCmdExecutionUserData>
{
    public EnAiExecutionType GetAiNodeType() => EnAiExecutionType.Cmd;

    private EnEntityCmd _Cmd;
    private int _AiId;
    public void OnPoolDestroy()
    {
        _AiId
            = -1;
        _Cmd = EnEntityCmd.None;
    }
    public void OnPoolInit(AiCmdExecutionUserData userData)
    {
        _AiId = userData.aiId;
        _Cmd = userData.cmd;
    }

    public void ExecuteAiNode()
    {
        var entityId = AiMgr.Instance.GetAiEntityId(_AiId);
        SceneEntityMgr.Instance.AddCmd(in entityId, in _Cmd);
    }
}