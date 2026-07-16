using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;


public class GameBattleSkillBallPartUserData : IClassPoolDestroy
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
public abstract class GameBattleSkillBallPartData<TEnttiyData, TStarData> : GameBattleSkillBallPartData
    where TEnttiyData : GameBattleSkillBallPartEntityData, new()
    where TStarData : GameBattleSkillBallPartStarData, new()
{
    private readonly Dictionary<int, TStarData> _StarDatas = new(3);

    public override void OnPoolDestroy()
    {
        foreach (var (_, data) in _StarDatas)
        {
            ClassPoolMgr.Instance.Push(data);
        }
        base.OnPoolDestroy();
        _StarDatas.Clear();
    }
    public override GameBattleSkillBallPartStarData GetStarData(in int star)
    {
        if (!_StarDatas.TryGetValue(star, out var starData))
        {
            var userData = ClassPoolMgr.Instance.Pull<CommonSerializeToIntArrayUserData>();
            starData = GameBattleBallPartsMgr.Instance.CreatePartStarData<TStarData>(_PartId, star, userData);
            ClassPoolMgr.Instance.Push(userData);
            _StarDatas.Add(star, starData);
        }
        return starData;
    }

    protected override int CreatePartEntityDataInner(int ownerEntityId, int ballId)
    {
        var userData = ClassPoolMgr.Instance.Pull<GameBattleSkillBallPartEntityUserData>();
        userData.ownerEntityId = ownerEntityId;
        userData.sourceBallId = ballId;
        userData.ballPartId = _PartId;
        var entityId = SceneEntityMgr.Instance.CreateEntityData<TEnttiyData>(userData);
        ClassPoolMgr.Instance.Push(userData);
        return entityId;
    }
}
public abstract class GameBattleSkillBallPartData : IClassPoolInit<GameBattleSkillBallPartUserData>
{
    protected int _Star;
    protected List<int> _PartEntityIds = new();
    protected int _PosIndex;
    protected int _PartId;
    public virtual void OnPoolDestroy()
    {
        foreach (var entityId in _PartEntityIds)
        {
            SceneEntityMgr.Instance.RemoveCom(entityId, EnSceneEntityComType.Attribute);
            SceneEntityMgr.Instance.DestroyEntityData(entityId);
        }
        _PosIndex
            = _Star
            = _PartId
            = -1;
        _PartEntityIds.Clear();
    }

    public virtual void OnPoolInit(GameBattleSkillBallPartUserData userData)
    {
        _Star = userData.star;
        _PartId = userData.partId;
        _PosIndex = userData.posIndex;
    }

    public abstract GameBattleSkillBallPartStarData GetStarData(in int star);
    protected abstract int CreatePartEntityDataInner(int ownerEntityId, int ballId);
    public int CreatePartEntity(int ownerEntityId, int ballId, in Vector2 worldPos)
    {
        var entityId = CreatePartEntityDataInner(ownerEntityId, ballId);
        SceneEntityMgr.Instance.AddCom(entityId, EnSceneEntityComType.Attribute);

        EntityMgr.Instance.SetEntityWorldPos(entityId, worldPos);
        EntityMgr.Instance.LoadEntity(entityId);
        _PartEntityIds.Add(entityId);
        return entityId;
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
    public void DestroyPartEntity(int entityId)
    {
        if (!_PartEntityIds.Remove(entityId))
            return;
        SceneEntityMgr.Instance.RemoveCom(entityId, EnSceneEntityComType.Attribute);
        SceneEntityMgr.Instance.DestroyEntityData(entityId);
    }
}