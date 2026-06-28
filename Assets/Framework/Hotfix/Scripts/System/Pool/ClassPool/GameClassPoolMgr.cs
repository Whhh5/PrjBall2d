using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public sealed class ClassPoolMgr : Singleton<ClassPoolMgr>
{
    private readonly Dictionary<Type, ClassPoolData> _DicClassPool = new();
    private ClassPoolData GetClassPoolData(Type type)
    {
        if (!_DicClassPool.TryGetValue(type, out var poolData))
        {
            poolData = new ClassPoolData();
            _DicClassPool.Add(type, poolData);
        }
        return poolData;
    }

    public T Pull<T>()
        where T : IClassPool, new()
    {
        var data = Pull<T>(null);
        return data;
    }
    public T Pull<T>(IClassPoolUserData userData)
        where T : IClassPool, new()
    {
        var type = typeof(T);
        var poolData = GetClassPoolData(type);
        if (!poolData.TryPull(out var data))
        {
            data = new T();
            data.PoolConstructor();
        }
        data.OnPoolInit(userData);
        return (T)data;
    }
    public void Push<T>(in T classData)
        where T : IClassPool
    {
        if (classData == null)
        {
            ABBUtil.LogWarring($"警告 ({typeof(T)})，回收一个 null 引用");
            return;
        }
        var type = classData.GetType();
        classData.OnPoolDestroy();
        var poolData = GetClassPoolData(type);
        poolData.Push(classData);
    }
    
}

