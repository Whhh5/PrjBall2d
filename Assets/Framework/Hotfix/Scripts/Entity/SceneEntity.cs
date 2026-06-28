using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeChatWASM;


public class MonsterEntityDataUserData : GameEntityDataUserData
{
    public int monsterCfgId;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        monsterCfgId
            = -1;
    }
}
public abstract class SceneEntityData<T, TUserData> : SceneEntityData<T>
    where T : SceneEntity
    where TUserData : MonsterEntityDataUserData
{
    protected abstract void OnPoolInit(TUserData userData);
    protected sealed override void OnPoolInit(MonsterEntityDataUserData userData)
    {
        base.OnPoolInit(userData);
        OnPoolInit(userData as TUserData);
    }
}
public abstract class SceneEntityData<T> : SceneEntityData
    where T : SceneEntity
{

}
public abstract class SceneEntityData : GameEntityData<SceneEntity, MonsterEntityDataUserData>, ICameraTarget
{
    private EnSceneEntityStatus _SceneEntityStatus;
    private EnGameLayerInt _EnemyLayerInt;
    private EnGameLayerInt _FriendLayerInt;
    private EnGameLayerInt _LayerInt;
    private int _MonsterCfgId;

    protected virtual Vector3 HitPositionOffset { get; } = Vector3.up;
    private Vector3 _HitPosition;

    private Dictionary<EnSceneEntityComType, IEntity3DComData> _EntityComs = new();
    
    private EnSceneEntityControllerType _ControllerType = EnSceneEntityControllerType.None;
    public virtual ref readonly Vector3 GetHitPosition()
    {
        return ref _HitPosition;
    }

    public override EnLoadTarget LoadTarget
    {
        get
        {
            var cfg = GameSchedule.Instance.GetMonsterCfg0(_MonsterCfgId);
            return (EnLoadTarget)cfg.nAssetCfgID;
        }
    }
    public override void Destroy()
    {
        base.Destroy();
        _EnemyLayerInt 
            = _FriendLayerInt 
            = _LayerInt
            = EnGameLayerInt.None;
        _SceneEntityStatus = EnSceneEntityStatus.None;
        _ControllerType =  EnSceneEntityControllerType.None;
    }

    protected override void OnPoolInit(MonsterEntityDataUserData userData)
    {
        _MonsterCfgId = userData.monsterCfgId;
        var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(userData.monsterCfgId);
        for (int i = 0; i < monsterCfg.arrEnemyLayer.Length; i++)
            _EnemyLayerInt |= (EnGameLayerInt)(1 << monsterCfg.arrEnemyLayer[i]);
        for (int i = 0; i < monsterCfg.arrFriendLayer.Length; i++)
            _FriendLayerInt |= (EnGameLayerInt)(1 << monsterCfg.arrFriendLayer[i]);
        for (int i = 0; i < monsterCfg.arrLayer.Length; i++)
            _LayerInt |= (EnGameLayerInt)(1 << monsterCfg.arrLayer[i]);

        _SceneEntityStatus = EnSceneEntityStatus.Normalize;
    }

    public ref readonly int GetMonsterCfgId()
    {
        return ref _MonsterCfgId;
    }
    public ref readonly EnSceneEntityStatus GetSceneEntityStatus()
    {
        return ref _SceneEntityStatus;
    }

    public ref readonly EnGameLayerInt GetEnemyLayer()
    {
        return ref _EnemyLayerInt;
    }
    public ref readonly EnGameLayerInt GetFriendLayer()
    {
        return ref _FriendLayerInt;
    }
    public ref readonly EnGameLayerInt GetLayer()
    {
        return ref _LayerInt;
    }

    public void AddControllerType(in EnSceneEntityControllerType controllerType)
    {
        _ControllerType |= controllerType;
    }

    public void RemoveControllerType(in EnSceneEntityControllerType controllerType)
    {
        _ControllerType &= ~controllerType;
    }

    public bool IsCanController(in EnSceneEntityControllerType controllerType)
    {
        return controllerType >= _ControllerType;
    }

    public override void SetPosition(in Vector3 worldPos)
    {
        base.SetPosition(worldPos);
        UpdateHitPosition();
    }
    public override void SetLocalRotation(in Vector3 localRotation)
    {
        base.SetLocalRotation(in localRotation);
        UpdateHitPosition();
    }

    private void UpdateHitPosition()
    {
        ref readonly var curPos = ref GetWorldPos();
        _HitPosition = curPos
            + Forword * HitPositionOffset.z
            + Up * HitPositionOffset.y
            + Right * HitPositionOffset.x;
    }

