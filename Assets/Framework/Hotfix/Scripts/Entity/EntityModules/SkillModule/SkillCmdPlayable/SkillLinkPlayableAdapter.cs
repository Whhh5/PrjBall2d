using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


public class SkillLinkPlayableAdapter : SkillPlayableAdapter<SkillPlayableAdapterUserData>
{
    public override List<int> SerializeToIntArray()
    {
        var result = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref result, in _DataList);
        ISerializeToIntArray.SerializeToInt(ref result, in _BuffInfoList);
        return result;
    }

    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _DataList, in userData);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _BuffInfoList, in userData);
    }
    private SkillStageInfo[] _DataList = null;
    private IEntityBuffParams[] _BuffInfoList = null;

    public override EnTypeId GetTypeDefineId() => EnTypeId.SkillLinkPlayableAdapter;

    private readonly List<int> _BuffAddKeyList = new();
    private int _Index = 0;
    private SkillStageInfo curAttackData => _DataList[_Index];

    private IPlayableAdapter _CurClipAdapter = null;
    private int _PortID = -1;

    private int _EntityID = -1;

    public override void OnPoolDestroy()
    {
        foreach (var item in _DataList)
            ClassPoolMgr.Instance.Push(item);
        foreach (var item in _BuffInfoList)
            ClassPoolMgr.Instance.Push(item);

        IPlayableAdapter.Destroy(_CurClipAdapter);

        base.OnPoolDestroy();

        _Index = 0;
        _BuffInfoList = null;
        _DataList = null;
        _BuffAddKeyList.Clear();
        _CurClipAdapter = null;
        _PortID
            = _EntityID
            = -1;
    }
    public override void OnPoolInit(SkillPlayableAdapterUserData userData)
    {
        base.OnPoolInit(userData);

        _EntityID = (int)_Graph;
        _CurClipAdapter = _Graph.CreateClipPlayableAdapter(curAttackData.GetClipID());
        _PortID = AddConnectRootAdapter(_CurClipAdapter, GlobalConfig.Int0, GlobalConfig.Float1);
    }
    private int GetCount()
    {
        return _DataList.Length;
    }
    public override bool IsPlayEnd()
    {
        if (!curAttackData.GetIsAutoRemove())
            return false;
        return base.IsPlayEnd();
    }
    public override float GetPlayTime()
    {
        return _CurClipAdapter.GetPlayTime();
    }
    public override float GetUnitTime()
    {
        return _CurClipAdapter.GetUnitTime();
    }
    public override EnAnimLayer GetOutputLayer()
    {
        return _CurClipAdapter.GetOutputLayer();
    }
    public override bool NextAnimLevelComdition()
    {
        return GetPlaySchedule01() > curAttackData.GetCanNextTime();
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
        curAttackData.OnEnable(_EntityID);
    }
    public override void RemovePlayable()
    {
        curAttackData.OnDisable(_EntityID);
        foreach (var addKey in _BuffAddKeyList)
        {
            BuffMgr.Instance.RemoveEntityBuff(addKey);
        }
        _BuffAddKeyList.Clear();

        base.RemovePlayable();
    }
    public override void ReexecutePlayable()
    {
        base.ReexecutePlayable();

        var slider = GetPlaySlider();
        if (slider < curAttackData.GetAtkEndTime())
            return;

        var targetIndex = (_Index + 1) % GetCount();
        SetAttackIndex(targetIndex);
    }

    public override bool OnPrepareFrame(Playable playable, FrameData info)
    {
        if (!base.OnPrepareFrame(playable, info))
            return false;

        if (curAttackData.ScheduleEventIsValid())
        {
            var curAttackItem = curAttackData.GetCurScheduleItem();
            if (GetPlaySchedule01() >= curAttackItem.schedule)
            {
                curAttackItem.onEvent(_EntityID, curAttackItem.userData);
                curAttackData.NextEventAction();
            }
        }
        return true;
    }

    private float GetPlaySlider()
    {
        var curTime = _CurClipAdapter.GetPlayTime();
        var maxTime = _CurClipAdapter.GetUnitTime();
        var slider = curTime / maxTime;
        return Mathf.Clamp01(slider);
    }
    private void SetAttackIndex(int index)
    {
        curAttackData.OnDisable(_EntityID);
        _Index = index;
        curAttackData.OnEnable(_EntityID);
        var fromAdapter = _CurClipAdapter;
        var toAdapter = _Graph.CreateClipPlayableAdapter(curAttackData.GetClipID());

        fromAdapter.Complete();
        DisconnectRootAdapter();
        _CurClipAdapter = _Graph.CreateMixerPlayableAdapter(fromAdapter, toAdapter, GlobalConfig.Float02, MixerComplete);

        ConnectRootAdapter(_PortID, _CurClipAdapter, 0, 1);
    }
    private void MixerComplete(PlayableMixerAdapter mixer, IPlayableAdapter frome, IPlayableAdapter to)
    {
        mixer.DisconnectAll();
        DisconnectRootAdapter(_PortID);
        IPlayableAdapter.Destroy(frome);
        ConnectRootAdapter(_PortID, to, GlobalConfig.Int0, GlobalConfig.Int1);
        _CurClipAdapter = to;
    }

}
