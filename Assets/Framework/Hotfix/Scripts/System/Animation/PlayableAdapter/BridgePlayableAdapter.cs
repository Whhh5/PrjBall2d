using System;
using UnityEngine.Playables;

public sealed class BridgePlayableAdapter : PlayableBehaviour // BridgePlayableAdapter
{
    private IPlayableAdapter m_PlayableAdapter = null;
    private bool m_IsInited = false;
    public void Initialization(IPlayableAdapter playableAdapter)
    {
        m_IsInited = true;
        m_PlayableAdapter = playableAdapter;
        m_PlayableAdapter.OnPlayableCreate();
    }
    public void OnDestroy()
    {
        m_IsInited = false;
        m_PlayableAdapter = null;
    }

    public override void OnGraphStart(Playable playable)
    {
        base.OnGraphStart(playable);
        if (m_IsInited)
            m_PlayableAdapter.OnGraphStart(playable);
    }

    public override void OnGraphStop(Playable playable)
    {
        base.OnGraphStop(playable);
        if (m_IsInited)
            m_PlayableAdapter.OnGraphStop(playable);
    }

    public override void OnPlayableCreate(Playable playable)
    {
        base.OnPlayableCreate(playable);
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        base.OnPlayableDestroy(playable);
        if (m_IsInited)
            m_PlayableAdapter.OnPlayableDestroy(playable);
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        base.OnBehaviourPlay(playable, info);
        if (m_IsInited)
            m_PlayableAdapter.OnBehaviourPlay(playable, info);
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        base.OnBehaviourPause(playable, info);
        if (m_IsInited)
            m_PlayableAdapter.OnBehaviourPause(playable, info);
    }

    public override void PrepareData(Playable playable, FrameData info)
    {
        base.PrepareData(playable, info);
        if (m_IsInited)
            m_PlayableAdapter.PrepareData(playable, info);
    }

    public override void PrepareFrame(Playable playable, FrameData info)
    {
        base.PrepareFrame(playable, info);
        if (m_IsInited)
            m_PlayableAdapter.OnPrepareFrame(playable, info);
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        base.ProcessFrame(playable, info, playerData);
        if (m_IsInited)
            m_PlayableAdapter.ProcessFrame(playable, info, playerData);
    }
}
