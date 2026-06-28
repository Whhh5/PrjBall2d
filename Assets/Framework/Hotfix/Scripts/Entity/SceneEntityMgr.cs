using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneEntityMgr : Singleton<SceneEntityMgr>
{
    private readonly Dictionary<int, SceneEntityData> _SceneEntityDatas = new();


    private SceneEntityData GetSceneEntityData(in int entityId)
    {
        return _SceneEntityDatas[entityId];
    }
    public bool EntityIsValid(in int entityId, in EnSceneEntityStatus checkRules, in EnSceneEntityStatus ignoreRules)
    {
        if (!_SceneEntityDatas.TryGetValue(entityId, out var entityData))
            return false;

        ref readonly var entityState = ref entityData.GetSceneEntityStatus();
        var checkResult = checkRules & entityState & ~ignoreRules;
        if (checkResult != EnSceneEntityStatus.None)
            return false;
        return true;
    }
    public bool EntityIsValid(in int entityId)
    {
        var result = EntityIsValid(entityId, EnSceneEntityStatus.Die, EnSceneEntityStatus.None);
        return result;
    }

    public int CreateEntityData(in int typeId)
    {
        var userData = ClassPoolMgr.Instance.Pull<GameEntityDataUserData>();
        var sceneEntityId = CreateEntityData(in typeId, in userData);
        ClassPoolMgr.Instance.Push(in userData);
        return sceneEntityId;
    }
    public int CreateEntityData(in int typeId, in GameEntityDataUserData userData)
    {
        var sceneEntityId = EntityMgr.Instance.CreateEntityData(in typeId, userData);
        var sceneEntity = EntityMgr.Instance.GetEntityData<SceneEntityData>(in sceneEntityId);

        _SceneEntityDatas.Add(sceneEntityId, sceneEntity);
        return sceneEntityId;
    }
    public void DestroyEntityData(in int entityId)
    {
        if (!_SceneEntityDatas.ContainsKey(entityId))
            return;
        _SceneEntityDatas.Remove(entityId);
        EntityMgr.Instance.RecycleEntityData(in entityId);

    }
    public ref readonly Vector3 GetSceneEntityHitPosition(in int entityId)
    {
        var entityData = GetSceneEntityData(in entityId);
        ref readonly var pos = ref entityData.GetHitPosition();
        return ref pos;
    }
    public ref readonly EnGameLayerInt GetEntityEnemyLayer(in int entityId)
    {
        var entityData = GetSceneEntityData(in entityId);
        ref readonly var enemyLayer = ref entityData.GetEnemyLayer();
        return ref enemyLayer;
    }
    public ref readonly EnGameLayerInt GetEntityFriendLayer(in int entityId)
    {
        var entityData = GetSceneEntityData(in entityId);
        ref readonly var friendLayer = ref entityData.GetFriendLayer();
        return ref friendLayer;
    }
    public ref readonly EnGameLayerInt GetEntityLayer(in int entityId)
    {
        var entityData = GetSceneEntityData(in entityId);
        ref readonly var layer = ref entityData.GetLayer();
        return ref layer;
    }

    #region Com
    public void AddCom(in int entityId, in EnSceneEntityComType comType)
    {
        var entityData = GetSceneEntityData(in entityId);
        entityData.AddEntityCom(comType);
    }
    public void RemoveCom(in int entityId, in EnSceneEntityComType comType)
    {
        var entityData = GetSceneEntityData(in entityId);
        entityData.RemoveEntityCom(in comType);
    }
    public void RemoveAllCom(in int entityId)
    {
        var entityData = GetSceneEntityData(in entityId);
        entityData.RemoveAllEntityCom();
    }
    public T GetEntityCom<T>(in int entityId, in EnSceneEntityComType comType)
        where T : class, IEntity3DComData, new()
    {
        var entityData = GetSceneEntityData(in entityId);
        var com = entityData.GetEntityCom<T>(in comType);
        return com;
    }
    public bool HasEntityCom(in int entityId, in EnSceneEntityComType comType)
    {
        var entityData = GetSceneEntityData(in entityId);
        var result = entityData.ContainsEntityCom(in comType);
        return result;
    }

    #endregion

    #region cmd
    public void AddCmd(in int entityId, in EnEntityCmd cmd)
    {
        var entityData = GetSceneEntityData(in entityId);
        var cmdCom = entityData.GetEntityCom<EntityCmdComData>(EnSceneEntityComType.Cmd);
        cmdCom.AddCmd(in cmd);
    }
    public void RemoveCmd(in int entityId, in EnEntityCmd cmd)
    {
        var entityData = GetSceneEntityData(in entityId);
        var cmdCom = entityData.GetEntityCom<EntityCmdComData>(EnSceneEntityComType.Cmd);
        cmdCom.RemoveCmd(in cmd);
    }
    public void RemoveCmd(in int entityId, in int cmd)
    {
        RemoveCmd(in entityId, (EnEntityCmd)cmd);
    }
    public void AddCmd(in int entityId, in int cmd)
    {
        AddCmd(in entityId, (EnEntityCmd)cmd);
    }

    public bool HasCmd(in int entityId, in int cmd)
    {
        var result = HasCmd(in entityId, (EnEntityCmd)cmd);
        return result;
    }
    public bool HasCmd(in int entityId, in EnEntityCmd cmd)
    {
        var entityData = GetSceneEntityData(in entityId);
        var cmdCom = entityData.GetEntityCom<EntityCmdComData>(EnSceneEntityComType.Cmd);
        var result = cmdCom.ContainsCmd(cmd);
        return result;
    }
    #endregion

    #region skill
    public int GetEntityCurSkillId(int entityID)
    {
        var animCom = GetEntityCom<EntityAnimComData>(in entityID, EnSceneEntityComType.Animation);
        var skillId = animCom.GetCurSkillId();
        return skillId;
    }
    #endregion

    #region controller

    public void AddControllerType(in int entityId, in EnSceneEntityControllerType controllerType)
    {
        var entityData = EntityMgr.Instance.GetEntityData<SceneEntityData>(in entityId);
        entityData.AddControllerType(in controllerType);
    }

    public void RemoveControllerType(in int entityId, in EnSceneEntityControllerType controllerType)
    {
        var entityData = EntityMgr.Instance.GetEntityData<SceneEntityData>(in entityId);
        entityData.RemoveControllerType(in controllerType);
    }

    public bool IsCanController(in int entityId, in EnSceneEntityControllerType controllerType)
    {
        var entityData = EntityMgr.Instance.GetEntityData<SceneEntityData>(in entityId);
        var result = entityData.IsCanController(in controllerType);
        return result;
    }
    #endregion
}

