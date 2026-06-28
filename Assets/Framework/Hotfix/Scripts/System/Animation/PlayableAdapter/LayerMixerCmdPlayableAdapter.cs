using System.Collections.Generic;
using UnityEngine;

public class LayerMixerCmdPlayableAdapter : SkillPlayableAdapter<SkillPlayableAdapterUserData>
{
    private IPlayableAdapter _CurPlayableAdater = null;


    public override EnTypeId GetTypeDefineId() => EnTypeId.LayerMixerCmdPlayableAdapter;

    public override List<int> SerializeToIntArray()
    {
        return null;
    }

    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        
    }

    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
    }
    public override void OnPoolInit(SkillPlayableAdapterUserData userData)
    {
        base.OnPoolInit(userData);

        _CurPlayableAdater = _Graph.CreateClipPlayableAdapter(81);
        AddConnectRootAdapter(_CurPlayableAdater);
    }
    public override EnAnimLayer GetOutputLayer()
    {
        return EnAnimLayer.Bottom;
    }
    public override float GetPlayTime()
    {
        return _CurPlayableAdater.GetPlayTime();
    }
    public override float GetUnitTime()
    {
        return _CurPlayableAdater.GetUnitTime();
    }
}
