using UnityEngine;

public class GameBattleSkillBall10091010EntityData : GameBattleSkillBallEntityData<GameBattleSkillBall10091010Entity, GameBattleSkillBall10091010StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBall10091010Entity;

    private GamePhysicsHit[] _Hits = new GamePhysicsHit[30];
    protected override void OnCollision(in GamePhysicsHit hit)
    {
        var starData = GetStarUserData();

        var ballAtkValue = GameAttributeUtility.GetValueInt(_EntityID, EnEntityAttributeId.Attack);
        var ownerAtk = Mathf.CeilToInt(GetOwnerEntityAttributeDamage(starData.attId) * starData.attPercentage01);
        var atkValue = ballAtkValue + ownerAtk + starData.damageValue;
        var damageResult = GameAttackMgr.Instance.AttackMonster(_OwnerEntityId, hit.entityId, atkValue, hit.point, starData.damageType);
        PlayHitAudio(in damageResult);

        var laserAtkValue = starData.laserDamageValue + Mathf.CeilToInt(damageResult.value * starData.laserAttPercentage01);

        ref readonly var gridLbPos = ref GameMapMgr.Instance.GetGridLbPosition();
        ref readonly var gridRuPos = ref GameMapMgr.Instance.GetGridRuPosition();
        {
            var point1 = new Vector2(gridLbPos.x, hit.point.y);
            var point2 = new Vector2(gridRuPos.x, hit.point.y);
            var effectEntityId = SceneEntityMgr.Instance.CreateEntityData<GameEffectLaserEntityData>();
            var effectEntityData = EntityMgr.Instance.GetEntityData<GameEffectLaserEntityData>(effectEntityId);
            effectEntityData.SetPoint2(point1, point2);
            effectEntityData.SetLaserWidth(starData.width);
            EntityMgr.Instance.SetEntityWorldPos(effectEntityId, hit.point);
            EntityMgr.Instance.LoadEntity(effectEntityId);

            var count = GamePhysicsUtility.TryLineCast(in point1, in point2, starData.width / 2, ref _Hits);
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    ref readonly var atkHit = ref _Hits[i];
                    GameAttackMgr.Instance.AttackMonster(_OwnerEntityId, atkHit.entityId, laserAtkValue, atkHit.point, starData.laserDamageType);
                }
            }
        }
        {
            var laserPoint1 = new Vector2(hit.point.x, gridLbPos.y);
            var laserPoint2 = new Vector2(hit.point.x, gridRuPos.y);
            var effectEntityId = SceneEntityMgr.Instance.CreateEntityData<GameEffectLaserEntityData>();
            var effectEntityData = EntityMgr.Instance.GetEntityData<GameEffectLaserEntityData>(effectEntityId);
            effectEntityData.SetPoint2(laserPoint1, laserPoint2);
            effectEntityData.SetLaserWidth(starData.width);
            EntityMgr.Instance.SetEntityWorldPos(effectEntityId, hit.point);
            EntityMgr.Instance.LoadEntity(effectEntityId);

            var count = GamePhysicsUtility.TryLineCast(in laserPoint1, in laserPoint2, starData.width, ref _Hits);
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    ref readonly var atkHit = ref _Hits[i];
                    GameAttackMgr.Instance.AttackMonster(in _OwnerEntityId, in atkHit.entityId, in laserAtkValue, in atkHit.point, in starData.laserDamageType);
                }
            }
        }

        var evUd = ClassPoolMgr.Instance.Pull<GameBattleBall10091010AttackEventData>();
        evUd.damageType = starData.laserDamageType;
        evUd.width = starData.width;
        evUd.damageValue = laserAtkValue;
        evUd.ballEntityId = _EntityID;
        EventMgr.Instance.FireExecute(EnABBEvent.EVENT_BATTLE_BALLL10091010_ATTACK, evUd);
        ClassPoolMgr.Instance.Push(evUd);
    }
}
public class GameBattleSkillBall10091010Entity : GameBattleSkillBallEntity
{

}
