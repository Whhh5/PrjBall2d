using System.Collections.Generic;

public enum EnAiThanConditionType
{
    None = 0,
    GreaterThan,
    LessThan,
}
public class AiDistanceCondition : IAiCondition
{

    public EnAiConditionType GetConditionType() => EnAiConditionType.Distance;
    public List<int> SerializeToIntArray()
    {
        var result = new List<int>(5);
        ISerializeToIntArray.SerializeToInt(ref result, _ConditionType);
        ISerializeToIntArray.SerializeToInt(ref result, in _Distance);
        return result;
    }
    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _ConditionType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _Distance);
    }

    private EnAiThanConditionType _ConditionType = EnAiThanConditionType.None;
    private float _Distance = -1f;
    public EnTypeId GetTypeDefineId() => EnTypeId.AiDistanceCondition;


    private int _AiId;
    public void OnPoolDestroy()
    {
        ISerializeToIntArray.Release(ref _ConditionType);
        ISerializeToIntArray.Release(ref _Distance);
        _AiId = -1;
    }
    public void OnPoolInit(AiCommonUserData userData)
    {
        _AiId = userData.aiId;
    }

    public bool IsPass(in int inputId)
    {
        var entityId = AiMgr.Instance.GetAiEntityId(in _AiId);
        var disSqr = EntityUtility.GetA2bDisSqr(in entityId, in inputId);
        var result = _ConditionType switch
        {
            EnAiThanConditionType.GreaterThan => disSqr > _Distance * _Distance,
            EnAiThanConditionType.LessThan => disSqr < _Distance * _Distance,
            _ => false,
        };
        return result;
    }


}
