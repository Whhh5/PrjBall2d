
using System.Collections.Generic;

public class AiPerceptionInfos : ISerializeToIntArray<AiCommonUserData>
{
    private IAiPerception[] _Perceptions = null;

    public List<int> SerializeToIntArray()
    {
        var result = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref result, in _Perceptions);
        return result;
    }

    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _Perceptions, userData);
    }

    public EnTypeId GetTypeDefineId() => EnTypeId.AiPerceptionInfos;

    public void OnPoolDestroy()
    {
        ISerializeToIntArray.Release(ref _Perceptions);
    }

    public void OnPoolInit(AiCommonUserData userData)
    {
    }

    public int GetPerceptionCount()
    {
        return _Perceptions.Length;
    }

    public ref readonly IAiPerception GetPerceptionAt(in int index)
    {
        return ref _Perceptions[index];
    }
}
