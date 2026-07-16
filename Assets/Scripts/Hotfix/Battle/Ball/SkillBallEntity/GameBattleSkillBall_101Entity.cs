

using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBall_101EntityData : GameBattleSkillBallEntityData<GameBattleSkillBall_101Entity, GameBattleSkillBall_101StarData, BattleSkillBall_101UserData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_BattleSkillBall_101;

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
        var ownerAtk = Mathf.CeilToInt(GetOwnerEntityAttackDamage() * starData.atkPercentage);
        var atkValue = ownerAtk + starData.damageValue;
        GameAttackMgr.Instance.Attack(_OwnerEntityId, hit.entityId, atkValue, hit.point, starData.damageType);

        if (++_CurBounceCount >= starData.maxCollisionCount)
        {
            GameBattleBallMgr.Instance.DestroyBallEntity(_EntityID);
        }
    }


}
public class GameBattleSkillBall_101Entity : GameBattleSkillBallEntity
{

}