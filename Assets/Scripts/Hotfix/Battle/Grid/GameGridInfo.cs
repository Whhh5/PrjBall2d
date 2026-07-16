
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Flags]
public enum EnGridStatus
{
    None = 0,
    Idle = 1 << 0, // 空闲
    Lock = 1 << 1, // 未激活，禁止使用
    Entity = 1 << 2, // 有实体
}
public class GameGridInfo : IClassPoolInit
{
    private int chunkEntityId;
    private readonly Dictionary<int, int> monsterEntityIds = new(3);
    private EnGridStatus status;
    private readonly Dictionary<EnGridStatus, int> statusCountDic = new();
    private Vector3 pos;

    public void OnPoolDestroy()
    {
        Reset();
        if (chunkEntityId > 0)
        {
            DestroyGridEntityData();
        }
        chunkEntityId = -1;
        pos = Vector3.zero;
    }

    public void OnPoolInit<T>(T userData) where T : IClassPoolDestroy
    {
        status = EnGridStatus.Idle;
    }
    public void Reset()
    {
        monsterEntityIds.Clear();
        status = EnGridStatus.Idle;
        statusCountDic.Clear();
    }
    public void AddEntity(int entityId)
    {
        if (monsterEntityIds.TryGetValue(entityId, out var count))
        {
            monsterEntityIds[entityId] = count + 1;
        }
        else
        {
            monsterEntityIds.Add(entityId, 1);
            if (monsterEntityIds.Count == 1)
            {
                AddGridStatus(EnGridStatus.Entity);
            }
        }
    }
    public bool RemoveEntity(int entityId)
    {
        if (!monsterEntityIds.TryGetValue(entityId, out var count))
            return false;
        if (count == 1)
        {
            if (monsterEntityIds.Remove(entityId))
            {
                if (monsterEntityIds.Count == 0)
                {
                    RemoveGridStatus(EnGridStatus.Entity);
                }
            }
        }
        else
        {
            monsterEntityIds[entityId] = count - 1;
        }
        return true;
    }
    public void RemoveEntityAll(int entityId)
    {
        if (!monsterEntityIds.ContainsKey(entityId))
            return;
        if (monsterEntityIds.Remove(entityId))
        {
            if (monsterEntityIds.Count == 0)
            {
                RemoveGridStatus(EnGridStatus.Entity);
            }
        }
    }
    public bool ContainsEntity(int entityId)
    {
        return monsterEntityIds.ContainsKey(entityId);
    }
    public void CreateGridEntityData(Vector2 worldPos)
    {
        if (chunkEntityId > 0)
            throw new Exception("create error");
        pos = worldPos;
        chunkEntityId = EntityMgr.Instance.CreateEntityData<BattleMapChunk_1Data>();
        GameEntityUtility.SetEntityPos(chunkEntityId, worldPos);
    }
    public void DestroyGridEntityData()
    {
        EntityMgr.Instance.RecycleEntityData(chunkEntityId);
        chunkEntityId = -1;
    }
    public void LoadEntity()
    {
        EntityMgr.Instance.LoadEntity(chunkEntityId);
    }
    public void UnloadEntity()
    {
        EntityMgr.Instance.UnloadEntity(chunkEntityId);
    }
    public bool GridStatusIs(EnGridStatus status)
    {
        return status == this.status;
    }
    public bool GridStatusContains(EnGridStatus status)
    {
        return (this.status & status) == status;
    }
    public void AddGridStatus(EnGridStatus status)
    {
        if (!statusCountDic.TryGetValue(status, out var count))
            statusCountDic.Add(status, 0);

        if (count == 0)
        {
            this.status |= status;
        }
        statusCountDic[status] = count + 1;
    }
    public void RemoveGridStatus(EnGridStatus status)
    {
        if (!statusCountDic.TryGetValue(status, out var count))
            return;
        if (count == 1)
        {
            this.status &= ~status;
            statusCountDic.Remove(status);
        }
        else
        {
            statusCountDic[status] = count - 1;
        }

    }
    public Vector3 GetGridWorldPos()
    {
        return pos;
    }
    public void GetEntityIds(ref List<int> entityIds)
    {
        entityIds.AddRange(monsterEntityIds.Keys);
    }
}