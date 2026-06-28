using UnityEngine;

public class AIIdleModule : AIModule
{
    //private EnEntityCmd _IdleCmd = EnEntityCmd.None;
    private float _Time = 0;
    private float _IdleTime = 3f;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
    }

    public override void OnPoolInit(AIModuleUserData userData)
    {
        base.OnPoolInit(userData);
        //var enittyID = GetEntityID();
        //var enittyData = Entity3DMgr.Instance.GetMonsterEntityData(enittyID);
        //var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(enittyData.GetMonsterID());

        //_IdleCmd = EnEntityCmd.Idle;
    }

    public override void PreExecute()
    {
        _Time = ABBUtil.GetGameTimeSeconds();
    }
    public override void Reexecute()
    {
        _Time = ABBUtil.GetGameTimeSeconds();
    }
    public override void Execute()
    {
    }

    public override void Finish()
    {
    }

    public override bool IsBreak()
    {
        return true;
    }

    public override bool IsExecute()
    {
        return true;
    }

    public override bool IsNextModule()
    {
        var isNext = ABBUtil.GetGameTimeSeconds() > _Time + _IdleTime;
        return isNext;
    }



}
