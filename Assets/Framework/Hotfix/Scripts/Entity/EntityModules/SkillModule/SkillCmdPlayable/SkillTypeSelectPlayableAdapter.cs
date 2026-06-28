using System.Collections.Generic;
using UnityEngine;

public class SkillTypeSelectPlayableAdapter : SkillPlayableAdapter<SkillPlayableAdapterUserData>
{
    public override List<int> SerializeToIntArray()
    {
        var result = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref result, in _SelectItem);

        return result;
    }

    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _SelectItem, in userData);
    }
    private SkillTypeSelectData _SelectItem = null;
    public override EnTypeId GetTypeDefineId() => EnTypeId.SkillTypeSelectPlayableAdapter;


    private PlayableClipAdapter _Clipadapter = null;
    private SkillStageInfo _atkLinkStageData = null;

    public override void OnPoolDestroy()
    {
        IPlayableAdapter.Destroy(_Clipadapter);
        ClassPoolMgr.Instance.Push(_SelectItem);
        base.OnPoolDestroy();
        _Clipadapter = null;
        _SelectItem = null;
        _atkLinkStageData = null;
    }

    public override EnAnimLayer GetOutputLayer()
    {
        return _Clipadapter.GetOutputLayer();
    }
    public override void OnPoolInit(SkillPlayableAdapterUserData userData)
    {
        base.OnPoolInit(userData);

        var velocity = 0; // Entity3DMgr.Instance.GetEntityVerticalVelocity(_Graph);

        var value = Mathf.RoundToInt(velocity * 100);
        _atkLinkStageData = _SelectItem.CompareResult(value);

        var cipID = _atkLinkStageData.GetClipID();
        _Clipadapter = _Graph.CreateClipPlayableAdapter(cipID);
        AddConnectRootAdapter(_Clipadapter);
    }
    public override bool IsPlayEnd()
    {
        if (!_atkLinkStageData.GetIsAutoRemove())
            return false;
        return base.IsPlayEnd();
    }
    public override void ExecutePlayable()
    {
        base.ExecutePlayable();
        _atkLinkStageData.OnEnable(_Graph);
    }
    public override void RemovePlayable()
    {
        _atkLinkStageData.OnDisable(_Graph);
        base.RemovePlayable();
    }
    public override void ReexecutePlayable()
    {
        base.ReexecutePlayable();
    }
    public override bool NextAnimLevelComdition()
    {
        var curSchedule = GetPlaySchedule01();
        if (!_atkLinkStageData.IsCanNextAction(curSchedule))
            return false;
        return true;
    }

    public override float GetUnitTime()
    {
        return _Clipadapter.GetUnitTime();
    }
    public override float GetPlayTime()
    {
        return _Clipadapter.GetPlayTime();
    }
}
