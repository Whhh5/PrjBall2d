

using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBall101EntityData : GameBattleSkillBallEntityData<GameBattleSkillBall101Entity, GameBattleSkillBall101StarData, BattleSkillBall_101UserData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBall101Entity;

    private int _Star = -1;
    private int _CurBounceCount = 0;
    public override void Destroy()
    {
        base.Destroy();
        _Star = -1;
        _CurBounceCount = 0;
    }
    protected override void OnPoolInit(BattleSkillBall_101UserData userData)
    {
        _Star = userData.star;
    }
    protected override void OnCollision(in GamePhysicsHit hit)
    {
        var starData = GetStarUserData(_Star);

        var ballAtkValue = GameAttributeUtility.GetValueInt(_EntityID, EnEntityAttributeId.Attack);
        var ownerAtk = Mathf.CeilToInt(GetOwnerEntityAttributeDamage(starData.attId) * starData.attPercentage);
        var atkValue = ballAtkValue + ownerAtk + starData.damageValue;
        var damageResult = GameAttackMgr.Instance.AttackMonster(_OwnerEntityId, hit.entityId, atkValue, hit.point, starData.damageType);
        PlayHitAudio(in damageResult);

        if (++_CurBounceCount >= starData.maxCollisionCount)
        {
            GameBattleBallMgr.Instance.DestroyBallEntity(_EntityID);
        }
    }


}
public class GameBattleSkillBall101Entity : GameBattleSkillBallEntity
{

}