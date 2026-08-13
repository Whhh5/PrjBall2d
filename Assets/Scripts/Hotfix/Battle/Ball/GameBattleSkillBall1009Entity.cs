

using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBall1009EntityData : GameBattleSkillBallEntityData<GameBattleSkillBall1009Entity, GameBattleSkillBall1009StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBall1009Entity;

    private GamePhysicsHit[] _Hits = new GamePhysicsHit[30];

    protected override void OnCollision(in GamePhysicsHit hit)
    {
        var starData = GetStarUserData();
        var ownerAtk = Mathf.CeilToInt(GetOwnerEntityAttributeDamage(starData.attId) * starData.attPercentage01);
        var atkValue = ownerAtk + starData.damageValue;
        var damageResult = GameAttackMgr.Instance.AttackMonster(_OwnerEntityId, hit.entityId, atkValue, hit.point, starData.damageType);
        PlayHitAudio(in damageResult);

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
            var effectAtkDamage = starData.effectAtkDamage + Mathf.CeilToInt(damageResult.value * starData.effectAtkPercentage01);
            for (int i = 0; i < count; i++)
            {
                ref readonly var atkHit = ref _Hits[i];
                GameAttackMgr.Instance.AttackMonster(_OwnerEntityId, atkHit.entityId, effectAtkDamage, atkHit.point, starData.effectAtkDamageType);
            }
        }
    }

}
public class GameBattleSkillBall1009Entity : GameBattleSkillBallEntity
{

}