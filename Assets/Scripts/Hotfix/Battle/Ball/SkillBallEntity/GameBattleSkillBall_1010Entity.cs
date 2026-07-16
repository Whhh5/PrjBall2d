

using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBall_1010EntityData : GameBattleSkillBallEntityData<GameBattleSkillBall_1010Entity, GameBattleSkillBall_1010StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_BattleSkillBall_1010;

    private GamePhysicsHit[] _Hits = new GamePhysicsHit[30];

    protected override void OnCollision(in GamePhysicsHit hit)
    {
        var starData = GetStarUserData();
        var ownerAtk = Mathf.CeilToInt(GetOwnerEntityAttackDamage() * starData.atkPercentage);
        var atkValue = ownerAtk + starData.damageValue;
        GameAttackMgr.Instance.Attack(_OwnerEntityId, hit.entityId, atkValue, hit.point, starData.damageType);


        var mapId = GameMapMgr.Instance.GetCurMapId();

        var gridLbPos = GameMapUtility.GetGridLbPosition(mapId);
        var gridRuPos = GameMapUtility.GetGridRuPosition(mapId);
        var laserPoint1 = new Vector2(hit.point.x, gridLbPos.y);
        var laserPoint2 = new Vector2(hit.point.x, gridRuPos.y);
        var effectEntityId = SceneEntityMgr.Instance.CreateEntityData<GameEffectLaserEntityData>();
        var effectEntityData = EntityMgr.Instance.GetEntityData<GameEffectLaserEntityData>(effectEntityId);
        effectEntityData.SetPoint2(laserPoint1, laserPoint2);
        effectEntityData.SetLaserWidth(starData.effectWidth);
        EntityMgr.Instance.SetEntityWorldPos(effectEntityId, hit.point);
        EntityMgr.Instance.LoadEntity(effectEntityId);

        var count = GamePhysicsUtility.TryLineCast(in laserPoint1, in laserPoint2, starData.effectWidth, ref _Hits);
        if (count > 0)
        {
            var effectAtkDamage = starData.effectAtkDamage + Mathf.CeilToInt(GetOwnerEntityAttackDamage() * starData.effectAtkPercentage);
            for (int i = 0; i < count; i++)
            {
                ref readonly var atkHit = ref _Hits[i];
                GameAttackMgr.Instance.Attack(_OwnerEntityId, atkHit.entityId, effectAtkDamage, atkHit.point, in starData.effectAtkDamageType);
            }
        }
    }

}
public class GameBattleSkillBall_1010Entity : GameBattleSkillBallEntity
{

}