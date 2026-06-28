using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTypeLoopData : ISkillTypeData<SkillPlayableAdapterUserData>, ISerializeToIntArray
{

    public List<int> SerializeToIntArray()
    {
        var result = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref result, in _DataList);
        ISerializeToIntArray.SerializeToInt(ref result, in _BuffInfoList);
        return result;
    }

    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _DataList, userData);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _BuffInfoList, userData);
    }


    protected SkillStageInfo[] _DataList = null;
    protected IEntityBuffParams[] _BuffInfoList = null;
    public EnTypeId GetTypeDefineId() => EnTypeId.SkillTypeLoopData;
    
    private List<int> _BuffAddKeyList = new();

    //
    private EnCmdStep _StepIndex = EnCmdStep.None;
    private float _LastExecuteTime = float.NaN;
    private SkillStageInfo CurAtkLinkStageData => _DataList[(int)_StepIndex];
    private IPlayableAdapter _MainAdapter = null;
    private PlayableGraphAdapter _Graph = null;
    private int _EntityID = -1;
    private IPlayableAdapter m_PlayableAdapter = null;
    private int _LoopCount = GlobalConfig.IntM1;

    public void OnPoolDestroy()
    {
        IPlayableAdapter.Destroy(m_PlayableAdapter);
        foreach (var item in _DataList)
            ClassPoolMgr.Instance.Push(item);

        foreach (var item in _BuffInfoList)
            ClassPoolMgr.Instance.Push(item);


        _DataList = null;
        _BuffInfoList = null;

        _BuffAddKeyList.Clear();
        _Graph = null;
        _MainAdapter = null;
        m_PlayableAdapter = null;

        _LoopCount
            = _EntityID
            = GlobalConfig.IntM1;
        _StepIndex = EnCmdStep.None;
        _LastExecuteTime = float.NaN;
    }



    public void OnPoolInit(SkillPlayableAdapterUserData userData)
    {
        DeserializeFromIntArray(userData.arrParams, userData.startIndex, userData.count, userData);
    }
    public void InitRuntime(IPlayableAdapter mainAdapter)
    {
        _MainAdapter = mainAdapter;
        _Graph = _MainAdapter.GetGraph();
        _EntityID = _Graph;


        _StepIndex = EnCmdStep.Step0;
        _LastExecuteTime = ABBUtil.GetGameTimeSeconds();

        m_PlayableAdapter = _Graph.CreateClipPlayableAdapter(CurAtkLinkStageData.GetClipID());
        _MainAdapter.AddConnectRootAdapter(m_PlayableAdapter);
    }
    public void OnEnable(int entityID)
    {
        foreach (var buffDataParams in _BuffInfoList)
        {
            var addKey = BuffMgr.Instance.AddEntityBuff(entityID, entityID, buffDataParams);
            if (BuffMgr.Instance.GetBuffType(addKey) != EnBuffType.Time)
                _BuffAddKeyList.Add(addKey);
        }
        CurAtkLinkStageData.OnEnable(_EntityID);
    }
    public void OnDisable(int entityID)
    {
        CurAtkLinkStageData.OnDisable(_EntityID);
        foreach (var addKey in _BuffAddKeyList)
        {
            BuffMgr.Instance.RemoveEntityBuff(addKey);
        }
        _BuffAddKeyList.Clear();
    }
    private int GetCount()
    {
        return _DataList.Length;
    }
    public EnAnimLayer GetOutputLayer()
    {
        return m_PlayableAdapter.GetOutputLayer();
    }
    public bool NextAnimLevelComdition()
    {
        return _StepIndex != EnCmdStep.Step1 && _MainAdapter.GetPlaySchedule01() >= CurAtkLinkStageData.GetCanNextTime();
    }
    public float GetPlayTime()
    {
        return m_PlayableAdapter.GetPlayTime();
    }
    public float GetUnitTime()
    {
        return m_PlayableAdapter.GetUnitTime();
    }
    public int GetPlayCount()
    {
        var time = m_PlayableAdapter.GetPlayTime() / m_PlayableAdapter.GetUnitTime();
        return Mathf.FloorToInt(time);
    }



    public bool CurIsPlayEnd()
    {
        if (!CurAtkLinkStageData.GetIsAutoRemove())
            return false;
        if (_StepIndex == EnCmdStep.Step1)
            return false;
        return true;
    }
    public void Reexcute()
    {
        _LastExecuteTime = ABBUtil.GetGameTimeSeconds();
        if (_StepIndex + GlobalConfig.Int1 >= (EnCmdStep)GetCount())
            return;
        if (_StepIndex == EnCmdStep.Step1)
            return;
        if (_MainAdapter.GetPlaySchedule01() < GlobalConfig.Float095)
            return;
        CurAtkLinkStageData.OnDisable(_Graph);
        _StepIndex++;
        CurAtkLinkStageData.OnEnable(_Graph);

        var from = m_PlayableAdapter;
        var to = _Graph.CreateClipPlayableAdapter(CurAtkLinkStageData.GetClipID());
        from.Complete();
        _MainAdapter.DisconnectRootAdapter();
        m_PlayableAdapter = _Graph.CreateMixerPlayableAdapter(from, to, GlobalConfig.Float02, MixerComplete);
        _MainAdapter.ConnectRootAdapter(m_PlayableAdapter);
    }
    public void CmdEnd()
    {
        CurAtkLinkStageData.OnDisable(_EntityID);
        _StepIndex++;
        CurAtkLinkStageData.OnEnable(_EntityID);
        var from = m_PlayableAdapter;
        var to = _Graph.CreateClipPlayableAdapter(CurAtkLinkStageData.GetClipID());
        from.Complete();
        _MainAdapter.DisconnectRootAdapter();
        m_PlayableAdapter = _Graph.CreateMixerPlayableAdapter(from, to, GlobalConfig.Float02, MixerComplete);
        _MainAdapter.ConnectRootAdapter(m_PlayableAdapter);
    }
    private void MixerComplete(PlayableMixerAdapter mixer, IPlayableAdapter from, IPlayableAdapter to)
    {
        mixer.DisconnectAll();
        _MainAdapter.DisconnectRootAdapter();
        IPlayableAdapter.Destroy(from);
        IPlayableAdapter.Destroy(mixer);
        _MainAdapter.ConnectRootAdapter(to);
        m_PlayableAdapter = to;
    }
    public void Update()
    {
        if (_StepIndex == EnCmdStep.Step1
            && ABBUtil.GetGameTimeSeconds() - _LastExecuteTime > GlobalConfig.Float02)
        {
            if (_LoopCount != GetPlayCount())
            {
                CmdEnd();
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
            var schedule = _MainAdapter.GetPlaySchedule() % GetUnitTime() / GetUnitTime();
            if (schedule > curAttackItem.schedule)
            {
                curAttackItem.onEvent.Invoke(_EntityID, curAttackItem.userData);
                CurAtkLinkStageData.NextEventAction();
            }
        }
    }

}
