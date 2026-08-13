using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public static class GameEntityUtility
{
	public static List<GameBattleGridIndex > GetEntityGridPosList(int monsterId, int row, int col)
	{
        var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(monsterId);
        var pos = monsterCfg.arrGridLocalPos;
        var result = new List<GameBattleGridIndex>(pos.Length);
        for (int i = 0; i < pos.Length; i += 2)
        {
            var offsetRow = pos[i];
            var offsetCol = pos[i + 1];
            var gridIndex = GameMapUtility.CreateCurMapGridIndex(row + offsetRow, col + offsetCol);
            result.Add(gridIndex);
        }
        return result;
    }
    public static void SetEntityPos(int entityId, Vector2 worldPos)
    {
        var entity = EntityMgr.Instance.GetEntityData<GameEntityData>(entityId);
        entity.SetPosition(worldPos);
    }

    public static void AddAttributeValue(in int ownerEntityId, in int entityId, in EnEntityAttributeId attId, in int value)
    {
        var pointInfo = EntityMgr.Instance.GetEntityData<IEntityGridPointInfo>(entityId);
        var targetPos = pointInfo.GetCenterPointWorldPos();
        if (attId == EnEntityAttributeId.Hp)
        {
            GameAttackMgr.Instance.RestoreHealth(ownerEntityId, entityId, value, targetPos);
        }
        else
        {
            GameAttributeUtility.AddResultValue(entityId, attId, value);
        }
    }
}

