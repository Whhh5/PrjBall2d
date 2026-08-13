
using System.Collections.Generic;

public class GameBattleSkillBallRuntimeData : IClassPoolDestroy
{
    private int _Star;
    private readonly List<int> _EntityIds = new();

    public void OnPoolDestroy()
    {
        _EntityIds.Clear();
        _Star = 0;
    }
    public int GetStar()
    {
        return _Star;
    }

    public void SetStar(in int star)
    {
        _Star = star;
    }

    public int GetEntityCount()
    {
        return _EntityIds.Count;
    }
    public int GetEntity(int index)
    {
        return _EntityIds[index];
    }
    public void AddEntityId(in int entityId)
    {
        _EntityIds.Add(entityId);
    }
    public void RemoveEntityId(in int entityId)
    {
        _EntityIds.Remove(entityId);
    }
}