public interface IClassPoolUserData : IClassPoolDestroy
{

}
public interface IClassPoolInit : IClassPool
{
    void IClassPool.PoolConstructor() { }
    void IClassPool.OnPoolEnable() { }
    void IClassPool.PoolRelease() { }
}
public interface IClassPoolInit<T> : IClassPoolInit, IClassPool<T>
    where T : IClassPoolUserData
{
}
public interface IClassPoolDestroy : IClassPoolInit
{
    void IClassPool.OnPoolInit<T>(T userData) { }
}
public interface IClassPoolNone : IClassPoolDestroy
{
    void IClassPool.OnPoolDestroy() { }
}
public interface IClassPool
{
    public void PoolConstructor();
    public void OnPoolInit<T>(T userData) where T : IClassPoolUserData;
    public void OnPoolEnable();

    public void OnPoolDestroy();

    public void PoolRelease();
}

public interface IClassPool<T> : IClassPool
    where T : IClassPoolUserData
{
    public void OnPoolInit(T userData);

    void IClassPool.OnPoolInit<T1>(T1 userData)
    {
        if (userData is not T uData)
            return;
        OnPoolInit(uData);
    }
}
public interface IClassPool2<T> : IClassPool
    where T : IClassPoolUserData
{
    public void OnPoolInit(in T userData);

}