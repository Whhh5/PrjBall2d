public interface IAIModule : IClassPoolInit<AIModuleUserData>
{
    public int GetAIModuleCfgID();
    public int GetModuleDataID();
    public bool IsBreak();
    public void PreExecute();
    public void Reexecute();
    public void Execute();
    // 完成一次行为执行一次
    public void Finish();
    // 是否可以进行下一个 ai 行为
    public bool IsNextModule();
    // 判断是否可以进行当前行为
    public bool IsExecute();
}

public abstract class AIModule : IAIModule
{
    private int _EntityID = -1;
    protected int EntityID => _EntityID;
    private int _AIModuleCfgID = -1;
    private int _ModuleDataID = -1;

    public abstract void Execute();
    public abstract bool IsBreak();
    public abstract bool IsExecute();
    public abstract bool IsNextModule();
    public abstract void Finish();
    public abstract void PreExecute();
    public abstract void Reexecute();


    public int GetAIModuleCfgID()
    {
        return _AIModuleCfgID;
    }

    public int GetModuleDataID()
    {
        return _ModuleDataID;
    }
    public int GetEntityID()
    {
        return _EntityID;
    }

    public virtual void OnPoolDestroy()
    {
    }

    public virtual void OnPoolInit(AIModuleUserData userData)
    {
        _AIModuleCfgID = userData.aiModuleCfgID;
        _ModuleDataID = userData.moduleDataID;
        _EntityID = userData.entityID;
    }

}