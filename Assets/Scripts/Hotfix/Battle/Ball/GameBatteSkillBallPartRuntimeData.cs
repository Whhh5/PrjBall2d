
using System.Collections.Generic;

public class GameBattleSkillBallPartRuntimeUserData : IClassPoolDestroy
{
    public int partId;
    public int star;
    public int posIndex;
    public void OnPoolDestroy()
    {
        posIndex
            = star
            = partId
            = -1;
    }
}
public class GameBatteSkillBallPartRuntimeData : IClassPoolInit<GameBattleSkillBallPartRuntimeUserData>
{
    protected int _Star;
    protected List<int> _PartEntityIds = new();
    protected int _PosIndex;
    protected int _PartId;

    public virtual void OnPoolDestroy()
    {
        _PosIndex
            = _Star
            = _PartId
            = -1;
        _PartEntityIds.Clear();
    }

    public virtual void OnPoolInit(GameBattleSkillBallPartRuntimeUserData userData)
    {
        _Star = userData.star;
        _PartId = userData.partId;
        _PosIndex = userData.posIndex;
    }

    public void SetStar(in int star)
    {
        _Star = star;
    }
    public int GetStar()
    {
        return _Star;
    }
    public void SetPosIndex(int value)
    {
        _PosIndex = value;
    }
    public int GetPosIndex()
    {
        return _PosIndex;
    }
    public bool ContainsEntity()
    {
        return _PartEntityIds.Count > 0;
    }
    public int GetEntityCount()
    {
        return _PartEntityIds.Count;
    }
    public int GetPartEntityId(int index)
    {
        return _PartEntityIds[index];
    }
    public void AddEntityId(int entityId)
    {
        _PartEntityIds.Add(entityId);
    }
    public void RemoveEntityId(int entityId)
    {
        _PartEntityIds.Remove(entityId);
    }
}