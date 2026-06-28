using UnityEngine;


public class GameEntityDataUserData : IClassPoolUserData
{
    public int entityId;
    public virtual void OnPoolDestroy()
    {
        entityId
            = -1;
    }
}
public abstract class GameEntityData<T, TUserData> : GameEntityData
    where T : GameEntity
    where TUserData: GameEntityDataUserData
{
    protected T _Entity = null;
    public override void OnGODestroy()
    {
        _Entity = null;
        base.OnGODestroy();
    }
    public override void OnGOCreate()
    {
        base.OnGOCreate();
        _Entity = _GameEntity as T;
    }
    protected abstract void OnPoolInit(TUserData userData);
    public sealed override void OnPoolInit(GameEntityDataUserData userData)
    {
        base.OnPoolInit(userData);
        OnPoolInit(userData as TUserData);
    }
}
public abstract class GameEntityData<T>: GameEntityData
    where T: GameEntity
{
    protected T _Entity = null;
    public override void OnGODestroy()
    {
        _Entity = null;
        base.OnGODestroy();
    }
    public override void OnGOCreate()
    {
        base.OnGOCreate();
        _Entity = _GameEntity as T;
    }
}
public abstract class GameEntityData : IClassPool<GameEntityDataUserData>
{
    protected int _EntityID;
    public int EntityID => _EntityID;
    private int _GoId;
    public int GoId => _GoId;
    protected bool _IsLoadSuccess = false;
    public bool IsLoadSuccess => _IsLoadSuccess;
    private int _LoadKey = -1;
    public int LoadKey => _LoadKey;
    public abstract EnLoadTarget LoadTarget { get; }
    private EnLoadStatus _LoadStatus = EnLoadStatus.None;
    public EnLoadStatus LoadStatus => _LoadStatus;
    private Vector3 _WorldPos;
    public Vector3 WorldPos => _WorldPos;
    private Vector3 _LocalRotation;
    public virtual Vector3 LocalRotation => _LocalRotation;
    private Vector3 _LocalScale = Vector3.one;
    public Vector3 LocalScale => _LocalScale;
    public Vector3 Up { get; private set; } = Vector3.up;
    public Vector3 Right { get; private set; } = Vector3.right;
    public Vector3 Forword { get; private set; } = Vector3.forward;
    private Transform _ParentTran = null;
    public Transform ParentTran => _ParentTran;
    protected GameEntity _GameEntity = null;
    public GameEntity EntityGO => _GameEntity;

    private bool _IsActive = false;
    public virtual void OnPoolDestroy()
    {
        _EntityID
            = _GoId
            = _LoadKey
            = -1;
        _IsLoadSuccess
            = _IsActive
            = false;
        _LoadStatus = EnLoadStatus.None;
        _WorldPos = Vector3.zero;
        _ParentTran = null;
        _LocalScale = Vector3.one;
        _LocalRotation = Vector3.zero;
        Up = Vector3.up;
        Right = Vector3.right;
        Forword = Vector3.forward;
    }

    public virtual void OnEnable()
    {
        _IsActive = true;
    }

    public virtual void OnDisable()
    {
        _IsActive = false;
    }
    public virtual void OnPoolInit(GameEntityDataUserData userData)
    {
        _EntityID = userData.entityId;
        _LoadStatus = EnLoadStatus.Start;
    }

    public void PoolConstructor()
    {
    }

    public void OnPoolEnable()
    {
    }

    public void PoolRelease()
    {
    }
    public virtual void Create()
    {

    }
    public virtual void Destroy()
    {
        _LocalScale = Vector3.one;
        _LocalRotation = Vector3.zero;
    }
    public virtual void OnGOCreate()
    {
        var go = ABBGOMgr.Instance.GetGo(_GoId);
        _GameEntity = go.GetComponent<GameEntity>();
    }
    public virtual void OnGODestroy()
    {
        _GameEntity = null;
    }

