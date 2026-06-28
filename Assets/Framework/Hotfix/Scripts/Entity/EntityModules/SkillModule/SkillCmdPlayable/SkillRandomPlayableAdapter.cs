using System.Collections.Generic;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.Playables;


public class SkillRandomPlayableAdapter : SkillPlayableAdapter<SkillPlayableAdapterUserData>
{

    public override List<int> SerializeToIntArray()
    {
        var result = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref result, in _SkillStageList);
        return result;
    }

    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out _SkillStageList, userData);
    }
    private SkillStageInfo[] _SkillStageList = null;

    public override EnTypeId GetTypeDefineId() => EnTypeId.SkillRandomPlayableAdapter;


    private int _CurStageIndex = -1;
    private SkillStageInfo CurSkillStageInfo => _SkillStageList[_CurStageIndex];
    private int _EntityId = -1;
    private IPlayableAdapter _CurClipAdapter = null;
    public override void OnPoolDestroy()
    {
        CurSkillStageInfo.OnDisable(_EntityId);
        IPlayableAdapter.Destroy(_CurClipAdapter);
        base.OnPoolDestroy();
        _CurStageIndex 
            = _EntityId
            = -1;
        _SkillStageList = null;
        _CurClipAdapter = null;
    }
    public override void OnPoolInit(SkillPlayableAdapterUserData userData)
    {
        base.OnPoolInit(userData);
        _EntityId = (int)_Graph;
        _CurStageIndex = 0;

        _CurClipAdapter = _Graph.CreateClipPlayableAdapter(CurSkillStageInfo.GetClipID());
        AddConnectRootAdapter(_CurClipAdapter);
    }
    public override EnAnimLayer GetOutputLayer()
    {
        return EnAnimLayer.Base;
    }
    public override float GetUnitTime()
    {
        return _CurClipAdapter.GetUnitTime();
    }
    public override float GetPlayTime()
    {
        return _CurClipAdapter.GetPlayTime();
    }
    public override bool IsLoop()
    {
        return true;
    }
    public override void ExecutePlayable()
    {
        base.ExecutePlayable();
        CurSkillStageInfo.OnEnable(_EntityId);
    }
    public override void RemovePlayable()
    {
        base.RemovePlayable();
        CurSkillStageInfo.OnDisable(_EntityId);
    }
    public override void ReexecutePlayable()
    {
        base.ReexecutePlayable();
    }
    public override bool OnPrepareFrame(Playable playable, FrameData info)
    {
        if (!base.OnPrepareFrame(playable, info))
            return false;
        if (GetPlaySchedule01() >= 1f)
        {
            CurSkillStageInfo.OnDisable(_EntityId);
            DisconnectRootAdapter();
            var index = Random.Range(GlobalConfig.Int0, _SkillStageList.Length);
            _CurStageIndex = index;
            var toAdapter = _Graph.CreateClipPlayableAdapter(CurSkillStageInfo.GetClipID());

            _CurClipAdapter.Complete();
            _CurClipAdapter = _Graph.CreateMixerPlayableAdapter(_CurClipAdapter, toAdapter, 0.2f, MixerComplete);
            ConnectRootAdapter(_CurClipAdapter);

            CurSkillStageInfo.OnEnable(_EntityId);
        }
        return true;
    }


    private void MixerComplete(PlayableMixerAdapter mixer, IPlayableAdapter from, IPlayableAdapter to)
    {
        DisconnectRootAdapter();
        mixer.DisconnectAll();
        IPlayableAdapter.Destroy(from);
        IPlayableAdapter.Destroy(mixer);
        _CurClipAdapter = to;
        ConnectRootAdapter(_CurClipAdapter);
    }

}
