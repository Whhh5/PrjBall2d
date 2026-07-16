

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
public class GameBattleSkillBall_1005EntityData : GameBattleSkillBallEntityData<GameBattleSkillBall_1005Entity, GameBattleSkillBall_1005StarData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_BattleSkillBall_1005;
    protected override void OnCollision(in GamePhysicsHit hit)
    {
        var starData = GetStarUserData();
        // 地震效果
        EffectAttack(in hit, in starData);

        // 球攻击
        var ownerAtk = Mathf.CeilToInt(GetOwnerEntityAttackDamage() * starData.atkPercentage);
        var atkValue = ownerAtk + starData.damageValue;
        GameAttackMgr.Instance.Attack(_OwnerEntityId, hit.entityId, atkValue, hit.point, starData.damageType);
    }
    private void EffectAttack(in GamePhysicsHit hit, in GameBattleSkillBall_1005StarData starData)
    {
        var addOwnerAtk = Mathf.CeilToInt(GetOwnerEntityAttackDamage() * starData.effectAtkPercentage);
        var effectAtkDamage = starData.effectAtkDamageValue + addOwnerAtk;

        var mapId = GameMapMgr.Instance.GetCurMapId();
        var gridPos = GameBattleGridMgr.Instance.GetGridWorldPos(hit.pointGridIndex);

        GameEffectMgr.Instance.PlayEffectOnce(starData.effectId, gridPos);

        var (row, col) = GameMapUtility.IndexToRowCol(mapId, hit.pointGridIndex);
        var entityIds = new List<int>(10);
        var extendGridRow = (starData.effectRowCount - 1) / 2;
        var extendGridCol = (starData.effectColCount - 1) / 2;

        for (int i = row - extendGridRow; i <= row + extendGridRow; i++)
        {
            for (int j = col - extendGridCol; j <= col + extendGridCol; j++)
            {
                if (!GameMapUtility.RowColIsValid(mapId, i, j))
                    continue;
                var index = GameMapUtility.RowColToIndex(mapId, i, j);
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
            GameAttackMgr.Instance.Attack(in _OwnerEntityId, in entityId, in effectAtkDamage, in hitPos, in starData.effectAtkDamageType);
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
public class GameBattleSkillBall_1005Entity : GameBattleSkillBallEntity
{

}