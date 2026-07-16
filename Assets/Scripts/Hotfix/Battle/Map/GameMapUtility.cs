using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameMapUtility
{
    public static Vector2 GetGridLbPosition(in int mapId)
    {
        var lbPos = GetMapStartPosition(mapId);
        var mapCfg = GameSchedule.Instance.GetMapCfg0(mapId);
        var pos = new Vector2(mapCfg.fMapBound[2], mapCfg.fMapBound[1]);
        return pos + lbPos;
    }
    public static Vector2 GetGridRuPosition(in int mapId)
    {
        var chunkLbPos = GetGridLbPosition(mapId);
        var size = GetBattleAreaBoundSize(mapId);
        return chunkLbPos + size;
    }
    public static Vector2 GetBattleAreaBoundSize(in int mapId)
    {
        var mapCfg = GameSchedule.Instance.GetMapCfg0(mapId);
        var size = new Vector3(mapCfg.fMapBound[3] - mapCfg.fMapBound[2], mapCfg.fMapBound[0] - mapCfg.fMapBound[1]);
        return size;
    }
    public static Vector2 GetMapStartPosition(in int mapId)
    {
        var mapCfg = GameSchedule.Instance.GetMapCfg0(mapId);
        var pos = new Vector2(mapCfg.fMapLbPos[0], mapCfg.fMapLbPos[1]);
        return pos;
    }
    public static float GetCurMapDepth()
    {
        var mapId = GameMapMgr.Instance.GetCurMapId();
        var depth = GetMapDepth(mapId);
        return depth;
    }
    public static float GetMapDepth(in int mapId)
    {
        var mapCfg = GameSchedule.Instance.GetMapCfg0(mapId);
        return mapCfg.nDepth;
    }
    public static Vector2 GetMapPlayerStartPosition(in int mapId)
    {
        var lbPos = GetMapStartPosition(mapId);
        var mapCfg = GameSchedule.Instance.GetMapCfg0(mapId);
        var localPos = new Vector2(mapCfg.fPlayerLocalPos[0], mapCfg.fPlayerLocalPos[1]);
        return lbPos + localPos;

    }
    public static Vector2 GetMapSize(in int mapId)
    {
        var mapCfg = GameSchedule.Instance.GetMapCfg0(mapId);
        return new Vector2(mapCfg.fMapSize[0], mapCfg.fMapSize[1]);
    }
    public static Vector2 GetMapGridSize(in int mapId)
    {
        var mapCfg = GameSchedule.Instance.GetMapCfg0(mapId);
        return new Vector2(mapCfg.nGridSize[0], mapCfg.nGridSize[1]);
    }
    public static Vector2Int GetMapGridRowCol(in int mapId)
    {
        var mapCfg = GameSchedule.Instance.GetMapCfg0(mapId);
        return new Vector2Int(mapCfg.nGridRowCol[0], mapCfg.nGridRowCol[1]);
    }
    public static Vector2Int GetCurMapGridRowCol()
    {
        var mapId = GameMapMgr.Instance.GetCurMapId();
        var result = GetMapGridRowCol(mapId);
        return result;
    }
    public static bool RowColIsValid(int mapId, int row, int col)
    {
        var maxRowCol = GetMapGridRowCol(mapId);
        if (row < 0 || row >= maxRowCol.x)
            return false;
        if (col < 0 || col >= maxRowCol.y)
            return false;
        return true;
    }
    public static bool IndexIsValid(int mapId, int index)
    {
        var maxRowCol = GetMapGridRowCol(mapId);
        if (index < 0)
            return false;
        if (index >= maxRowCol.x * maxRowCol.y)
            return false;
        return true;
    }
    public static int RowColToIndex(in int mapId, in int row, in int col)
    {
        var maxRowCol = GetMapGridRowCol(mapId);
        return row * maxRowCol.y + col;
    }
    public static (int row, int col) IndexToRowCol(int mapId, int index)
    {
        var maxRowCol = GetMapGridRowCol(mapId);
        var row = Mathf.FloorToInt(index / maxRowCol.y);
        var col = Mathf.FloorToInt(index % maxRowCol.y);
        return (Mathf.Clamp(row, 0, maxRowCol.x - 1), col);
    }

    public static bool IsValidWorldPos(in int mapId, in Vector2 worldPos)
    {
        var lbPos = GetGridLbPosition(mapId);
        var ruPos = GetGridRuPosition(mapId);
        if (worldPos.x < lbPos.x || worldPos.x > ruPos.x)
            return false;
        if (worldPos.y < lbPos.y || worldPos.y > ruPos.y)
            return false;
        return true;
    }
    #region grid index
    public static int WorldPosY2Row(in int mapId, in float posY)
    {
        var startPos = GetGridLbPosition(mapId);
        var size = GetMapGridSize(mapId);
        var localPosY = posY - startPos.y;
        var row = Mathf.FloorToInt(localPosY / size.y);
        return Mathf.Max(row, 0);
    }
    public static int WorldPosToGridIndex(int mapId, Vector2 worldPos)
    {
        var startPos = GetGridLbPosition(mapId);
        var size = GetMapGridSize(mapId);
        var localPos = worldPos - startPos;
        var row = Mathf.FloorToInt(localPos.y / size.y);
        var col = Mathf.FloorToInt(localPos.x / size.x);
        var maxRowCol = GetMapGridRowCol(mapId);
        var index = Mathf.Clamp(row, 0, maxRowCol.x - 1) * maxRowCol.y + Mathf.Clamp(col, 0, maxRowCol.y - 1);
        return index;
    }
    public static GameBattleGridIndex CreateGridIndex(int mapId, int index)
    {
        var mapCfg = GameSchedule.Instance.GetMapCfg0(mapId);
        var col = mapCfg.nGridRowCol[1];
        return new GameBattleGridIndex(index, Mathf.FloorToInt(index / col), index % col);
    }
    public static GameBattleGridIndex CreateGridIndex(int mapId, int row, int col)
    {
        var mapCfg = GameSchedule.Instance.GetMapCfg0(mapId);
        var maxCol = mapCfg.nGridRowCol[1];
        var index = row * maxCol + col;
        return new GameBattleGridIndex(index, row, col);
    }

    public static GameBattleGridIndex CreateCurMapGridIndex(int index)
    {
        var mapId = GameMapMgr.Instance.GetCurMapId();
        return GameMapUtility.CreateGridIndex(mapId, index);
    }
    public static GameBattleGridIndex CreateCurMapGridIndex(int row, int col)
    {
        var mapId = GameMapMgr.Instance.GetCurMapId();
        return GameMapUtility.CreateGridIndex(mapId, row, col);
    }
    #endregion


    public static bool GridsIsIdle(List<GameBattleGridIndex> gridIndexs)
    {
        for (int i = 0; i < gridIndexs.Count; i++)
        {
            var index = gridIndexs[i];
            if (!GameBattleGridMgr.Instance.GridStatusIs(index.index, EnGridStatus.Idle))
                return false;
        }
        return true;
    }

    public static int CreateMonsterEntity(int monsterId, GameBattleGridIndex gridIndex)
    {
        var monsterEntityId = MonsterUtility.CreateMonster(monsterId);
        return monsterEntityId;
    }

    public static void CheckMoveBoundsTopY(ref float worldPosY)
    {
        var mapId = GameMapMgr.Instance.GetCurMapId();
        var gridSize = GetMapGridSize(mapId);
        CheckMoveBoundsY(ref worldPosY, -gridSize.y / 2, gridSize.y / 2);
    }
    public static void CheckMoveBoundsY(ref float worldPosY, in float offsetTop, in float offsetDown)
    {
        var mapId = GameMapMgr.Instance.GetCurMapId();
        var lbPos = GameMapUtility.GetGridLbPosition(mapId);
        var ruPos = GameMapUtility.GetGridRuPosition(mapId);

        var moveLbBoundY = lbPos.y + offsetDown;
        var moveRuBoundY = ruPos.y + offsetTop;
        if (worldPosY > moveRuBoundY || worldPosY < moveLbBoundY)
        {
            worldPosY = Mathf.Clamp(worldPosY, moveLbBoundY, moveRuBoundY);
        }
    }
    public static void CheckMoveBounds(ref Vector2 worldPos, in Vector2 halfSize)
    {
        var mapId = GameMapMgr.Instance.GetCurMapId();
        var lbPos = GameMapUtility.GetGridLbPosition(mapId);
        var ruPos = GameMapUtility.GetGridRuPosition(mapId);

        var moveLbBound = lbPos + halfSize;
        var moveRuBound = ruPos + halfSize;
        if (worldPos.x > moveRuBound.x || worldPos.x < moveLbBound.x)
        {
            worldPos.x = Mathf.Clamp(worldPos.x, moveLbBound.x, moveRuBound.x);
        }
        if (worldPos.y > moveRuBound.y || worldPos.y < moveLbBound.y)
        {
            worldPos.y = Mathf.Clamp(worldPos.y, moveLbBound.y, moveRuBound.y);
        }
    }

    public static int GetMapEagePoint(in int mapId, in Vector2 startPoint, in Vector2 dir, in EnDirection validEage, ref GamePhysicsHit[] points)
    {
        var count = GetMapEagePoint(in mapId, in startPoint, in dir, in validEage, ref points, points.Length);
        return count;
    }
    public static int GetMapEagePoint(in int mapId, in Vector2 startPoint, in Vector2 dir, in EnDirection validEage, ref GamePhysicsHit[] points, in int maxCount)
    {
        var lbPos = GetGridLbPosition(mapId);
        var ruPos = GetGridRuPosition(mapId);
        var count = GamePhysicsUtility.CastBoxEage(in lbPos, ruPos, in startPoint, in dir, in validEage, ref points, maxCount);
        return  count;
    }
}

