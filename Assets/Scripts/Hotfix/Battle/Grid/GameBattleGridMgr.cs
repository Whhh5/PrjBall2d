using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameBattleGridMgr : Singleton<GameBattleGridMgr>, IGameMapLifecycle
{
    private readonly Dictionary<int, GameGridInfo> _GridDic = new();
    private readonly Dictionary<int, HashSet<int>> _Entity2Index = new();

    public override void Destroy()
    {
        GameMapMgr.Instance.UnregisterLifecycle(this);
        base.Destroy();
    }
    public override async UniTask AwakeAsync()
    {
        await base.AwakeAsync();
        GameMapMgr.Instance.RegisterLifecycle(this);
    }
    public EnGameMapLifecycleOrder GetMapLifecycleOrder()
    {
        return EnGameMapLifecycleOrder.GameBattleGridMgr;
    }
    public void EnterMap(int mapId, int chapterId)
    {

    }

    public void ExitMap()
    {
        ClearGridEntity();
    }

    public void ReenterMap()
    {
        ResetGridInfo();
    }
    public void CreateGridEntity(int index, Vector2 worldPos)
    {
        var info = ClassPoolMgr.Instance.Pull<GameGridInfo>();
        _GridDic.Add(index, info);

        info.CreateGridEntityData(worldPos);
        info.LoadEntity();
    }
    private void ClearGridEntity()
    {
        foreach (var (_, gridInfo) in _GridDic)
        {
            ClassPoolMgr.Instance.Push(gridInfo);
        }
        _GridDic.Clear();
        _Entity2Index.Clear();
    }
    private void ResetGridInfo()
    {
        foreach (var (_, gridInfo) in _GridDic)
        {
            gridInfo.Reset();
        }
        _Entity2Index.Clear();
    }

    public bool GridStatusIs(int index, EnGridStatus status)
    {
        var gridInfo = _GridDic[index];
        var result = gridInfo.GridStatusIs(status);
        return result;
    }
    public Vector2 GetGridWorldPos(int index)
    {
        var gridInfo = _GridDic[index];
        var pos = gridInfo.GetGridWorldPos();
        return pos;
    }
    public bool IsValid(int index)
    {
        return _GridDic.ContainsKey(index);
    }
    public void AddGridEntity(int gridIndex, int entityId)
    {
        var gridInfo = _GridDic[gridIndex];
        gridInfo.AddEntity(entityId);

        if (!_Entity2Index.TryGetValue(entityId, out var gridIndexList))
        {
            gridIndexList = new();
            _Entity2Index.Add(entityId, gridIndexList);
        }
        if (!gridIndexList.Contains(gridIndex))
        {
            gridIndexList.Add(gridIndex);
        }
    }
    public void RemoveGridEntity(int gridIndex, int entityId)
    {
        var gridInfo = _GridDic[gridIndex];

        if (gridInfo.RemoveEntity(entityId) && !gridInfo.ContainsEntity(entityId))
        {
            var entity2Index = _Entity2Index[entityId];
            if (entity2Index.Remove(gridIndex))
            {
                if (entity2Index.Count == 0)
                {
                    _Entity2Index.Remove(entityId);
                }
            }
        }
    }
    public void RemoveGridEntity(int entityId)
    {
        if (!_Entity2Index.TryGetValue(entityId, out var gridIndexs))
            return;
        foreach (var index in gridIndexs)
        {
            var gridInfo = _GridDic[index];
            gridInfo.RemoveEntityAll(entityId);
        }
        _Entity2Index.Remove(entityId);
    }
    public bool ContainsGridEntity(int gridIndex, int entityId)
    {
        var gridInfo = _GridDic[gridIndex];
        var result = gridInfo.ContainsEntity(entityId);
        return result;
    }
    public void GetGridEntityIds(in int gridIndex, ref List<int> entityIds)
    {
        var gridInfo = _GridDic[gridIndex];
        gridInfo.GetEntityIds(ref entityIds);
    }

}

