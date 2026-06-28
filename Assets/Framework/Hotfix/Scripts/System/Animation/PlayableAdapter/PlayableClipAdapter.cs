using UnityEngine.Playables;
using UnityEngine.Animations;
using UnityEngine.PlayerLoop;
using UnityEngine;

public class PlayableClipAdapterUserData : PlayableAdapterUserData
{
    public int clipID = -1;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        clipID = -1;
    }
}
public class PlayableClipAdapter : PlayableAdapter<PlayableClipAdapterUserData>
{
    private AnimationClipPlayable _ClipPlayable;
    private int _ClipID = -1;
    private float _ClipLength = -1;
    private EnAnimLayer _AnimLayer = EnAnimLayer.None;
    public override void OnPoolDestroy()
    {
        _ClipPlayable.Destroy();
        AnimMgr.Instance.RecycleClip(_ClipID);
        base.OnPoolDestroy();
        _ClipID = -1;
        _ClipLength = -1;
    }
    public override void OnPoolInit(PlayableClipAdapterUserData userData)
    {
        base.OnPoolInit(userData);

        _ClipID = userData.clipID;
        var clipCfg = GameSchedule.Instance.GetClipCfg0(_ClipID); 
        _AnimLayer = (EnAnimLayer)clipCfg.nLayer;
        var clip = AnimMgr.Instance.GetClip(_ClipID);
        _ClipLength = clip.length;
        _ClipPlayable = AnimationClipPlayable.Create(_Graph.GetGraph(), clip);
        _ClipPlayable.SetApplyFootIK(false);
        _ClipPlayable.SetApplyPlayableIK(false);
        AddConnectRootAdapter(_ClipPlayable, GlobalConfig.Int0, GlobalConfig.Float1);
    }
    public override float GetPlayTime()
    {
        return (float)_ClipPlayable.GetTime();
    }
    public override float GetUnitTime()
    {
        return _ClipLength;
    }

    public override EnAnimLayer GetOutputLayer()
    {
        return _AnimLayer;
    }
}