    public bool GetActive()
    {
        return _IsActive;
    }
    public void SetLoadStatus(EnLoadStatus loadStatus)
    {
        _LoadStatus = loadStatus;
    }
    public virtual void SetGOID(int goID)
    {
        _GoId = goID;
    }
    public void SetLoadKey(int loadKey)
    {
        _LoadKey = loadKey;
    }
    public void SetIsLoadSuccess(bool isloadSuccess)
    {
        _IsLoadSuccess = isloadSuccess;
    }
    public virtual void SetPosition(in Vector3 worldPos)
    {
        _WorldPos = worldPos;
        if (_IsLoadSuccess)
            _GameEntity.SetPosition();
    }
    public void SetParentTran(Transform parentTran)
    {
        _ParentTran = parentTran;
        if (_IsLoadSuccess)
            _GameEntity.SetParentTran();
    }
    public Transform GetTransfrom()
    {
        if (!_IsLoadSuccess)
            return null;
        return _GameEntity.transform;
    }
    public T GetEntityComponent<T>()
    {
        if (!_IsLoadSuccess)
            return default;
        var entity = _GameEntity.GetEntity<T>();
        return entity;
    }
    public virtual void SetLocalRotation(in Vector3 localRotation)
    {
        _LocalRotation = localRotation;
        Forword = Quaternion.Euler(_LocalRotation) * Vector3.forward;
        Up = Quaternion.Euler(_LocalRotation) * Vector3.up;
        Right = Quaternion.Euler(_LocalRotation) * Vector3.right;
        if (_IsLoadSuccess)
            _GameEntity.SetLocalRotation();
    }
    public void SetLocalScale(Vector3 localScale)
    {
        _LocalScale = localScale;
        if (_IsLoadSuccess)
            _GameEntity.SetLocalScale();
    }
    public ref readonly Vector3 GetWorldPos()
    {
        return ref _WorldPos;
    }

    public ref readonly Vector3 GetLocalRotation()
    {
        return ref _LocalRotation;
    }
    public ref readonly Vector3 GetLocalScale()
    {
        return ref _LocalScale;
    }
}
public abstract class GameEntity<T> : GameEntity
    where T: GameEntityData
{
    protected T _GameEntityData = null;
    public override void OnUnload()
    {
        _GameEntityData = null;
        base.OnUnload();
    }
    public override void LoadCompeletion()
    {
        base.LoadCompeletion();
        _GameEntityData = _EntityData as T;
    }
}
public abstract class GameEntity : MonoBehaviour
{
    private int _EntityID;
    protected GameEntityData _EntityData = null;

    public static implicit operator int(GameEntity entity)
    {
        return entity._EntityID;
    }
    
    public void SetEntityID(int entityDataID)
    {
        var entityData = EntityMgr.Instance.GetEntityData(entityDataID);
        _EntityData = entityData;
        _EntityID = entityDataID;
    }
    public int GetEntityID()
    {
        return _EntityID;
    }
    public T GetEntity<T>()
    {
        if (!this.TryGetComponent<T>(out var com))
            return default;
        return com;
    }
    public virtual void LoadCompeletion()
    {
        SetParentTran();
        SetPosition();
        SetLocalRotation();
        SetLocalScale();
    }
    public virtual void SetLocalRotation()
    {
        transform.localEulerAngles = _EntityData.LocalRotation;
    }
    public virtual void SetLocalScale()
    {
        transform.localScale = _EntityData.LocalScale;
    }
    public virtual void OnUnload()
    {
        _EntityData = null;
    }


    public virtual void SetPosition()
    {
        transform.position = _EntityData.WorldPos;
    }
    public void SetParentTran()
    {
        transform.SetParent(_EntityData.ParentTran);
    }
    public virtual Vector3 GetForward()
    {
        return transform.forward;
    }
    public virtual Vector3 GetUp()
    {
        return transform.up;
    }
    public virtual Vector3 GetRight()
    {
        return transform.right;
    }
    protected virtual void Update()
    {

    }

    protected virtual void Awake()
    {

    }

    protected virtual void OnEnable()
    {

    }
    protected virtual void OnDisable()
    {

    }
}

