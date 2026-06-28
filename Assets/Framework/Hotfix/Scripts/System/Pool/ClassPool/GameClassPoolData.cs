using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClassPoolData : ClassData
{
    private readonly List<IClassPool> _ListClass = new(GlobalConfig.Int5);

    public void Push(IClassPool classType)
    {
        _ListClass.Add(classType);
    }
    public bool TryPull(out IClassPool result)
    {
        result = null;
        if (_ListClass.Count == 0)
            return false;

        result = _ListClass[^1];
        _ListClass.RemoveAt(_ListClass.Count - 1);
        return true;
    }
}

