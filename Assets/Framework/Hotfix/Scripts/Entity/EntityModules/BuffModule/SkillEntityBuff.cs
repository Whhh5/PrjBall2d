using System.Collections.Generic;


public interface ISkillEntityBuff : IClassPoolInit<EntityBuffDataUserData>
{
    public EnBuff GetBuff();
    public void OnEnable(int addKey, IEntityBuffParams buffParams);
    public bool OnDisable(int addKey);
    public void ReOnEnable(int addKey, IEntityBuffParams buffParams);
    public bool IsRemove();
}
public abstract class SkillEntityBuff<T> : SkillEntityBuff
    where T: class, IEntityBuffParams
{
    public sealed override void OnEnable(int addKey, IEntityBuffParams buffParams)
    {
        base.OnEnable(addKey, buffParams);

        OnEnable(addKey, buffParams as T);
    }
    public sealed override void ReOnEnable(int addKey, IEntityBuffParams buffParams)
    {
        base.ReOnEnable(addKey, buffParams);

        ReOnEnable(addKey, buffParams as T);
    }
    protected virtual void OnEnable(int addKey, T buffParams)
    {
        
    }
    protected virtual void ReOnEnable(int addKey, T buffParams)
    {
        
    }
}
public abstract class SkillEntityBuff : ISkillEntityBuff
{
    public int Count => _AddKey.Count;
    protected int _TargetEntityID = GlobalConfig.IntM1;
    protected int _SourceEntityID = GlobalConfig.IntM1;
    private EnBuff _Buff = EnBuff.None;

    private readonly HashSet<int> _AddKey = new();
    public virtual void OnPoolDestroy()
    {
        _Buff = EnBuff.None;
        _TargetEntityID
            = GlobalConfig.IntM1;
        _AddKey.Clear();
    }

    public virtual void OnPoolInit(EntityBuffDataUserData userData)
    {
        _Buff = userData.buff;
        _TargetEntityID = userData.targetEntityID;
        _SourceEntityID = userData.sourceEntityID;
    }
    public EnBuff GetBuff()
    {
        return _Buff;
    }

    public virtual void OnEnable(int addKey, IEntityBuffParams buffParams)
    {
        _AddKey.Add(addKey);
    }
    public virtual bool OnDisable(int addKey)
    {
        var result = _AddKey.Remove(addKey);
        return result;
    }
    public bool IsRemove()
    {
        return Count == 0;
    }
    public virtual void ReOnEnable(int addKey, IEntityBuffParams buffParams)
    {
        _AddKey.Add(addKey);
    }
}