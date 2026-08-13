using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameSkillPassivityMgr : Singleton<GameSkillPassivityMgr>, IGameMapLifecycle
{
    private class SkillPassivityRuntimeData : IClassPoolDestroy
    {
        public int star;
        public void OnPoolDestroy()
        {
            star = -1;
        }
    }

    private readonly Dictionary<int, GameBattleSkillPassivityData> _SkillId2Datas = new();
    private readonly Dictionary<int, SkillPassivityRuntimeData> _SkillRuntimeData = new();

    public override void Destroy()
    {
        foreach (var (_, rtData) in _SkillRuntimeData)
        {
            ClassPoolMgr.Instance.Push(rtData);
        }
        foreach (var (_, data) in _SkillId2Datas)
        {
            ClassPoolMgr.Instance.Push(data);
        }
        base.Destroy();
        _SkillRuntimeData.Clear();
    }

    public EnGameMapLifecycleOrder GetMapLifecycleOrder() => EnGameMapLifecycleOrder.GameSkillPassivityMgr;
    public override async UniTask OnEnableAsync()
    {
        await base.OnEnableAsync();
        GameMapMgr.Instance.RegisterLifecycle(this);
    }
    public override void OnDisable()
    {
        GameMapMgr.Instance.UnregisterLifecycle(this);
        base.OnDisable();
    }

    public void EnterMap(int mapId)
    {
        
    }

    public void ExitMap(int mapId)
    {
        ClearSkillPassivityData();
    }

    public void ReenterMap(int mapId)
    {
        ClearSkillPassivityData();
    }
    private void ClearSkillPassivityData()
    {
        var keys = _SkillRuntimeData.Keys.ToList();
        foreach (var key in keys)
        {
            RemoveSkillPassivity(key);
        }
        _SkillRuntimeData.Clear();
    }
    public List<int> GetSkillPassivityIds()
    {
        return _SkillRuntimeData.Keys.ToList();
    }
    public GameBattleSkillPassivityData GetData(int id)
    {
        var cfg = GameSchedule.Instance.GetSkillPassivityCfg0(id);
        if (!_SkillId2Datas.TryGetValue(id, out var data))
        {
            var playerEntityId = GamePlayerMgr.Instance.GetPlayerEntityId();
            var userData = ClassPoolMgr.Instance.Pull<GameBattleSkillPassivityUserData>();
            userData.skillPassivityId = id;
            userData.ownerEntityId = playerEntityId;
            data = TypeDefine.CreateInstance<GameBattleSkillPassivityData>(cfg.nClassTypeId, userData);
            _SkillId2Datas.Add(id, data);
        }
        return data;
    }
    public void AddSkillPassivity(int id, in int star)
    {
        var curStar = GetStar(id);
        SetSkillPassivityStar(id, curStar + star);
    }
    public void AddSkillPassivity(int id)
    {
        var curStar = GetStar(id);
        SetSkillPassivityStar(id, curStar + 1);
    }
    private void SetSkillPassivityStar(int id, int star)
    {
        var cfg = GameSchedule.Instance.GetSkillPassivityCfg0(id);
        if (!_SkillRuntimeData.TryGetValue(id, out var rtData))
        {
            if (star > cfg.nMaxStar)
                return;
            rtData = ClassPoolMgr.Instance.Pull<SkillPassivityRuntimeData>();
            _SkillRuntimeData.Add(id, rtData);
            rtData.star = star;

            var data = GetData(id);
            data.OnEnable(star);

            var userData = ClassPoolMgr.Instance.Pull<GameSkillPassivityAddEventData>();
            userData.skillPassivityId = id;
            userData.star = star;
            EventMgr.Instance.FireExecute(EnABBEvent.EVENT_SKILL_PASSIVITY_ADD, id, 0, userData);
            ClassPoolMgr.Instance.Push(userData);
        }
        else
        {
            if (star > cfg.nMaxStar)
                return;
            var lastStar = rtData.star;
            rtData.star = star;

            var data = GetData(id);
            data.OnUpstar(lastStar, star);

            var userData = ClassPoolMgr.Instance.Pull<GameSkillPassivityUpstarEventData>();
            userData.skillPassivityId = id;
            userData.lastStar = lastStar;
            userData.currentStar = star;
            EventMgr.Instance.FireExecute(EnABBEvent.EVENT_SKILL_PASSIVITY_UPSTAR, id, 0, userData);
            ClassPoolMgr.Instance.Push(userData);
        }
    }
    public void RemoveSkillPassivity(int id)
    {
        if (!_SkillRuntimeData.TryGetValue(id, out var rtData))
            return;

        var userData = ClassPoolMgr.Instance.Pull<GameSkillPassivityRemoveEventData>();
        userData.skillPassivityId = id;
        EventMgr.Instance.FireExecute(EnABBEvent.EVENT_SKILL_PASSIVITY_REMOVE, id, 0, userData);
        ClassPoolMgr.Instance.Push(userData);

        var data = GetData(id);
        data.OnDisable();

        ClassPoolMgr.Instance.Push(rtData);
        _SkillRuntimeData.Remove(id);
    }
    public int GetStar(int id)
    {
        if (!_SkillRuntimeData.TryGetValue(id, out var rtData))
            return 0;
        return rtData.star;
    }
    public T GetStarData<T>(int id)
        where T : GameBattleSkillPassivityStarData
    {
        var star = GetStar(id);
        var starData = GetStarData(id, star);
        return starData as T;
    }
    public T GetStarData<T>(int id, int star)
        where T : GameBattleSkillPassivityStarData
    {
        var starData = GetStarData(id, star);
        return starData as T;
    }
    public GameBattleSkillPassivityStarData GetStarData(int id, int star)
    {
        var data = GetData(id);
        var starData = data.GetStarData(star);
        return starData;
    }
    public T CreateStarData<T>(int ballId, int starLevel, ISerializeToIntArrayUserData userData)
        where T : GameBattleSkillPassivityStarData, new()
    {
        var ballCfg = GameSchedule.Instance.GetSkillPassivityCfg0(ballId);
        var startIndex = 0;
        for (int i = 0; i < starLevel - 1; i++)
        {
            var itemCount = ballCfg.arrStarParams[startIndex++];
            startIndex += itemCount;
        }
        var count = ballCfg.arrStarParams[startIndex++];
        userData.arrParams = ballCfg.arrStarParams;
        userData.startIndex = startIndex;
        userData.count = count;
        var starUserData = ClassPoolMgr.Instance.Pull<T>(userData);
        return starUserData;
    }
}
