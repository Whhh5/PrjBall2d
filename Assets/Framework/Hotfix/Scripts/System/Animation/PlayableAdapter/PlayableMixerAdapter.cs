
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public delegate void PlayableMixerCompleteAction(PlayableMixerAdapter mixer, IPlayableAdapter from, IPlayableAdapter to);

public class PlayableMixerAdapterUserData : PlayableAdapterUserData
{
    public IPlayableAdapter from;
    public IPlayableAdapter to;
    public float time;
    public PlayableMixerCompleteAction complete;

    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        from = null;
        to = null;
        time = -1;
        complete = null;
    }
}
public class PlayableMixerAdapter : PlayableAdapter<PlayableMixerAdapterUserData>
{
    private AnimationMixerPlayable m_MixerPlayable;
    private IPlayableAdapter _From = null;
    private IPlayableAdapter _To = null;
    private float _Time;
    private float _EndTime;
    private PlayableMixerCompleteAction _CompleteAction = null;
    private bool _IsComplete = true;

    public override void OnPoolDestroy()
    {
        if (!_IsComplete)
            Complete();
        _IsComplete = true;
        if (_From != null && _From.GetIsValid())
            IPlayableAdapter.Destroy(_From);
        if (_To != null && _To.GetIsValid())
            IPlayableAdapter.Destroy(_To);
        base.OnPoolDestroy();
        m_MixerPlayable.Destroy();
        _From = null;
        _To = null;
        _Time = -1;
        _EndTime = 0;
        _CompleteAction = null;
    }
    public override void OnPoolInit(PlayableMixerAdapterUserData userData)
    {
        base.OnPoolInit(userData);

        m_MixerPlayable = AnimationMixerPlayable.Create(_Graph.GetGraph(), GlobalConfig.Int2);
        AddConnectRootAdapter(m_MixerPlayable);


        var mixerTime = Mathf.Min(userData.time, userData.to.GetUnitTime());
        _CompleteAction = userData.complete;
        _From = userData.from;
        _To = userData.to;
        _Time = mixerTime;
        _IsComplete = false;
        var curTimeSec = ABBUtil.GetGameTimeSeconds();
        _EndTime = curTimeSec + mixerTime;

        m_MixerPlayable.ConnectInput(GlobalConfig.Int0, userData.to.GetPlayable(), GlobalConfig.Int0, GlobalConfig.Int0);
        m_MixerPlayable.ConnectInput(GlobalConfig.Int1, userData.from.GetPlayable(), GlobalConfig.Int0, GlobalConfig.Int1);
    }
    private void MixerComplete()
    {
        _CompleteAction.Invoke(this, _From, _To);
    }
    public override void Complete()
    {
        base.Complete();
        _IsComplete = true;
    }
    public void DisconnectAll()
    {
        _From = null;
        _To = null;
        for (int i = 0; i < m_MixerPlayable.GetInputCount(); i++)
        {
            m_MixerPlayable.DisconnectInput(i);
        }
    }
    public override IPlayableAdapter GetMainPlayableAdapter()
    {
        return _To.GetMainPlayableAdapter();
    }

    public override float GetUnitTime()
    {
        return _To.GetUnitTime();
    }
    public override float GetPlayTime()
    {
        return _To.GetPlayTime();
    }

    public override bool OnPrepareFrame(Playable playable, FrameData info)
    {
        if (!base.OnPrepareFrame(playable, info))
            return false;
        if (_IsComplete)
            return false;
        var curTime = ABBUtil.GetGameTimeSeconds();
        if (curTime > _EndTime)
        {
            Complete();
            MixerComplete();
            return true;
        }
        var residue = _EndTime - curTime;
        var slider = Mathf.Clamp01(1 - (float)residue / _Time);
        m_MixerPlayable.SetInputWeight(GlobalConfig.Int1, 1 - slider);
        m_MixerPlayable.SetInputWeight(GlobalConfig.Int0, slider);
        return true;
    }

    public override EnAnimLayer GetOutputLayer()
    {
        return _To.GetOutputLayer();
    }
    public override int GetSkillId()
    {
        //return base.GetEntityCmd();
        return _To.GetSkillId();
    }
}
