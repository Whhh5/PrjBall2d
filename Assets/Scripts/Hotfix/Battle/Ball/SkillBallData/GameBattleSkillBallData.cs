
using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBallUserData : IClassPoolDestroy
{
    public int ballId;
    public void OnPoolDestroy()
    {
        ballId
            = -1;
    }
}

public abstract class GameBallData<TEntityData, TStarData> : GameBattleSkillBallData
    where TEntityData : GameBattleSkillBallEntityData, new()
    where TStarData : GameBattleSkillBallStarData, new()
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
    public sealed override GameBattleSkillBallStarData GetStarData(in int star)
    {
        if (!_StarDatas.TryGetValue(star, out var starData))
        {
            var userData = ClassPoolMgr.Instance.Pull<CommonSerializeToIntArrayUserData>();
            starData = GameBattleBallMgr.Instance.CreateStarData<TStarData>(_BallId, star, userData);
            ClassPoolMgr.Instance.Push(userData);
            _StarDatas.Add(star, starData);
        }
        return starData;
    }

    protected override int CreateBallEntityDataInner(in int ownerEntityId, in int ballId, GameBattleSkillBallEntityUserData userData)
    {
        userData.skillBallId = ballId;
        userData.ownerEntityId = ownerEntityId;
        var entityId = SceneEntityMgr.Instance.CreateEntityData<TEntityData>(userData);
        return entityId;
    }
}
public abstract class GameBattleSkillBallData : IClassPoolInit<GameBattleSkillBallUserData>
{
    protected int _BallId;

    public virtual void OnPoolDestroy()
    {
        _BallId
            = 0;
    }

    public virtual void OnPoolInit(GameBattleSkillBallUserData userData)
    {
        _BallId = userData.ballId;
    }
    public abstract GameBattleSkillBallStarData GetStarData(in int star);
    protected abstract int CreateBallEntityDataInner(in int ownerEntityId, in int ballId, GameBattleSkillBallEntityUserData userData);
    public int CreateBallEntity(in int ownerEntityId, in int ballId, in Vector2 pos, GameBattleSkillBallEntityUserData userData)
    {
        var entityId = CreateBallEntityDataInner(in ownerEntityId, in ballId, userData);

        SceneEntityMgr.Instance.AddCom(entityId, EnSceneEntityComType.Attribute);

        EntityMgr.Instance.SetEntityWorldPos(entityId, pos);
        EntityMgr.Instance.LoadEntity(entityId);
        return entityId;
    }
    public void DestroyBallEntity(in int entityId)
    {
        SceneEntityMgr.Instance.RemoveCom(entityId, EnSceneEntityComType.Attribute);
        SceneEntityMgr.Instance.DestroyEntityData(entityId);
    }
}