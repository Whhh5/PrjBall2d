

using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBall1010EntityData : GameBattleSkillBallEntityData<GameBattleSkillBall1010Entity, GameBattleSkillBall1010StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBall1010Entity;

    private GamePhysicsHit[] _Hits = new GamePhysicsHit[30];

    protected override void OnCollision(in GamePhysicsHit hit)
    {
        var starData = GetStarUserData();

        var ballAtkValue = GameAttributeUtility.GetValueInt(_EntityID, EnEntityAttributeId.Attack);
        var ownerAtk = Mathf.CeilToInt(GetOwnerEntityAttributeDamage(starData.attId) * starData.attPercentage01);
        var atkValue = ballAtkValue + ownerAtk + starData.damageValue;
        var damageResult = GameAttackMgr.Instance.AttackMonster(_OwnerEntityId, hit.entityId, atkValue, hit.point, starData.damageType);
        PlayHitAudio(in damageResult);


        ref readonly var gridLbPos = ref GameMapMgr.Instance.GetGridLbPosition();
        ref readonly var gridRuPos = ref GameMapMgr.Instance.GetGridRuPosition();
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
            var effectAtkDamage = starData.effectAtkDamage + Mathf.CeilToInt(damageResult.value * starData.effectAtkPercentage01);
            for (int i = 0; i < count; i++)
            {
                ref readonly var atkHit = ref _Hits[i];
                GameAttackMgr.Instance.AttackMonster(in _OwnerEntityId, in atkHit.entityId, in effectAtkDamage, in atkHit.point, in starData.effectAtkDamageType);
            }
        }
    }

}
public class GameBattleSkillBall1010Entity : GameBattleSkillBallEntity
{

}