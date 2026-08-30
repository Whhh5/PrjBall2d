using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UICoreMgr : Singleton<UICoreMgr>
{
    private readonly HashSet<int> _WearCoreIds = new(30);
    private readonly HashSet<int> _BagCoreIds = new(100);
    private List<int> _TempIntList = new(100);

    public override void Destroy()
    {
        EventMgr.Instance.Unregister<GameBattlePlayerCreateEventData>(EnABBEvent.EVENT_BATTLE_PLAYER_CREATE, 0, 0, OnEvent_PlayerCreate);
        base.Destroy();
        _WearCoreIds.Clear();
        _BagCoreIds.Clear();
    }
    public override async UniTask AwakeAsync()
    {
        await base.AwakeAsync();

        var perData = PersistentMgr.Instance.GetData<PersistentCoreData>(EnPersistentId.Core);
        _TempIntList.Clear();
        GetAlreadyCoreIds(ref _TempIntList);
        for (int i = 0; i < _TempIntList.Count; i++)
        {
            var coreId = _TempIntList[i];
            var isWear = perData.IsWear(coreId);
            if (isWear)
                _WearCoreIds.Add(coreId);
            else
                _BagCoreIds.Add(coreId);
        }

        EventMgr.Instance.Register<GameBattlePlayerCreateEventData>(EnABBEvent.EVENT_BATTLE_PLAYER_CREATE, 0, 0, OnEvent_PlayerCreate);
    }

    private void OnEvent_PlayerCreate(in int entityId, in int _, in GameBattlePlayerCreateEventData data)
    {
        _TempIntList.Clear();
        GetWearCoreIds(ref _TempIntList);
        for (int i = 0; i < _TempIntList.Count; i++)
        {
            var coreId = _TempIntList[i];
            UICoreUtility.ApplyEntity(data.entityId, coreId);
        }
    }

    public int AddCore(int coreGoodsCfgId)
    {
        var goodsCfg = GameSchedule.Instance.GetGoodsCfg0(coreGoodsCfgId);
        var coreCfgId = goodsCfg.nLinkCfgId;
        var quality = (EnCoreQuality)Random.Range((int)EnCoreQuality.None + 1, (int)EnCoreQuality.EnumCount);
        var effectValue = UICoreUtility.GetRandomCoreEffectValue(coreCfgId, quality);
        var coreId = AddCore(coreGoodsCfgId, effectValue, quality);
        return coreId;
    }
    public int AddCore(int coreGoodsCfgId, int effectValue, EnCoreQuality quality)
    {
        var perData = PersistentMgr.Instance.GetData<PersistentCoreData>(EnPersistentId.Core);
        var coreId = perData.AddCoreData(coreGoodsCfgId, effectValue, quality);
        _BagCoreIds.Add(coreId);

        var evData = ClassPoolMgr.Instance.Pull<GameCoreAddEventData>();
        evData.Setting(coreId);
        EventMgr.Instance.FireExecute(EnABBEvent.EVENT_CORE_ADD, coreId, 0, evData);
        ClassPoolMgr.Instance.Push(evData);
        return coreId;
    }
    public bool RemoveCore(int coreId)
    {
        var evData = ClassPoolMgr.Instance.Pull<GameCoreRemoveEventData>();
        evData.Setting(coreId);
        EventMgr.Instance.FireExecute(EnABBEvent.EVENT_CORE_REMOVE, coreId, 0, evData);
        ClassPoolMgr.Instance.Push(evData);

        _WearCoreIds.Remove(coreId);
        _BagCoreIds.Remove(coreId);

        var perData = PersistentMgr.Instance.GetData<PersistentCoreData>(EnPersistentId.Core);
        var result = perData.RemoveCoreData(coreId);
        return result;
    }
    public float GetCoreEffectValue(in int coreId)
    {
        var perData = PersistentMgr.Instance.GetData<PersistentCoreData>(EnPersistentId.Core);
        var value = perData.GetEffectValue(coreId);
        return value;
    }
    public void SetCoreEffectValue(in int coreId, in float effectValue)
    {
        var perData = PersistentMgr.Instance.GetData<PersistentCoreData>(EnPersistentId.Core);
        var lastValue = perData.GetEffectValue(coreId);
        perData.SetEffectValue(coreId, effectValue);

        var evData = ClassPoolMgr.Instance.Pull<GameCoreChangeEffectValueEventData>();
        evData.Setting(coreId, lastValue, effectValue);
        EventMgr.Instance.FireExecute(EnABBEvent.EVENT_CORE_CHANGE_EFFECTVALUE, coreId, 0, evData);
        ClassPoolMgr.Instance.Push(evData);
    }
    public void SetCoreGemId(in int coreId, in int grooveIndex, in int gemId)
    {
        var perData = PersistentMgr.Instance.GetData<PersistentCoreData>(EnPersistentId.Core);
        var lastGemId = perData.GetGemId(coreId, grooveIndex);
        if (lastGemId > 0)
        {
            ClearCoreGem(coreId, grooveIndex);
        }
        perData.SetGemId(coreId, grooveIndex, gemId);
        UIGemMgr.Instance.SetGemInlayStatus(gemId, true);

        var evData = ClassPoolMgr.Instance.Pull<GameCoreInlayGemEventData>();
        evData.Setting(grooveIndex, gemId);
        EventMgr.Instance.FireExecute(EnABBEvent.EVENT_CORE_INLAY_GEM, coreId, 0, evData);
        ClassPoolMgr.Instance.Push(evData);
    }
    public void ClearCoreGem(in int coreId, in int grooveIndex)
    {
        var perData = PersistentMgr.Instance.GetData<PersistentCoreData>(EnPersistentId.Core);
        var lastGemId = perData.GetGemId(coreId, grooveIndex);
        if (lastGemId <= 0)
            return;
        UIGemMgr.Instance.SetGemInlayStatus(lastGemId, false);
        perData.SetGemId(coreId, grooveIndex, -1);

        EventMgr.Instance.FireExecute(EnABBEvent.EVENT_CORE_CLEAR_GEM, coreId, grooveIndex);
    }
    public int GetWearIndex(in int coreId)
    {
        var perData = PersistentMgr.Instance.GetData<PersistentCoreData>(EnPersistentId.Core);
        var value = perData.GetWearIndex(coreId);
        return value;
    }

    public void GetAlreadyCoreIds(ref List<int> list)
    {
        var perData = PersistentMgr.Instance.GetData<PersistentCoreData>(EnPersistentId.Core);
        perData.GetCoreIds(ref list);
    }
    public void GetWearCoreIds(ref List<int> ids)
    {
        foreach (var id in _WearCoreIds)
            ids.Add(id);
    }
    public void GetBagCoreIds(ref List<int> ids)
    {
        foreach (var id in _BagCoreIds)
            ids.Add(id);
    }
    public bool IsWear(int coreId)
    {
        return _WearCoreIds.Contains(coreId);
    }
    public bool WearCore(int coreId)
    {
        if (!_BagCoreIds.Contains(coreId))
            return false;
        if (_WearCoreIds.Contains(coreId))
            return false;
        var coreType = UICoreUtility.GetCoreType(coreId);
        if (!TryGetWearIndex(coreType, out var wearIndex))
            return false;

        _BagCoreIds.Remove(coreId);
        _WearCoreIds.Add(coreId);
        var perData = PersistentMgr.Instance.GetData<PersistentCoreData>(EnPersistentId.Core);
        perData.SetIsWear(coreId, true);
        perData.SetWearIndex(coreId, wearIndex);

        var evData = ClassPoolMgr.Instance.Pull<GameCoreWearEventData>();
        evData.Setting(coreId, wearIndex);
        EventMgr.Instance.FireExecute(EnABBEvent.EVENT_CORE_WEAR, coreId, 0, evData);
        ClassPoolMgr.Instance.Push(evData);
        return true;
    }
    public bool TryGetWearIndex(EnCoreType type, out int wearIndex)
    {
        var indexs = new HashSet<int>(_WearCoreIds.Count);
        foreach (var cordId in _WearCoreIds)
        {
            var coreType = UICoreUtility.GetCoreType(cordId);
            if (coreType != type)
                continue;
            var index = GetWearIndex(cordId);
            indexs.Add(index);
        }
        wearIndex = -1;
        var maxCount = UICoreUtility.GetWearUnitCount();
        for (int i = 0; i < maxCount; i++)
        {
            if (indexs.Contains(i))
                continue;
            wearIndex = i;
            return true;
        }
        return false;
    }
    public bool UnwearCore(int coreId)
    {
        if (!_WearCoreIds.Contains(coreId))
            return false;
        if (_BagCoreIds.Contains(coreId))
            return false;
        var perData = PersistentMgr.Instance.GetData<PersistentCoreData>(EnPersistentId.Core);
        var lastWearIndex = perData.GetWearIndex(coreId);
        var evData = ClassPoolMgr.Instance.Pull<GameCoreUnwearEventData>();
        evData.Setting(coreId, lastWearIndex);
        EventMgr.Instance.FireExecute(EnABBEvent.EVENT_CORE_UNWEAR, coreId, 0, evData);
        ClassPoolMgr.Instance.Push(evData);

        _BagCoreIds.Add(coreId);
        _WearCoreIds.Remove(coreId);
        perData.SetIsWear(coreId, false);
        perData.SetWearIndex(coreId, -1);
        return true;
    }
}
