

public class EntityCmdComData : Entity3DComData
{
    public override EnSceneEntityComType GetComType() => EnSceneEntityComType.Cmd;


    
    public void AddCmd(in EnEntityCmd cmd)
    {
        var exe = CmdMgr.Instance.GetExecute(in cmd);
        exe.AddCmd(in _EntityId, in cmd);
    }

    public void RemoveCmd(in EnEntityCmd cmd)
    {
        var exe = CmdMgr.Instance.GetExecute(in cmd);
        exe.RemoveCmd(in _EntityId, in cmd);
    }

    public bool ContainsCmd(in EnEntityCmd cmd)
    {
        var exe = CmdMgr.Instance.GetExecute(in cmd);
        var contains = exe.ContainsCmd(in _EntityId, in cmd);
        return contains;
    }
    public bool CmdIsEnd(in EnEntityCmd cmd)
    {
        var exe = CmdMgr.Instance.GetExecute(in cmd);
        var isEnd = exe.CmdIsEnd(in _EntityId, in cmd);
        return isEnd;
    }
    public void CancelCmd(in EnEntityCmd cmd)
    {
        var exe = CmdMgr.Instance.GetExecute(in cmd);
        exe.CancelCmd(in _EntityId, in cmd);
    }

    public bool IsAddCmd(in EnEntityCmd cmd)
    {
        var exe = CmdMgr.Instance.GetExecute(in cmd);
        var isAdd = exe.IsAddCmd(in _EntityId, in cmd);
        return isAdd;
    }
}