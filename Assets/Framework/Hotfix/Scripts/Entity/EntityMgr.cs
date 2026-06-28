using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EntityMgr : Singleton<EntityMgr>
{
    private Dictionary<int, GameEntityData> m_EntityDataMap = new();

    public GameEntityData GetEntityData(in int entityID)
    {
        if (!m_EntityDataMap.TryGetValue(entityID, out var entityData))
            return null;
        return entityData;
    }
    public T GetEntityData<T>(in int entityID)
    {
        var entityData = GetEntityData(entityID);
        if (entityData is not T tData)
            return default;
        return tData;
    }
    public bool IsValid(in int entityID)
    {
        var result = m_EntityDataMap.ContainsKey(entityID);
        return result;
    }

    public int CreateEntityData(in int typeId)
    {
        var userData = ClassPoolMgr.Instance.Pull<GameEntityDataUserData>();
        var entityId = CreateEntityData(in typeId, in userData);
        ClassPoolMgr.Instance.Push(userData);
        return entityId;
    }

    public int CreateEntityData(in int typeId, in GameEntityDataUserData userData)
    {
        var entityId = ABBUtil.GetTempKey();
        userData.entityId = entityId;
        var entityData = TypeDefine.CreateInstance<GameEntityData>(typeId, userData);
        entityData.Create();
        entityData.OnEnable();
        m_EntityDataMap.Add(entityId, entityData);
        return entityId;
    }

    public int CreateEntityData<T>(in GameEntityDataUserData userData)
        where T : GameEntityData, new()
    {
        var entityID = ABBUtil.GetTempKey();
        userData.entityId = entityID;
        var entityData = ClassPoolMgr.Instance.Pull<T>(userData);
        entityData.SetLoadStatus(EnLoadStatus.Start);
        entityData.Create();
        entityData.OnEnable();
        m_EntityDataMap.Add(entityID, entityData);
        return entityID;
    }

    public int CreateEntityData<T>()
        where T : GameEntityData, new()
    {
        var userData = ClassPoolMgr.Instance.Pull<GameEntityDataUserData>();
        var entityID = CreateEntityData<T>(userData);
        ClassPoolMgr.Instance.Push(in userData);
        return entityID;
    }
    public void RecycleEntityData(in int entityID)
    {
        if (!m_EntityDataMap.TryGetValue(entityID, out var entityData))
            return;
        if (entityData.IsLoadSuccess)
            UnloadEntity(entityID);
        m_EntityDataMap.Remove(entityID);
        if (entityData.GetActive())
            entityData.OnDisable();
        entityData.Destroy();
        ClassPoolMgr.Instance.Push(entityData);
    }
    public async void LoadEntity(int entityID)
    {
        var entityData = GetEntityData(entityID);
        if (entityData.LoadStatus != EnLoadStatus.Start)
            return;
        entityData.SetLoadStatus(EnLoadStatus.Loading);
        var loadKey = ABBUtil.GetTempKey();
        entityData.SetLoadKey(loadKey);
        var goID = await ABBGOMgr.Instance.CreateGOAsync(entityData.LoadTarget, entityData.ParentTran);
        if (goID < 0)
        {
            entityData.SetLoadStatus(EnLoadStatus.Failed);
            Debug.LogError($"加载失败 target: {entityData.LoadTarget}, loadId: {entityID}");
            return;
        }
        if (loadKey != entityData.LoadKey)
        {
            ABBGOMgr.Instance.DestroyGO(goID);
            //Debug.LogError(entityID);
            return;
        }
        entityData.SetLoadStatus(EnLoadStatus.Success);
        entityData.SetGOID(goID);
        entityData.SetIsLoadSuccess(true);
        entityData.OnGOCreate();
        var go = ABBGOMgr.Instance.GetGo(goID);
        var goEntity = go.GetComponent<GameEntity>();
        goEntity.SetEntityID(entityID);
        goEntity.LoadCompeletion();
    }
    public void UnloadEntity(int entityID)
    {
        var entityData = GetEntityData(entityID);
        if (entityData.IsLoadSuccess)
        {
            entityData.EntityGO.OnUnload();
            entityData.OnGODestroy();
            ABBGOMgr.Instance.DestroyGO(entityData.GoId);
            entityData.SetGOID(-1);
            entityData.SetIsLoadSuccess(false);
        }
        entityData.SetLoadStatus(EnLoadStatus.Start);
        entityData.SetLoadKey(-1);
    }
    public int EntityID2RoleID(int m_EntityID)
    {
        return 1;
    }
    public void OnDisableEntity(int entityID)
    {
        var entityData = GetEntityData(entityID);
        if (!entityData.GetActive())
            return;
        entityData.OnDisable();
    }
    public void OnEnableEntity(int entityID)
    {
        var entityData = GetEntityData(entityID);
        if (entityData.GetActive())
            return;
        entityData.OnEnable();
    }

    public bool IsLoadSuccess(in int m_EntityID)
    {
        var entityData = GetEntityData(m_EntityID);
        if (entityData.LoadStatus == EnLoadStatus.Success)
            return true;
        return false;
    }


    #region Other

    public Vector3 GetEntityForword(in int entityId)
    {
        var entityData = GetEntityData(in entityId);
        var forword = entityData.Forword;
        return forword;
    }

    public void SetEntityWorldPos(in int sceneEntityId, in Vector3 worldPos)
    {
        var entityData = GetEntityData(in sceneEntityId);
        entityData.SetPosition(in worldPos);
    }
    public ref readonly Vector3 GetEntityRotation(in int entityId)
    {
        var entityData = GetEntityData(in entityId);
        ref readonly var rotation = ref entityData.GetLocalRotation();
        return ref rotation;
    }
    public ref readonly Vector3 GetEntityScale(in int entityId)
    {
        var entityData = GetEntityData(in entityId);
        ref readonly var scale = ref entityData.GetLocalScale();
        return ref scale;
    }
    public void SetEntityRotation(in int sceneEntityId, in Vector3 rotation)
    {
        var entityData = GetEntityData(in sceneEntityId);
        entityData.SetLocalRotation(in rotation);
    }
    public Vector3 GetEntityRotationOffset(in int entityId, in Vector3 rotOffset)
    {
        ref readonly var rotation = ref GetEntityRotation(in entityId);
        var result = rotation + rotOffset;
        return result;
    }
    public Vector3 GetEntityScaleOffset(in int entityId, in Vector3 scaleOffset)
    {
        ref readonly var rotation = ref GetEntityScale(in entityId);
        var result = rotation + new Vector3(scaleOffset.x * rotation.x, scaleOffset.y * rotation.y, scaleOffset.z * rotation.z);
        return result;
    }
    public Vector3 GetEntityWorldPosOffset(in int entityId, in Vector3 dirOffset)
    {
        ref readonly var worldPos = ref GetEntityWorldPos(in entityId);
        var entityData = GetEntityData(in entityId);
        var result = worldPos
            + entityData.Forword * dirOffset.z
            + entityData.Up * dirOffset.y
            + entityData.Right * dirOffset.x;
        return result;
    }
    public ref readonly Vector3 GetEntityWorldPos(in int entityId)
    {
        var entityData = GetEntityData(in entityId);
        ref readonly var worldPos = ref entityData.GetWorldPos();
        return ref worldPos;
    }
    public T GetEntityComponent<T>(in int entityId)
    {
        var entityData = GetEntityData(in entityId);
        var com = entityData.GetEntityComponent<T>();
        return com;
    }
    public bool GetEntityIsLoadSuccess(in int entityID)
    {
        var entityData = GetEntityData(in entityID);
        return entityData.IsLoadSuccess;
    }

    #endregion
}

