using System.Collections.Generic;
using UnityEngine;

public class AiPerceptionTestResult : IAiPerceptionTestResult
{
    public readonly List<int> entityIds = new(10);

    public void OnPoolDestroy()
    {
        entityIds.Clear();
    }
}

public class AiRandomEntityPerception : IAiPerception<AiCommonUserData>
{
    public List<int> SerializeToIntArray()
    {
        var result = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref result, _Layer);
        ISerializeToIntArray.SerializeToInt(ref result, in _Radius);
        return result;
    }
    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _Layer);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _Radius);
    }
    private EnGameLayerInt _Layer = EnGameLayerInt.None;
    private float _Radius = -1f;
    public EnTypeId GetTypeDefineId() => EnTypeId.AiRandomEntityPerception;

    public EnAiPerceptionId GetPerceptionId() => EnAiPerceptionId.RandomEntity;

    private readonly List<int> _EntityList = new(10);
    private int _AiId = -1;

    public void OnPoolDestroy()
    {
        ISerializeToIntArray.Release(ref _Layer);
        ISerializeToIntArray.Release(ref _Radius);
        _AiId
            = -1;
        _EntityList.Clear();
    }

    public void OnPoolInit(AiCommonUserData userData)
    {
        _AiId = userData.aiId;
    }

    public bool UpdateAiPerception()
    {
        _EntityList.Clear();
        var entityId = AiMgr.Instance.GetAiEntityId(in _AiId);
        var pos = EntityMgr.Instance.GetEntityWorldPos(in entityId);
        ref readonly var count = ref SkillPhysicsMgr.Instance.SphereCast(in pos, in _Radius, in _Layer);
        if (count == 0)
            return false;
        for (var i = 0; i < count; i++)
        {
            ref readonly var castEntityId = ref SkillPhysicsMgr.Instance.GetLastCastEntityId(i);
            _EntityList.Add(castEntityId);
        }
        return true;
    }

    public ref readonly List<int> GetResultEntityList()
    {
        return ref _EntityList;
    }
}