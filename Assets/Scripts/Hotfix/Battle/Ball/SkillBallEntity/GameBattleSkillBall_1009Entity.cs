

using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBall_1009EntityData : GameBattleSkillBallEntityData<GameBattleSkillBall_1009Entity, GameBattleSkillBall_1009StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_BattleSkillBall_1009;

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
        var point1 = new Vector2(gridLbPos.x, hit.point.y);
        var point2 = new Vector2(gridRuPos.x, hit.point.y);
        var effectEntityId = SceneEntityMgr.Instance.CreateEntityData<GameEffectLaserEntityData>();
        var effectEntityData = EntityMgr.Instance.GetEntityData<GameEffectLaserEntityData>(effectEntityId);
        effectEntityData.SetPoint2(point1, point2);
        effectEntityData.SetLaserWidth(starData.effectWidth);
        EntityMgr.Instance.SetEntityWorldPos(effectEntityId, hit.point);
        EntityMgr.Instance.LoadEntity(effectEntityId);

        var count = GamePhysicsUtility.TryLineCast(in point1, in point2, starData.effectWidth / 2, ref _Hits);
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
public class GameBattleSkillBall_1009Entity : GameBattleSkillBallEntity
{

}