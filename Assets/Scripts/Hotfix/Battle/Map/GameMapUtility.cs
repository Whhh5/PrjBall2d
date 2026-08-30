using System;
using System.Collections.Generic;
using UnityEngine;


public static class GameMapUtility
{
    public static bool RowColIsValid(int row, int col)
    {
        ref readonly var maxRowCol = ref GameMapMgr.Instance.GetMapGridRowCol();
        if (row < 0 || row >= maxRowCol.x)
            return false;
        if (col < 0 || col >= maxRowCol.y)
            return false;
        return true;
    }
    public static bool RowIsValid(int row)
    {
        ref readonly var maxRowCol = ref GameMapMgr.Instance.GetMapGridRowCol();
        if (row < 0 || row >= maxRowCol.x)
            return false;
        return true;
    }
    public static bool ColIsValid(int col)
    {
        if (col < 0)
            return false;
        ref readonly var maxRowCol = ref GameMapMgr.Instance.GetMapGridRowCol();
        if (col >= maxRowCol.y)
            return false;
        return true;
    }
    public static bool IndexIsValid(int index)
    {
        if (index < 0)
            return false;
        ref readonly var maxRowCol = ref GameMapMgr.Instance.GetMapGridRowCol();
        if (index >= maxRowCol.x * maxRowCol.y)
            return false;
        return true;
    }
    public static int RowColToIndex(in int row, in int col)
    {
        ref readonly var maxRowCol = ref GameMapMgr.Instance.GetMapGridRowCol();
        return row * maxRowCol.y + col;
    }
    public static (int row, int col) IndexToRowCol(int index)
    {
        ref readonly var maxRowCol = ref GameMapMgr.Instance.GetMapGridRowCol();
        var row = (int)(index / maxRowCol.y);
        var col = (int)(index % maxRowCol.y);
        return (Mathf.Clamp(row, 0, maxRowCol.x - 1), col);
    }

    public static bool IsValidWorldPos(in Vector2 worldPos)
    {
        ref readonly var lbPos = ref GameMapMgr.Instance.GetGridLbPosition();
        ref readonly var ruPos = ref GameMapMgr.Instance.GetGridRuPosition();
        if (worldPos.x < lbPos.x || worldPos.x > ruPos.x)
            return false;
        if (worldPos.y < lbPos.y || worldPos.y > ruPos.y)
            return false;
        return true;
    }
    #region grid index
    public static int WorldPosY2Row(in float posY)
    {
        ref readonly var startPos = ref GameMapMgr.Instance.GetGridLbPosition();
        ref readonly var size = ref GameMapMgr.Instance.GetMapGridSize();
        var localPosY = posY - startPos.y;
        var row = (int)(localPosY / size.y);
        return Mathf.Max(row, 0);
    }
    public static int WorldPosToGridIndex(Vector2 worldPos)
    {
        ref readonly var startPos = ref GameMapMgr.Instance.GetGridLbPosition();
        ref readonly var size = ref GameMapMgr.Instance.GetMapGridSize();
        var localPos = worldPos - startPos;
        var row = (int)(localPos.y / size.y);
        var col = (int)(localPos.x / size.x);
        ref readonly var maxRowCol = ref GameMapMgr.Instance.GetMapGridRowCol();
        var index = Mathf.Clamp(row, 0, maxRowCol.x - 1) * maxRowCol.y + Mathf.Clamp(col, 0, maxRowCol.y - 1);
        return index;
    }
    public static bool WorldPosToGridRowCol(in Vector2 worldPos, out int row, out int col)
    {
        ref readonly var startPos = ref GameMapMgr.Instance.GetGridLbPosition();
        ref readonly var size = ref GameMapMgr.Instance.GetMapGridSize();
        var localPos = worldPos - startPos;
        row = (int)(localPos.y / size.y);
        col = (int)(localPos.x / size.x);
        ref readonly var maxRowCol = ref GameMapMgr.Instance.GetMapGridRowCol();
        return row >= 0 && row < maxRowCol.x && col >= 0 && col <= maxRowCol.y;
    }

    public static GameBattleGridIndex CreateGridIndex(int row, int col)
    {
        ref readonly var maxRowCol = ref GameMapMgr.Instance.GetMapGridRowCol();
        var index = row * maxRowCol.y + col;
        return new GameBattleGridIndex(index, row, col);
    }

    public static GameBattleGridIndex CreateGridIndex(int index)
    {
        ref readonly var maxRowCol = ref GameMapMgr.Instance.GetMapGridRowCol();
        var col = maxRowCol.y;
        return new GameBattleGridIndex(index, (int)(index / col), index % col);
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

    public static void CheckMoveBoundsTopY(ref float worldPosY)
    {
        ref readonly var gridSize = ref GameMapMgr.Instance.GetMapGridSize();
        CheckMoveBoundsY(ref worldPosY, -gridSize.y / 2, gridSize.y / 2);
    }
    public static void CheckMoveBoundsY(ref float worldPosY, in float offsetTop, in float offsetDown)
    {
        ref readonly var lbPos = ref GameMapMgr.Instance.GetGridLbPosition();
        ref readonly var ruPos = ref GameMapMgr.Instance.GetGridRuPosition();

        var moveLbBoundY = lbPos.y + offsetDown;
        var moveRuBoundY = ruPos.y + offsetTop;
        if (worldPosY > moveRuBoundY || worldPosY < moveLbBoundY)
        {
            worldPosY = Mathf.Clamp(worldPosY, moveLbBoundY, moveRuBoundY);
        }
    }
    public static void CheckMoveBounds(ref Vector2 worldPos, in Vector2 halfSize)
    {
        ref readonly var lbPos = ref GameMapMgr.Instance.GetGridLbPosition();
        ref readonly var ruPos = ref GameMapMgr.Instance.GetGridRuPosition();

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

    public static int GetMapEagePoint(in Vector2 startPoint, in Vector2 dir, in EnDirection validEage, ref GamePhysicsHit[] points)
    {
        var count = GetMapEagePoint(in startPoint, in dir, in validEage, ref points, points.Length);
        return count;
    }
    public static int GetMapEagePoint(in Vector2 startPoint, in Vector2 dir, in EnDirection validEage, ref GamePhysicsHit[] points, in int maxCount)
    {
        ref readonly var lbPos = ref GameMapMgr.Instance.GetGridLbPosition();
        ref readonly var ruPos = ref GameMapMgr.Instance.GetGridRuPosition();
        var count = GamePhysicsUtility.CastBoxEage(in lbPos, ruPos, in startPoint, in dir, in validEage, ref points, maxCount);
        return count;
    }
}

