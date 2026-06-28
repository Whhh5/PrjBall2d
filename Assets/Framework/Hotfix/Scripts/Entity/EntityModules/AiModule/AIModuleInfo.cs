
using System.Collections.Generic;

public class AIModuleInfo : IClassPoolDestroy
{
    public IAIModule aiModule = null;
    private List<int> _EntityIDList = new(5);
    public int Count => _EntityIDList.Count;

    public void AddEntityID(int entityID)
    {
        _EntityIDList.Add(entityID);
    }
    public void RemoveEntityID(int entityID)
    {
        var index = _EntityIDList.IndexOf(entityID);
        if (index < 0)
            return;
        _EntityIDList[index] = _EntityIDList[^1];
        _EntityIDList.Remove(Count - 1);
    }
    public int GetEntityIDByIndex(int index)
    {
        return _EntityIDList[index];
    }
    public void OnPoolDestroy()
    {
        aiModule = null;
        _EntityIDList.Clear();
    }
}