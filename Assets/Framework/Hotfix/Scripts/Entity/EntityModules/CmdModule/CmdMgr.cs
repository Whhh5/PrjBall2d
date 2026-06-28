using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public enum EnCmdType
{
    None = 0,
    Skill = 1,
}

public interface IEntityCmdExecute
{
    public void AddCmd(in int entityId, in EnEntityCmd cmd);
    public bool CmdIsEnd(in int entityId, in EnEntityCmd cmd);
    public bool ContainsCmd(in int entityId, in EnEntityCmd cmd);
    public void CancelCmd(in int entityId, in EnEntityCmd cmd);
    public void RemoveCmd(in int entityId, in EnEntityCmd cmd);
    public bool IsAddCmd(in int entityId, in EnEntityCmd cmd);
}

public class AnimCmdExecute : IEntityCmdExecute
{
    public void AddCmd(in int entityId, in EnEntityCmd cmd)
    {
        var cmdCfg = GameSchedule.Instance.GetCmdCfg0((int)cmd);
        var animCom =
            SceneEntityMgr.Instance.GetEntityCom<EntityAnimComData>(in entityId, EnSceneEntityComType.Animation);
        var skillId = cmdCfg.arrParams[0];
        animCom.AddSkill(in skillId);
    }

    public bool CmdIsEnd(in int entityId, in EnEntityCmd cmd)
    {
        var cmdCfg = GameSchedule.Instance.GetCmdCfg0((int)cmd);
        var animCom =
            SceneEntityMgr.Instance.GetEntityCom<EntityAnimComData>(in entityId, EnSceneEntityComType.Animation);
        var skillId = cmdCfg.arrParams[0];
        var isEnd = animCom.SkillIsEnd(in skillId);
        return isEnd;
    }

    public bool ContainsCmd(in int entityId, in EnEntityCmd cmd)
    {
        var cmdCfg = GameSchedule.Instance.GetCmdCfg0((int)cmd);
        var animCom =
            SceneEntityMgr.Instance.GetEntityCom<EntityAnimComData>(in entityId, EnSceneEntityComType.Animation);
        var skillId = cmdCfg.arrParams[0];
        var contains = animCom.ContainsSkill(in skillId);
        return contains;
    }

    public void CancelCmd(in int entityId, in EnEntityCmd cmd)
    {
        var cmdCfg = GameSchedule.Instance.GetCmdCfg0((int)cmd);
        var animCom =
            SceneEntityMgr.Instance.GetEntityCom<EntityAnimComData>(in entityId, EnSceneEntityComType.Animation);
        var skillId = cmdCfg.arrParams[0];
        animCom.CancelSkill(in skillId);
    }

    public void RemoveCmd(in int entityId, in EnEntityCmd cmd)
    {
        var cmdCfg = GameSchedule.Instance.GetCmdCfg0((int)cmd);
        var animCom =
            SceneEntityMgr.Instance.GetEntityCom<EntityAnimComData>(in entityId, EnSceneEntityComType.Animation);
        var skillId = cmdCfg.arrParams[0];
        animCom.RemoveSkill(in skillId);
    }

    public bool IsAddCmd(in int entityId, in EnEntityCmd cmd)
    {
        var cmdCfg = GameSchedule.Instance.GetCmdCfg0((int)cmd);
        var animCom =
            SceneEntityMgr.Instance.GetEntityCom<EntityAnimComData>(in entityId, EnSceneEntityComType.Animation);
        var skillId = cmdCfg.arrParams[0];
        var isAdd = animCom.IsAddSkill(in skillId);
        return isAdd;
    }
}

public class CmdMgr : Singleton<CmdMgr>
{
    private Dictionary<EnCmdType, IEntityCmdExecute> _CmdExecutes = new();

    public override async UniTask AwakeAsync()
    {
        await base.AwakeAsync();
        _CmdExecutes.Add(EnCmdType.Skill, new AnimCmdExecute());
    }

    public IEntityCmdExecute GetExecute(in EnEntityCmd cmd)
    {
        var cmdCfg = GameSchedule.Instance.GetCmdCfg0((int)cmd);
        var cmdType = (EnCmdType)cmdCfg.nCmdType;
        return _CmdExecutes[cmdType];
    }
}