using System;
using System.Collections.Generic;
using UnityEngine;


public interface IEntityAnimCom : IEntity3DCom
{
    public Animator GetAnimator();
}
public sealed class EntityAnimComData : Entity3DComDataGO<IEntityAnimCom>, IUpdate
{

    public override EnSceneEntityComType GetComType() => EnSceneEntityComType.Animation;
    private PlayableGraphAdapter _PlayableGraph = null;

    private readonly Dictionary<IPlayableAdapter, LayerMixerConnectInfo> _Adapter2ConnectInfo = new();
    private readonly List<IPlayableAdapter> _NoLoopPlayableList = new();
    private readonly HashSet<int> _CreateAddList = new();
    private readonly List<int> _CurSkillList = new();
    private readonly Dictionary<int, List<ISkillPlayableAdapter>> _SkillAdapterDic = new();

    #region life
    public override void OnEnable()
    {
        base.OnEnable();
        UpdateMgr.Instance.Registener(this);
    }
    public override void OnDisable()
    {
        UpdateMgr.Instance.Unregistener(this);
        base.OnDisable();
    }
    public override void OnDestroyGO()
    {
        _CreateAddList.Clear();
        var listKey = new List<int>(_SkillAdapterDic.Count);
        foreach (var item in _SkillAdapterDic) listKey.Add(item.Key);
        foreach (var item in listKey) RemoveSkill(item);
        PlayableGraphAdapter.OnDestroy(_PlayableGraph);
        base.OnDestroyGO();
        _SkillAdapterDic.Clear();
        _CurSkillList.Clear();
        _PlayableGraph = null;
        _Adapter2ConnectInfo.Clear();
        _NoLoopPlayableList.Clear();
    }
    public override void OnCreateGO()
    {
        base.OnCreateGO();
        var animCom = _GoCom.GetAnimator();
        _PlayableGraph = PlayableGraphAdapter.Create(_EntityId, animCom);
        foreach (var item in _CreateAddList)
            AddSkill(item);
        _CreateAddList.Clear();
    }

    #endregion


    private ISkillPlayableAdapter GetSkillPlayableAdapter(int skillId)
    {
        var skillCfg = GameSchedule.Instance.GetSkillCfg0(skillId);
        var customData = ClassPoolMgr.Instance.Pull<SkillPlayableAdapterUserData>();
        customData.arrParams = skillCfg.arrParams;
        customData.startIndex = 0;
        customData.count = skillCfg.arrParams.Length;

        var result = _PlayableGraph.Create(in skillCfg.nTypeId, customData);
        result.SetSkillId(skillId);
        ClassPoolMgr.Instance.Push(customData);
        return result as ISkillPlayableAdapter;
    }
    #region skill
    public void AddSkill(in int skillId)
    {
        if (!_SkillAdapterDic.TryGetValue(skillId, out var skillAdapters))
        {
            if (!IsAddSkill(skillId))
                return;
            if (!EntityMgr.Instance.GetEntityIsLoadSuccess(_EntityId))
            {
                _CreateAddList.Add(skillId);
                return;
            }
            var skillAdapter = GetSkillPlayableAdapter(skillId);
            AddSkillData(skillAdapter);
            ConnectLayerInput(skillId, skillAdapter, skillAdapter.GetOutputLayer());
            skillAdapter.ExecutePlayable();

            var skillCfg = GameSchedule.Instance.GetSkillCfg0(skillId);
            var applyRootMotion = skillCfg.bApplyRootMotion;
            SetApplyRootMotion(applyRootMotion > 0);
        }
        else
        {
            foreach (var skillAdapter in skillAdapters)
                skillAdapter.ReexecutePlayable();
        }

    }
    public int GetCurSkillId()
    {
        return _CurSkillList[^1];
    }
    public bool ContainsSkill(in int skillId)
    {
        var contains = _SkillAdapterDic.ContainsKey(skillId);
        return contains;
    }
    public bool SkillIsEnd(in int skillId)
    {
        if (!_SkillAdapterDic.TryGetValue(skillId, out var skillAdapterList))
            return true;
        foreach (var adapter in skillAdapterList)
        {
            if (!adapter.IsPlayEnd())
                return false;
        }
        return true;
    }
    public void RemoveSkill(in int skillId)
    {
        var adapters = RemoveSkillData(skillId);
        if (adapters == null)
            return;
        foreach (var adapter in adapters)
            DisconnectLayerInput(adapter);
    }
    public void CancelSkill(in int skillId)
    {
        if (!_SkillAdapterDic.TryGetValue(skillId, out var skillAdapter))
            return;
        foreach (var item in skillAdapter)
            item.CancelPlayable();
    }

