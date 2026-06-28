public enum EnSceneEntityComType
{
    None,
    Life,
    Buff,
    AI,
    Animation,
    Camera,
    CharacterController,
    Rigidbody,
    Property,
    Cmd,
}
public interface IEntity3DComData : IClassPool
{
    public EnSceneEntityComType GetComType();
    public void OnCreateGO();
    public void OnDestroyGO();
    public bool IsCanActive();
    public bool IsActive();
    public void OnEnable();
    public void OnDisable();
}
public interface IEntity3DComData<T> : IEntity3DComData, IClassPoolInit<T>
    where T : class, IClassPoolUserData
{

}


public interface IEntity3DCom
{

}

public abstract class Entity3DComDataGO<TGOCom> : Entity3DComData
    where TGOCom : class, IEntity3DCom
{
    protected TGOCom _GoCom;
    public override void OnDestroyGO()
    {
        base.OnDestroyGO();
        _GoCom = null;
    }
    public override void OnCreateGO()
    {
        base.OnCreateGO();
        _GoCom = EntityMgr.Instance.GetEntityComponent<TGOCom>(_EntityId);
    }
}
public abstract class Entity3DComData : Entity3DComData<Entity3DComDataUserData>
{

}
public abstract class Entity3DComData<T> : IEntity3DComData<T>
    where T : Entity3DComDataUserData
{
    public abstract EnSceneEntityComType GetComType();
    private bool _IsActive = false;
    protected int _EntityId = -1;

    public virtual void OnPoolDestroy()
    {
        _EntityId = -1;
        _IsActive = false;
    }

    public virtual void OnPoolInit(T userData)
    {
        _EntityId = userData.entityID;
    }
    public bool IsActive()
    {
        return _IsActive;
    }

    public virtual void OnCreateGO()
    {

    }

    public virtual void OnDestroyGO()
    {

    }

    public virtual void OnDisable()
    {
        _IsActive = false;
    }

    public virtual void OnEnable()
    {
        _IsActive = true;
    }

    public bool IsCanActive()
    {
        return !_IsActive;
    }
}