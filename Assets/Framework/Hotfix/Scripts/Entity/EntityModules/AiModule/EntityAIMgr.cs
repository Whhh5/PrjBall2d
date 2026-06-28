
using System.Collections.Generic;

public enum EnAIModuleType
{
    None = 0,
    Idle = 2,
}

public class EntityAIMgr : Singleton<EntityAIMgr>
{
    private Dictionary<int, EntityAIInfo> _EntityAIData = new();
    private Dictionary<int, AIModuleInfo> _AiModuleDic = new();

    private List<int> _TempList = new(new int[100]);
    public bool AIModuleIDIsValid(int moduleID)
    {
        var result = _AiModuleDic.ContainsKey(moduleID);
        return result;
    }
    private IAIModule CreateAIModuleData(int moduleCfgID, AIModuleUserData userData)
    {
        return (EnAIModuleType)moduleCfgID switch
        {
            EnAIModuleType.Idle => ClassPoolMgr.Instance.Pull<AIIdleModule>(userData),
            _ => null
        };
    }
    public bool TryGetEntityAIInfo(int entityID, out EntityAIInfo info)
    {
        if (!_EntityAIData.TryGetValue(entityID, out info))
            return false;
        return true;
    }
    private EntityAIInfo AddEntityAIInfo(int entityID)
    {
        var entityAIInfo = ClassPoolMgr.Instance.Pull<EntityAIInfo>();
        _EntityAIData.Add(entityID, entityAIInfo);
        return entityAIInfo;
    }
    private void RemoveEntityAIInfo(int entityID)
    {
        if (!TryGetEntityAIInfo(entityID, out var info))
            return;
        ClassPoolMgr.Instance.Push(info);
        _EntityAIData.Remove(entityID);
    }
    public bool TryGetAIModuleData(int moduleID, out IAIModule moduleData)
    {
        if (!TryGetAIModuleData<IAIModule>(moduleID, out moduleData))
            return false;
        return true;
    }
    private bool TryGetAIModuleData<T>(int moduleID, out T moduleData)
        where T : class, IAIModule
    {
        if (!_AiModuleDic.TryGetValue(moduleID, out var moduleInfo))
        {
            moduleData = default;
            return false;
        }
        moduleData = moduleInfo.aiModule as T;
        return moduleData != null;
    }
    private int CreateAIMoudle(int moduleCfgID, AIModuleUserData userData)
    {
        var moduleDataID = ABBUtil.GetTempKey();
        userData.moduleDataID = moduleDataID;
        userData.aiModuleCfgID = moduleCfgID;
        var aiModuleData = CreateAIModuleData(moduleCfgID, userData);

        var moduleInfo = ClassPoolMgr.Instance.Pull<AIModuleInfo>();
        moduleInfo.aiModule = aiModuleData;
        _AiModuleDic.Add(moduleDataID, moduleInfo);
        return moduleDataID;
    }
    public void DestroyAIModule(int moduleDataID)
    {
        if (!_AiModuleDic.TryGetValue(moduleDataID, out var moduleInfo))
            return;
        for (int i = 0; i < moduleInfo.Count; i++)
        {
            var entityID = moduleInfo.GetEntityIDByIndex(i);
            RemoveEntityAIData(entityID, moduleDataID);
        }
        ClassPoolMgr.Instance.Push(moduleInfo);
        _AiModuleDic.Remove(moduleDataID);
    }

    public int AddEntityAIModule(int entityID, int moduleCfgID)
    {
        var userData = ClassPoolMgr.Instance.Pull<AIModuleUserData>();
        userData.entityID = entityID;
        var aiModuleID = AddEntityAIModule(entityID, moduleCfgID, userData);
        ClassPoolMgr.Instance.Push(userData);
        return aiModuleID;
    }
    public int AddEntityAIModule(int entityID, int moduleCfgID, AIModuleUserData userData)
    {
        userData.entityID = entityID;
        var moduleDataID = CreateAIMoudle(moduleCfgID, userData);
        if (!_AiModuleDic.TryGetValue(moduleDataID, out var moduleInfo))
            return -1;
        moduleInfo.AddEntityID(entityID);

        if (!TryGetEntityAIInfo(entityID, out var entityAIInfo))
            entityAIInfo = AddEntityAIInfo(entityID);

        entityAIInfo.AddAIModuleData(moduleCfgID, moduleDataID);
        return moduleDataID;
    }
    public void RemoveEntityAIModule(int entityID, int moduleDataID)
    {
        if (!_AiModuleDic.TryGetValue(moduleDataID, out var moduleInfo))
            return;
        moduleInfo.RemoveEntityID(entityID);

        RemoveEntityAIData(entityID, moduleDataID);
    }
    public void RemoveEntityAI(int entityID)
    {
        if (!_EntityAIData.TryGetValue(entityID, out var entityAIInfo))
            return;
        var count = entityAIInfo.GetAllAIModuleDataIDs(ref _TempList);
        for (int i = 0; i < count; i++)
        {
            var moduleDataID = _TempList[i];
            RemoveEntityAIModule(entityID, moduleDataID);
        }
        ClassPoolMgr.Instance.Push(entityAIInfo);
    }
    private void RemoveEntityAIData(int entityID, int moduleDataID)
    {
        if (!TryGetAIModuleData(moduleDataID, out var moduleData))
            return;
        if (!TryGetEntityAIInfo(entityID, out var entityAIInfo))
            return;
        var aiModuleCfgID = moduleData.GetAIModuleCfgID();
        entityAIInfo.RemoveAIModuleData(aiModuleCfgID, moduleDataID);
        if (entityAIInfo.Count == 0)
        {
            RemoveEntityAIInfo(entityID);
        }
    }

}
