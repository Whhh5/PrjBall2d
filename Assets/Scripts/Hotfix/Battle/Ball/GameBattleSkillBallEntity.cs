using System.Collections.Generic;
using UnityEngine;



public class GameBattleSkillBallEntityUserData : SceneEntityUserData
{
    public int skillBallId;
    public int ownerEntityId;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        skillBallId
            = ownerEntityId
            = -1;
    }
}
public interface IGameBallOnCollisionCallback
{
    public void GameBallOnCollision(in int ballId, in int entityId, in GamePhysicsHit hit);
}
public abstract class GameBattleSkillBallEntityData : SceneEntityData<GameBattleSkillBallEntity, GameBattleSkillBallEntityUserData>
{
    protected int _SkillBallId;
    protected int _OwnerEntityId;
    private float _BallRadius;

    public override void Destroy()
    {
        base.Destroy();
        _SkillBallId = -1;
        _BallRadius = -1;
    }
    public int GetSkillBallId()
    {
        return _SkillBallId;
    }
    protected override void OnPoolInit(GameBattleSkillBallEntityUserData userData)
    {
        _OwnerEntityId = userData.ownerEntityId;
        _SkillBallId = userData.skillBallId;

        var cfg = GameSchedule.Instance.GetSkillBallCfg0(userData.skillBallId);
        _BallRadius = cfg.fRadius;
    }
    public int GetOwnerEntityId()
    {
        return _OwnerEntityId;
    }
    public override void SetPosition(in Vector3 worldPos)
    {
        var posZ = GameMapMgr.Instance.GetLayerDepth(EnGameEntityOrderLayer.Ball);
        base.SetPosition(new Vector3(worldPos.x, worldPos.y, posZ));
    }
    public float GetSkillBallRadius()
    {
        return _BallRadius;
    }
    public virtual void Collision(in GamePhysicsHit hit)
    {
        // 碰撞状态
        GameMonsterUtility.OnCollision(hit.entityId, hit.dir);
        // 碰撞特效
        GameBattleBallMgr.Instance.OnCollision(_EntityID, hit.entityId, hit.point);

        var eventUserData = ClassPoolMgr.Instance.Pull<GameSkillBallOnCollisionEventData>();
        eventUserData.hit = hit;
        eventUserData.entityId = _EntityID;
        EventMgr.Instance.FireExecute(EnABBEvent.EVENT_SKILL_BALL_ONCOLLISION, _SkillBallId, _EntityID, eventUserData);
        ClassPoolMgr.Instance.Push(eventUserData);

        OnCollision(in hit);
    }
    protected abstract void OnCollision(in GamePhysicsHit hit);

    protected int GetOwnerEntityAttributeDamage(EnEntityAttributeId attId)
    {
        var ownerEntityData = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(_OwnerEntityId, EnSceneEntityComType.Attribute);
        var atkValue = ownerEntityData.GetValueInt(attId);
        return atkValue;
    }
    protected void PlayHitAudio(in AttackDamageResult damageResult)
    {
        var ballCfg = GameSchedule.Instance.GetSkillBallCfg0(_SkillBallId);
        var audioId = damageResult.valueType switch
        {
            EnDamageValueType.Critical => ballCfg.nHitCritAudioId,
            _ => ballCfg.nHitAudioId
        };
        AudioMgr.Instance.Play(audioId);
    }
}
public abstract class GameBattleSkillBallEntityData<T, TStarData, TUserData> : GameBattleSkillBallEntityData<T, TStarData>
    where T : GameBattleSkillBallEntity
    where TStarData : GameBattleSkillBallStarData, new()
    where TUserData : GameBattleSkillBallEntityUserData
{
    protected sealed override void OnPoolInit(GameBattleSkillBallEntityUserData userData)
    {
        base.OnPoolInit(userData);
        OnPoolInit((TUserData)userData);
    }

    protected abstract void OnPoolInit(TUserData userData);
}
public abstract class GameBattleSkillBallEntityData<T, TStarData> : GameBattleSkillBallEntityData
    where T : GameBattleSkillBallEntity
    where TStarData : GameBattleSkillBallStarData, new()
{
    protected T _GameBattleSkillBallEntity;

    public override void OnGOCreate(GameEntity entity)
    {
        base.OnGOCreate(entity);
        _GameBattleSkillBallEntity = entity as T;
    }
    public override void OnGODestroy()
    {
        base.OnGODestroy();
        _GameBattleSkillBallEntity = null;
    }
    protected TStarData GetStarUserData()
    {
        var value = GameBattleBallMgr.Instance.GetBallStarData<TStarData>(_SkillBallId);
        return value;
    }
    protected TStarData GetStarUserData(int star)
    {
        var value = GameBattleBallMgr.Instance.GetBallStarData<TStarData>(_SkillBallId, star);
        return value;
    }
}
public abstract class GameBattleSkillBallEntityData<T> : GameBattleSkillBallEntityData
    where T : GameBattleSkillBallEntity
{
    public T GetEntity()
    {
        return _Entity as T;
    }
}
public abstract class GameBattleSkillBallEntity : SceneEntity
{

}
