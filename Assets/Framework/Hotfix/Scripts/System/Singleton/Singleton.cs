using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;

public enum EnManagerFuncType
{
    None = 0,
    AwakeAsync = 1 << 0,
    OnEnableAsync = 1 << 1,
    Update = 1 << 2,
    FixedUpdate = 1 << 3,
}
public enum EnSingletonOrder
{
    None,
    Load,
    GameSchedule,
    Server,
    Base,
    Default,
}
public abstract class Singleton
{
    public virtual EnSingletonOrder SingletonOrder => EnSingletonOrder.Default;
    public virtual EnManagerFuncType FuncType => EnManagerFuncType.AwakeAsync | EnManagerFuncType.OnEnableAsync;
    public virtual async UniTask AwakeAsync()
    {
        await UniTask.DelayFrame(1);
    }
    public virtual async UniTask OnEnableAsync()
    {
        await UniTask.DelayFrame(1);
    }
    public virtual void OnDisable()
    {

    }
    public virtual void Destroy()
    {

    }
    public virtual void Update()
    {

    }
    public virtual void FixedUpdate()
    {

    }
}
public abstract class Singleton<T> : Singleton
    where T : class, new()
{
    public static T Instance = new();
}

