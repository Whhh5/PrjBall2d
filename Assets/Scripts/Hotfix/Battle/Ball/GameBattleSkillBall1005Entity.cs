

using System.Collections.Generic;
using UnityEngine;

public class BattleBallEarthquakeEventData : IClassPoolDestroy
{
    public HashSet<int> targetEntityIds = new();
    public void OnPoolDestroy()
    {
        targetEntityIds.Clear();
    }
}
public class GameBattleSkillBall1005EntityData : GameBattleSkillBallEntityData<GameBattleSkillBall1005Entity, GameBattleSkillBall1005StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleSkillBall1005Entity;
    protected override void OnCollision(in GamePhysicsHit hit)
    {
        var starData = GetStarUserData();

        // 球攻击
        var ballAtkValue = GameAttributeUtility.GetValueInt(_EntityID, EnEntityAttributeId.Attack);
        var ownerAtk = Mathf.CeilToInt(GetOwnerEntityAttributeDamage(starData.attId) * starData.attPercentage01);
        var atkValue = ballAtkValue + ownerAtk + starData.damageValue;
        var damageResult = GameAttackMgr.Instance.AttackMonster(_OwnerEntityId, hit.entityId, atkValue, hit.point, starData.damageType);
        // 地震效果
        EffectAttack(in damageResult.value, in hit, in starData);

        PlayHitAudio(in damageResult);
    }
    private void EffectAttack(in int finalDamaage, in GamePhysicsHit hit, in GameBattleSkillBall1005StarData starData)
    {
        var addOwnerAtk = Mathf.CeilToInt(finalDamaage * starData.effectAtkPercentage01);
        var effectAtkDamage = starData.effectAtkDamageValue + addOwnerAtk;

        var gridPos = GameBattleGridMgr.Instance.GetGridWorldPos(hit.pointGridIndex);

        GameEffectMgr.Instance.PlayEffectOnce(EnGameEffectId.Eff_buff_dizhen, gridPos);

        var (row, col) = GameMapUtility.IndexToRowCol(hit.pointGridIndex);
        var entityIds = new List<int>(10);
        var extendGridRow = (starData.effectRowCount - 1) / 2;
        var extendGridCol = (starData.effectColCount - 1) / 2;

        for (int i = row - extendGridRow; i <= row + extendGridRow; i++)
        {
            for (int j = col - extendGridCol; j <= col + extendGridCol; j++)
            {
                if (!GameMapUtility.RowColIsValid(i, j))
                    continue;
                var index = GameMapUtility.RowColToIndex(i, j);
                GameBattleGridMgr.Instance.GetGridEntityIds(index, ref entityIds);
            }
        }
        var entityHas = new HashSet<int>(entityIds.Count);
        for (int i = 0; i < entityIds.Count; i++)
        {
            var entityId = entityIds[i];
            if (!entityHas.Add(entityId))
                continue;
            var hitPos = EntityMgr.Instance.GetEntityWorldPos2D(entityId);
            GameAttackMgr.Instance.AttackMonster( _OwnerEntityId,  entityId,  effectAtkDamage,  hitPos,  starData.effectAtkDamageType);
        }

        if (entityHas.Count > 0)
        {
            var evUd = ClassPoolMgr.Instance.Pull<BattleBallEarthquakeEventData>();
            evUd.targetEntityIds = entityHas;
            EventMgr.Instance.FireExecute(EnABBEvent.EVENT_BATTLE_BALL_EARTHQUAKE, in evUd);
            ClassPoolMgr.Instance.Push(evUd);
        }
    }


}
public class GameBattleSkillBall1005Entity : GameBattleSkillBallEntity
{

}