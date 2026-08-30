

using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBall1002EntityData : GameBattleSkillBallEntityData<GameBattleSkillBall1002Entity, GameBattleSkillBall1002StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBall1002Entity;

    protected override void OnCollision(in GamePhysicsHit hit)
    {
        var starData = GetStarUserData();

        var probabilityTest = Random.Range(0, 100);
        if (probabilityTest < starData.fireProbability0100)
        {
            var count = Random.Range(starData.minCount, starData.maxCount + 1);
            FireSubBall(starData.subBallId, count, in hit.point, hit.dir);
        }

        var ballAtkValue = GameAttributeUtility.GetValueInt(_EntityID, EnEntityAttributeId.Attack);
        var ownerAtk = Mathf.CeilToInt(GetOwnerEntityAttributeDamage(starData.attId) * starData.attPercentage01);
        var atkValue = ballAtkValue + ownerAtk + starData.damageValue;
        var damageResult = GameAttackMgr.Instance.AttackMonster(_OwnerEntityId, hit.entityId, atkValue, hit.point, starData.damageType);

        PlayHitAudio(in damageResult);
    }

    private void FireSubBall(int ballId, int count, in Vector2 worldPos, EnDirection dir)
    {
        var normal = GetNormal(dir);

        var userData = ClassPoolMgr.Instance.Pull<BattleSkillBall_101UserData>();
        userData.star = GameBattleBallMgr.Instance.GetBallStar(_SkillBallId);
        for (int i = 0; i < count; i++)
        {
            var angle = Random.Range(-Mathf.PI, Mathf.PI) / 2 * Mathf.Rad2Deg;
            var rot = Quaternion.AngleAxis(angle, Vector3.forward);
            var v2Dir = (Vector2)(rot * normal);
            GameBattleBallMgr.Instance.CreateBallEntity(in _OwnerEntityId, in ballId, in worldPos, in v2Dir, userData);
        }
        ClassPoolMgr.Instance.Push(userData);
    }

    private Vector2 GetNormal(EnDirection dir)
    {
        return dir switch
        {
            EnDirection.Top => Vector2.up,
            EnDirection.Bottom => Vector2.down,
            EnDirection.Left => Vector2.left,
            EnDirection.Right => Vector2.right,
            _=> Vector2.zero,
        };
    }

}
public class GameBattleSkillBall1002Entity : GameBattleSkillBallEntity
{

}