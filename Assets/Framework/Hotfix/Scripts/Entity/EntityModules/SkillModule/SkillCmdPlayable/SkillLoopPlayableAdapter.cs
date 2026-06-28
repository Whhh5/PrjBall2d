using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


public class SkillLoopPlayableAdapter : SkillPlayableAdapter<SkillPlayableAdapterUserData>
{
    public sealed override List<int> SerializeToIntArray()
    {
        var result = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref result, in _DataList);
        ISerializeToIntArray.SerializeToInt(ref result, in _BuffInfoList);
        return result;
    }

    public sealed override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _DataList, userData);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _BuffInfoList, userData);
    }


    private SkillStageInfo[] _DataList = null;
    private IEntityBuffParams[] _BuffInfoList = null;
    public override EnTypeId GetTypeDefineId() => EnTypeId.SkillLoopPlayableAdapter;


    private List<int> _BuffAddKeyList = new();

    //
    private EnCmdStep _StepIndex = EnCmdStep.None;
    private float _LastExecuteTime = float.NaN;
    private SkillStageInfo CurAtkLinkStageData => _DataList[(int)_StepIndex];
    private int _EntityID = -1;
    private IPlayableAdapter m_PlayableAdapter = null;
    private int _LoopCount = GlobalConfig.IntM1;
    public override void OnPoolDestroy()
    {
        IPlayableAdapter.Destroy(m_PlayableAdapter);
        foreach (var item in _DataList)
            ClassPoolMgr.Instance.Push(item);

        foreach (var item in _BuffInfoList)
            ClassPoolMgr.Instance.Push(item);

        base.OnPoolDestroy();

        _DataList = null;
        _BuffInfoList = null;

        _BuffAddKeyList.Clear();
        _Graph = null;
        m_PlayableAdapter = null;

        _LoopCount
            = _EntityID
            = GlobalConfig.IntM1;
        _StepIndex = EnCmdStep.None;
        _LastExecuteTime = float.NaN;
    }
    public override void OnPoolInit(SkillPlayableAdapterUserData userData)
    {
        base.OnPoolInit(userData);

        _EntityID = (int)_Graph;


        _StepIndex = EnCmdStep.Step0;
        _LastExecuteTime = ABBUtil.GetGameTimeSeconds();

        m_PlayableAdapter = _Graph.CreateClipPlayableAdapter(CurAtkLinkStageData.GetClipID());
        AddConnectRootAdapter(m_PlayableAdapter);
    }
    public override EnAnimLayer GetOutputLayer()
    {
        var layer = m_PlayableAdapter.GetOutputLayer();
        return layer;
    }
    public override void ExecutePlayable()
    {
        base.ExecutePlayable();
        foreach (var buffDataParams in _BuffInfoList)
        {
            var addKey = BuffMgr.Instance.AddEntityBuff(_EntityID, _EntityID, buffDataParams);
            if (BuffMgr.Instance.GetBuffType(addKey) != EnBuffType.Time)
                _BuffAddKeyList.Add(addKey);
        }
        CurAtkLinkStageData.OnEnable(_EntityID);
    }
    public override void RemovePlayable()
    {
        CurAtkLinkStageData.OnDisable(_EntityID);
        foreach (var addKey in _BuffAddKeyList)
        {
            BuffMgr.Instance.RemoveEntityBuff(addKey);
        }
        _BuffAddKeyList.Clear();
        base.RemovePlayable();
    }
    public override bool NextAnimLevelComdition()
    {
        var  result = _StepIndex != EnCmdStep.Step1 && GetPlaySchedule01() >= CurAtkLinkStageData.GetCanNextTime();
        return result;
    }
    public override float GetPlayTime()
    {
        var time = m_PlayableAdapter.GetPlayTime();
        return time;
    }
    public override float GetUnitTime()
    {
        var time = m_PlayableAdapter.GetUnitTime();
        return time;
    }

    public override bool IsPlayEnd()
    {
        if (!CurAtkLinkStageData.GetIsAutoRemove())
            return false;
        if (_StepIndex == EnCmdStep.Step1)
            return false;
        return base.IsPlayEnd();
    }
    public override int GetPlayCount()
    {
        //return base.GetPlayCount();
        var time = m_PlayableAdapter.GetPlayTime() / m_PlayableAdapter.GetUnitTime();
        return Mathf.FloorToInt(time);
    }
    public override void ReexecutePlayable()
    {
        base.ReexecutePlayable();
        _LastExecuteTime = ABBUtil.GetGameTimeSeconds();
        if (_StepIndex + GlobalConfig.Int1 >= (EnCmdStep)GetCount())
            return;
        if (_StepIndex == EnCmdStep.Step1)
            return;
        if (GetPlaySchedule01() < GlobalConfig.Float095)
            return;
        CurAtkLinkStageData.OnDisable(_Graph);
        _StepIndex++;
        CurAtkLinkStageData.OnEnable(_Graph);

        var from = m_PlayableAdapter;
        var to = _Graph.CreateClipPlayableAdapter(CurAtkLinkStageData.GetClipID());
        from.Complete();
        DisconnectRootAdapter();
        m_PlayableAdapter = _Graph.CreateMixerPlayableAdapter(from, to, GlobalConfig.Float02, MixerComplete);
        ConnectRootAdapter(m_PlayableAdapter);
    }
    public override void CancelPlayable()
    {
        base.CancelPlayable();
        CurAtkLinkStageData.OnDisable(_EntityID);
        _StepIndex++;
        CurAtkLinkStageData.OnEnable(_EntityID);
        var from = m_PlayableAdapter;
        var to = _Graph.CreateClipPlayableAdapter(CurAtkLinkStageData.GetClipID());
        from.Complete();
        DisconnectRootAdapter();
        m_PlayableAdapter = _Graph.CreateMixerPlayableAdapter(from, to, GlobalConfig.Float02, MixerComplete);
        ConnectRootAdapter(m_PlayableAdapter);
    }
    private void MixerComplete(PlayableMixerAdapter mixer, IPlayableAdapter from, IPlayableAdapter to)
    {
        mixer.DisconnectAll();
        DisconnectRootAdapter();
        IPlayableAdapter.Destroy(from);
        IPlayableAdapter.Destroy(mixer);
        ConnectRootAdapter(to);
        m_PlayableAdapter = to;
    }

    public override bool OnPrepareFrame(Playable playable, FrameData info)
    {
        if (!base.OnPrepareFrame(playable, info))
            return false;
        if (_StepIndex == EnCmdStep.Step1 && ABBUtil.GetGameTimeSeconds() - _LastExecuteTime > GlobalConfig.Float02)
        {
            if (_LoopCount != GetPlayCount())
            {
                CancelPlayable();
            }
        }
        else
        {
            var curCRound = GetPlayCount();
            if (_LoopCount != curCRound)
            {
                CurAtkLinkStageData.ResetAllItemData();
                _LoopCount = curCRound;
            }
        }


        if (CurAtkLinkStageData.ScheduleEventIsValid())
        {
            var curAttackItem = CurAtkLinkStageData.GetCurScheduleItem();
            var schedule = GetPlaySchedule() % GetUnitTime() / GetUnitTime();
            if (schedule > curAttackItem.schedule)
            {
                curAttackItem.onEvent.Invoke(_EntityID, curAttackItem.userData);
                CurAtkLinkStageData.NextEventAction();
            }
        }
        return true;
    }
    private int GetCount()
    {
        return _DataList.Length;
    }

}
