using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;


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
            var userData = ClassPoolMgr.Instance.Pull<GameBattleSkillBallPartStarUserData>();
            userData.partId = _PartId;
            userData.star = star;
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
public abstract class GameBattleSkillBallPartData : IClassPoolDestroy
{
    protected int _PartId;
    public abstract GameBattleSkillBallPartStarData GetStarData(in int star);
    protected abstract int CreatePartEntityDataInner(int ownerEntityId, int ballId);
    public int CreatePartEntity(int ownerEntityId, int ballId, in Vector2 worldPos)
    {
        var entityId = CreatePartEntityDataInner(ownerEntityId, ballId);
        SceneEntityMgr.Instance.AddCom(entityId, EnSceneEntityComType.Attribute);

        EntityMgr.Instance.SetEntityWorldPos(entityId, worldPos);
        EntityMgr.Instance.LoadEntity(entityId);

        return entityId;
    }

    public void DestroyPartEntity(int entityId)
    {
        SceneEntityMgr.Instance.RemoveCom(entityId, EnSceneEntityComType.Attribute);
        SceneEntityMgr.Instance.DestroyEntityData(entityId);
    }

    public void SetPartId(int partId)
    {
        _PartId = partId;
    }

    public virtual void OnPoolDestroy()
    {
    }
}