    public bool IsAddSkill(in int skillId)
    {
        var level = SkillMgr.Instance.GetSkillLevel(skillId);
        var skillCfg = GameSchedule.Instance.GetSkillCfg0(skillId);
        foreach (var (itemSkillId, skillPlayableAdapters) in _SkillAdapterDic)
        {
            var level2 = SkillMgr.Instance.GetSkillLevel(itemSkillId);
            if (level2 < level)
                continue;
            if (skillCfg.bIdleLayerPlay <= 0)
                return false;
            for (var index = 0; index < skillPlayableAdapters.Count; index++)
            {
                var skillAdapter = skillPlayableAdapters[index];
                if (skillAdapter.NextAnimLevelComdition())
                    continue;

                var skillCfg2 = GameSchedule.Instance.GetSkillCfg0(itemSkillId);
                if (skillCfg2.arrLayer == null)
                    continue;
                for (var i = 0; i < skillCfg2.arrLayer.Length; i++)
                {
                    var itemLayer = skillCfg2.arrLayer[i];
                    var value = Array.FindIndex(skillCfg.arrLayer, layer => layer == itemLayer);
                    if (value >= 0)
                        return false;
                }
            }
        }
        return true;
    }
    private void AddSkillData(ISkillPlayableAdapter skillAdapter)
    {
        var skillId = skillAdapter.GetSkillId();
        if (!_SkillAdapterDic.TryGetValue(skillId, out var layerList))
        {
            layerList = new();
            _SkillAdapterDic.Add(skillId, layerList);
        }
        layerList.Add(skillAdapter);
        _CurSkillList.Add(skillId);
    }
    private List<ISkillPlayableAdapter> RemoveSkillData(int skillId)
    {
        if (!_SkillAdapterDic.TryGetValue(skillId, out var adapters))
            return null;
        _SkillAdapterDic.Remove(skillId);
        var index = _CurSkillList.IndexOf(skillId);
        foreach (var adapter in adapters)
            adapter.RemovePlayable();

        if (index == _CurSkillList.Count - 1)
        {
            if (_CurSkillList.Count <= 1)
            {
                SetApplyRootMotion(false);
            }
            else
            {
                var nextSkillId = _CurSkillList[^2];
                var skillCfg = GameSchedule.Instance.GetSkillCfg0(nextSkillId);
                SetApplyRootMotion(skillCfg.bApplyRootMotion > 0);
            }
        }
        _CurSkillList.RemoveAt(index);

        return adapters;
    }
    #endregion

    private int ConnectLayerInput(int skillId, ISkillPlayableAdapter skillAdapter, EnAnimLayer layer)
    {
        if (!_PlayableGraph.TryGetLayerMixerInfo(layer, out var layerInfo))
        {
            layerInfo = _PlayableGraph.CreateLayerMixerInfo(layer);
            _PlayableGraph.AddMixerLayer(layer);
        }
        else if (layerInfo.IsStatus(EnAnimLayerStatus.Exiting))
        {
            _PlayableGraph.AddMixerLayer(layer);
        }

        int portID;
        if (layerInfo.GetConnectCount() > 0)
        {
            var from = layerInfo.DisconnectNoDestroy(GlobalConfig.Int0);
            var mainAdapter2 = from.GetMainPlayableAdapter();
            RemoveAdapter(mainAdapter2);
            from.Complete();

            var mixerAdapter = _PlayableGraph.CreateMixerPlayableAdapter(from, skillAdapter, GlobalConfig.Float02, MixerComplete);
            portID = layerInfo.Connect(mixerAdapter);
        }
        else
        {
            portID = layerInfo.Connect(skillAdapter);
        }
        var mainAdapter = skillAdapter.GetMainPlayableAdapter();
        AddAdapter(mainAdapter, layer, portID, skillId);
        return portID;
    }
    private void MixerComplete(PlayableMixerAdapter mixerAdapter, IPlayableAdapter from, IPlayableAdapter to)
    {
        var mainAdapter = mixerAdapter.GetMainPlayableAdapter();
        if (!_Adapter2ConnectInfo.TryGetValue(mainAdapter, out var connectInfo))
            return;
        if (!_PlayableGraph.TryGetLayerMixerInfo(connectInfo.layer, out var layerInfo))
            return;
        mixerAdapter.DisconnectAll();
        layerInfo.Disconnect(connectInfo.port);
        IPlayableAdapter.Destroy(from);
        layerInfo.Connect(to);
    }
    #region adapter
    private void AddAdapter(IPlayableAdapter adapter, EnAnimLayer layer, int portID, int skillId)
    {
        var connectInfo = ClassPoolMgr.Instance.Pull<LayerMixerConnectInfo>();
        connectInfo.layer = layer;
        connectInfo.port = portID;
        connectInfo.skillId = skillId;
        _Adapter2ConnectInfo.Add(adapter, connectInfo);
        if (!adapter.IsLoop())
            _NoLoopPlayableList.Add(adapter);
    }
    private void RemoveAdapter(IPlayableAdapter adapter)
    {
        if (!_Adapter2ConnectInfo.TryGetValue(adapter, out var connectInfo2))
            return;
        _Adapter2ConnectInfo.Remove(adapter);
        _NoLoopPlayableList.Remove(adapter);
        RemoveSkillData(connectInfo2.skillId);
        ClassPoolMgr.Instance.Push(connectInfo2);
    }
    #endregion



    private void DisconnectLayerInput(IPlayableAdapter adapter)
    {
        if (!_Adapter2ConnectInfo.TryGetValue(adapter, out var connectInfo))
            return;
        var layer = connectInfo.layer;
        var port = connectInfo.port;
        var mainAdapter = adapter.GetMainPlayableAdapter();
        RemoveAdapter(mainAdapter);
        _PlayableGraph.DisconnectLayerInput(layer, port);
    }

    private void SetApplyRootMotion(bool applyRootMotion)
    {
        _PlayableGraph.SetApplyRootMotion(applyRootMotion);
    }


    public void Update()
    {
        for (var i = 0; i < _NoLoopPlayableList.Count; i++)
        {
            var item = _NoLoopPlayableList[i];
            if (!item.IsPlayEnd())
                continue;
            i--;
            DisconnectLayerInput(item);
        }
    }
}
