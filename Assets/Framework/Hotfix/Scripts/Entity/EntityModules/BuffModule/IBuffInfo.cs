
using UnityEngine;

public interface IBuffInfo<T> : IBuffInfo
    where T : class
{
    public void Execute(T buffInfo);
    void IBuffInfo.Execute(IClassPool buffInfo)
    {
        Execute(buffInfo as T);
    }
}
public interface IBuffInfo : IClassPoolInit
{
    public void Execute(IClassPool buffInfo);
}
public class BuffDefaultInfo : IBuffInfo
{
    public void Execute(IClassPool buffInfo)
    { }

    public void OnPoolDestroy()
    { }

    public void OnPoolInit<T>(T userData) where T : IClassPoolUserData
    { }
}
public interface IBuffTimeInfo
{
    public float GetTime();
}
public class BuffTimeInfo : IBuffInfo<IBuffTimeInfo>
{
    public float startTime = 0;
    public float endTime = 0;
    public float time = 0;


    public void OnPoolInit<T>(T userData) where T : IClassPoolUserData
    {
        startTime = ABBUtil.GetGameTimeSeconds();
    }

    public void Execute(IBuffTimeInfo buffInfo)
    {
        startTime = ABBUtil.GetGameTimeSeconds();
        time = Mathf.Max(time, buffInfo.GetTime());
        endTime = startTime + time;
    }

    public void OnPoolDestroy()
    {
        startTime
            = time
            = 0;
    }
}