    public override void OnEnable()
    {
        base.OnEnable();

        foreach (var item in _EntityComs)
        {
            if (!item.Value.IsActive())
                item.Value.OnEnable();
        }
    }
    public override void OnDisable()
    {
        base.OnDisable();

        foreach (var item in _EntityComs)
        {
            if (item.Value.IsActive())
                item.Value.OnDisable();
        }
    }

    public override void OnGOCreate()
    {
        base.OnGOCreate();
        foreach (var item in _EntityComs)
            item.Value.OnCreateGO();
    }
    public override void OnGODestroy()
    {
        foreach (var item in _EntityComs)
            item.Value.OnDestroyGO();
        base.OnGODestroy();
    }

    #region coms
    public T GetEntityCom<T>(in EnSceneEntityComType comType)
        where T : class, IEntity3DComData
    {
        var entityCom = GetEntityCom(comType);
        return entityCom as T;
    }
    public IEntity3DComData GetEntityCom(in EnSceneEntityComType comType)
    {
        if (!_EntityComs.TryGetValue(comType, out var entityCom))
            return default;
        return entityCom;
    }
    public bool AddEntityCom(in EnSceneEntityComType comType)
    {
        var userData = ClassPoolMgr.Instance.Pull<Entity3DComDataUserData>();
        userData.entityID = _EntityID;
        var entityCom = SceneEntityComUtility.CreateSceneEntityComData(in comType, userData);
        ClassPoolMgr.Instance.Push(userData);
        _EntityComs.Add(entityCom.GetComType(), entityCom);
        if (_IsLoadSuccess)
            entityCom.OnCreateGO();
        if (entityCom.IsCanActive())
            entityCom.OnEnable();
        return true;
    }

    public bool RemoveEntityCom(in EnSceneEntityComType comType)
    {
        if (!_EntityComs.TryGetValue(comType, out var entityCom))
            return false;
        DestroyComContent(entityCom);
        SceneEntityComUtility.DestroySceneEntityComData(entityCom);
        _EntityComs.Remove(comType);
        return true;
    }
    public void RemoveAllEntityCom()
    {
        foreach (var (_, entityCom) in _EntityComs)
        {
            DestroyComContent(entityCom);
            ClassPoolMgr.Instance.Push(entityCom);
        }
        _EntityComs.Clear();
    }
    private void DestroyComContent(IEntity3DComData entityCom)
    {
        if (entityCom.IsActive())
            entityCom.OnDisable();
        if (_IsLoadSuccess)
            entityCom.OnDestroyGO();
        entityCom.OnPoolDestroy();
    }
    public bool ContainsEntityCom(in EnSceneEntityComType comType)
    {
        if (!_EntityComs.ContainsKey(comType))
            return false;
        return true;
    }
    #endregion

    public Vector3 GetTopPoint()
    {
        return WorldPos + new Vector3(0, 2, 0);
    }

    public bool IsCameraTargetValid()
    {
        return _IsLoadSuccess;
    }

    public Vector3 GetCameraTargetWorldPos()
    {
        return WorldPos + new Vector3(-5, 10, -5);
    }

    public Vector3 GetCameraTargetOffsetPos()
    {
        return Vector3.zero;
    }

    public Vector3 GetCameraLookAtPos()
    {
        return WorldPos + Vector3.up * 1;
    }
}

public abstract class SceneEntity<T> : SceneEntity
    where T : SceneEntityData
{
    public T GetEntityData()
    {
        return _EntityData as T;
    }
}
public abstract class SceneEntity : GameEntity, IEntityAnimCom, ISceneEntityRigidbodyCom, IEntityCameraCom
{
    public static implicit operator int(SceneEntity entity3D)
    {
        return entity3D._Entity3DData.EntityID;
    }
    private SceneEntityData _Entity3DData = null;
    private Animator _Anim = null;
    private Rigidbody _Rigidbody = null;

    protected override void Awake()
    {
        base.Awake();
        _Anim = GetComponent<Animator>();
        _Rigidbody = GetComponent<Rigidbody>();
    }
    public override void LoadCompeletion()
    {
        _Entity3DData = _EntityData as SceneEntityData;
        base.LoadCompeletion();
    }

    public Animator GetAnimator()
    {
        return _Anim;
    }

    public Rigidbody GetRigidbody()
    {
        return _Rigidbody;
    }

    public void IncrementRotationOffset(Vector3 quaternion)
    {
        
    }

    public void IncrementRadiusOffset(float radius)
    {
        
    }
}
