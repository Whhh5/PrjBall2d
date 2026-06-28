using System.Collections.Generic;

public class SkillSingletonPlayableAdapter : SkillPlayableAdapter<SkillPlayableAdapterUserData>
{
    public override List<int> SerializeToIntArray()
    {
        var result = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref result, in _AnimationClipId);
        return result;
    }
    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _AnimationClipId);
    }
    private int _AnimationClipId = 0;
    public override EnTypeId GetTypeDefineId() => EnTypeId.SkillTypeSingletonPlayableAdapter;

    private PlayableClipAdapter _CurClipAdapter = null;

    public override void OnPoolDestroy()
    {
        IPlayableAdapter.Destroy(_CurClipAdapter);
        base.OnPoolDestroy();
        _AnimationClipId = -1;
        _CurClipAdapter = null;
    }

    public override void OnPoolInit(SkillPlayableAdapterUserData userData)
    {
        base.OnPoolInit(userData);

        _CurClipAdapter = _Graph.CreateClipPlayableAdapter(_AnimationClipId);
        AddConnectRootAdapter(_CurClipAdapter);
    }
    public override float GetUnitTime()
    {
        return _CurClipAdapter.GetUnitTime();
    }
    public override float GetPlayTime()
    {
        return _CurClipAdapter.GetPlayTime();
    }

    public override EnAnimLayer GetOutputLayer()
    {
        return _CurClipAdapter.GetOutputLayer();
    }
}
