using System.Collections.Generic;

public class AiPerceptionLayerTest : IAiPerceptionTest
{
    public List<int> SerializeToIntArray()
    {
        var result = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref result, _Layer);
        return result;
    }
    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _Layer);
    }
    private EnGameLayerInt _Layer = EnGameLayerInt.None;
    public EnTypeId GetTypeDefineId() => EnTypeId.AiPerceptionLayerTest;
    public EnAiPerceptionTestType GetAiPerceptionTestType() => EnAiPerceptionTestType.Layer;

    public void OnPoolDestroy()
    {
        ISerializeToIntArray.Release(ref _Layer);
    }

    public void OnPoolInit(AiCommonUserData userData)
    {
    }

    public bool IsPassable(in int testEntityId)
    {
        ref readonly var layer = ref SceneEntityMgr.Instance.GetEntityLayer(in testEntityId);
        var result = layer.HasFlag(_Layer);
        return result;
    }
}
