using UnityEngine;

public class GameBattleSkillBallPartEntityUserData : SceneEntityUserData
{
    public int ownerEntityId;
    public int sourceBallId;
    public int ballPartId;

    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        ownerEntityId
            = sourceBallId
            = ballPartId
            = -1;
    }
}
public abstract class GameBattleSkillBallPartEntityData<T, TStarData> : GameBattleSkillBallPartEntityData
    where T: GameBattleSkillBallPartEntity
    where TStarData : ISerializeToIntArray, new()
{
    protected T _GameSkillBallPartEntity;
    public override void OnGOCreate()
    {
        base.OnGOCreate();
        _GameSkillBallPartEntity = _Entity as T;
    }
    public override void OnGODestroy()
    {
        base.OnGODestroy();
        _GameSkillBallPartEntity = null;
    }
    protected TStarData GetBallPartStarData()
    {
        var star = GameBattleBallPartsMgr.Instance.GetPartStar(_BallPartId);
        var value = GetBallPartStarData(star);
        return value;
    }
    protected TStarData GetBallPartStarData(int star)
    {
        var value = GameBattleBallPartsMgr.Instance.GetBallPartStarData<TStarData>(_BallPartId, star);
        return value;
    }
}
public abstract class GameBattleSkillBallPartEntityData : SceneEntityData<GameBattleSkillBallPartEntity, GameBattleSkillBallPartEntityUserData>
{
    protected int _OwnerEntityId;
    protected int _SourceBallId;
    protected int _BallPartId;

    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        _OwnerEntityId
            = _SourceBallId
            = -1;
    }

    protected override void OnPoolInit(GameBattleSkillBallPartEntityUserData userData)
    {
        _OwnerEntityId = userData.ownerEntityId;
        _SourceBallId = userData.sourceBallId;
        _BallPartId = userData.ballPartId;
    }

    public override void SetPosition(in Vector3 worldPos)
    {
        var posZ = GameMapUtility.GetCurMapDepth() + (int)EnGameEntityOrderLayer.Ball;
        base.SetPosition(new Vector3(worldPos.x, worldPos.y, posZ));
    }

    public abstract void GameBallOnCollision(in int ballEntityId, in GamePhysicsHit hit);


    protected int GetOwnerEntityAttackDamage()
    {
        var ownerEntityData = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(_OwnerEntityId, EnSceneEntityComType.Attribute);
        var atkValue = ownerEntityData.GetValueInt(EnEntityAttributeId.Attack);
        return atkValue;
    }

}
public abstract class GameBattleSkillBallPartEntity : SceneEntity
{

}
