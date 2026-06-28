using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class AiMgr : Singleton<AiMgr>
{
    public override EnManagerFuncType FuncType => base.FuncType | EnManagerFuncType.Update;


    private readonly Dictionary<int, AiData> _AiDatas = new();
    private readonly Dictionary<int, int> _EntityId2AiId = new();

    public AiData GetAiData(in int aiId)
    {
        if (!_AiDatas.TryGetValue(aiId, out var aiData))
            throw new System.Exception($"AiData not found for aiId: {aiId}");
        return aiData;
    }

    public int GetAiEntityId(in int aiId)
    {
        var aiData = GetAiData(in aiId);
        return aiData.GetEntityId();
    }

    public int AddEntityAi(in int entityId)
    {
        var aiId = ABBUtil.GetTempKey();
        _EntityId2AiId.Add(entityId, aiId);

        var monsterCfg = MonsterUtility.GetMonsterCfg(in entityId);
        var aiDataUserData =  ClassPoolMgr.Instance.Pull<AiDataUserData>();
        aiDataUserData.entityId = entityId;
        aiDataUserData.aiId = aiId;
        aiDataUserData.aiCfgId = monsterCfg.nAIID;
        var aiData = ClassPoolMgr.Instance.Pull<AiData>(aiDataUserData);
        _AiDatas.Add(aiId, aiData);

        SceneEntityMgr.Instance.AddControllerType(in entityId, EnSceneEntityControllerType.Ai);
        return aiId;
    }

    public void RemoveEntityAi(in int entityId)
    {
        if (!_EntityId2AiId.TryGetValue(entityId, out var aiId))
            return;
        if (_AiDatas.TryGetValue(aiId, out var aiData))
        {
            _AiDatas.Remove(aiId);
            ClassPoolMgr.Instance.Push(aiData);
        }

        SceneEntityMgr.Instance.RemoveControllerType(in entityId, EnSceneEntityControllerType.Ai);
        _EntityId2AiId.Remove(entityId);
    }

    public override void Update()
    {
        base.Update();

        var timeInfo = new AiTimeInfo(0, 0);

        foreach (var (_, aiData) in _AiDatas)
        {
            aiData.Update();
        }
    }
}