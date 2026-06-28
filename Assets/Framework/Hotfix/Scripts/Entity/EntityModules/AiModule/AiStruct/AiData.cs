using System.Collections.Generic;

public class AiDataUserData : IClassPoolUserData
{
    public int aiId;
    public int entityId;
    public int aiCfgId;

    public void OnPoolDestroy()
    {
        aiId
            = entityId
            = aiCfgId
            = -1;
    }
}

public class AiData : IClassPoolInit<AiDataUserData>
{
    private int _AiId;
    private int _EntityId;
    private AiPerceptionInfos _PerceptionInfos = null;
    private AiDecisionInfo _Decision = null;

    private readonly List<IAiPerception> _TempPerceptions = new (10);
    public void OnPoolDestroy()
    {
        ClassPoolMgr.Instance.Push(_Decision);
        ClassPoolMgr.Instance.Push(_PerceptionInfos);
        _AiId
            = _EntityId
                = -1;
        _PerceptionInfos = null;
        _Decision = null;
        _TempPerceptions.Clear();
    }

    public void OnPoolInit(AiDataUserData userData)
    {
        _AiId = userData.aiId;
        _EntityId = userData.entityId;

        var aiCfg = GameSchedule.Instance.GetAICfg0(userData.aiCfgId);
        {
            var arrPerception = aiCfg.arrPerception;
            var aiComUserData = ClassPoolMgr.Instance.Pull<AiCommonUserData>();
            aiComUserData.startIndex = 0;
            aiComUserData.aiId = _AiId;
            aiComUserData.count = arrPerception.Length;
            aiComUserData.arrParams = arrPerception;
            _PerceptionInfos = ClassPoolMgr.Instance.Pull<AiPerceptionInfos>(aiComUserData);
        }
        {
            var arrDecision = aiCfg.arrDecision;
            var aiComUserData = ClassPoolMgr.Instance.Pull<AiCommonUserData>();
            aiComUserData.startIndex = 0;
            aiComUserData.aiId = _AiId;
            aiComUserData.count = arrDecision.Length;
            aiComUserData.arrParams = arrDecision;
            _Decision = ClassPoolMgr.Instance.Pull<AiDecisionInfo>(aiComUserData);
        }
    }

    public ref readonly int GetEntityId()
    {
        return ref _EntityId;
    }

    public void Update()
    {
        if (SceneEntityMgr.Instance.IsCanController(in _EntityId, EnSceneEntityControllerType.Ai))
            return;
        var count = _PerceptionInfos.GetPerceptionCount();
        for (var i = 0; i < count; i++)
        {
            ref readonly var perception = ref _PerceptionInfos.GetPerceptionAt(i);
            if (perception.UpdateAiPerception())
            {
                _TempPerceptions.Add(perception);
            }
        }

        _Decision.Execute(in _TempPerceptions);
        _TempPerceptions.Clear();
    }
}