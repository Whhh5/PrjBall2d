using UnityEngine;

public class GameBattleSkillBall10011008EntityData : GameBattleSkillBallEntityData<GameBattleSkillBall10011008Entity, GameBattleSkillBall10011008StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBall10011008Entity;

    protected override void OnCollision(in GamePhysicsHit hit)
    {
        var mapId = GameMapMgr.Instance.GetCurMapId();
        var lbPos = GameMapUtility.GetGridLbPosition(mapId);
        var ruPos = GameMapUtility.GetGridRuPosition(mapId);

        var starData = GetStarUserData();

        var ownerAtk = Mathf.CeilToInt(GetOwnerEntityAttributeDamage(starData.attId) * starData.attPercentage01);
        var atkValue = ownerAtk + starData.damageValue;
        var damageResult = GameAttackMgr.Instance.AttackMonster(_OwnerEntityId, hit.entityId, atkValue, hit.point, starData.damageType);
        PlayHitAudio(in damageResult);

        Vector2 point1 = Vector2.zero, point2 = Vector2.zero;
        var swordSystem = GameBattleMgr.Instance.GetSystem<GameBattleSwordSystem>();
        var star = GameBattleBallMgr.Instance.GetBallStar(_SkillBallId);
        var damageValue = starData.swordDamageValue + Mathf.CeilToInt(damageResult.value * starData.swordAttPercentage01);
        for (int i = 0; i < starData.swordCount; i++)
        {
            var lineAngle = Random.Range(0, Mathf.PI);

            GamePhysicsUtility.TryLineCaseRect(in lbPos, in ruPos, in hit.point, in lineAngle, ref point1, ref point2);

            if (Random.Range(0, 100) > 50)
            {
                (point2, point1) = (point1, point2);
            }

            var dir = (point2 - point1).normalized;

            point1 += -dir * 4f;
            point2 += dir * 4f;

            swordSystem.CreateSwordEntity(in _SkillBallId, in star, in damageValue, in _OwnerEntityId, in point1, in point2);

            AudioMgr.Instance.Play(EnAudioId.BallPart1011_Effect1);
        }

        var evUd = ClassPoolMgr.Instance.Pull<GameBattleBall10011008AttackEventData>();
        evUd.ownerEntityId = _OwnerEntityId;
        evUd.ballEntityId = _EntityID;
        evUd.damageValue = damageValue;
        evUd.point = hit.point;
        EventMgr.Instance.FireExecute(EnABBEvent.EVENT_BATTLE_BALLL10011008_ATTACK, 0, 0, evUd);
        ClassPoolMgr.Instance.Push(evUd);

        //AudioMgr.Instance.Play(EnAudioId.BallPart1011_Effect1);
    }
}
public class GameBattleSkillBall10011008Entity : GameBattleSkillBallEntity
{

}
