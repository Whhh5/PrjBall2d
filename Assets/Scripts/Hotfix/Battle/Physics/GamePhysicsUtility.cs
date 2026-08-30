using System.Collections.Generic;
using UnityEngine;

public enum EnDirection
{
    None,
    Top = 1 << 0,
    Bottom = 1 << 1,
    Left = 1 << 2,
    Right = 1 << 3,

    All = Top | Bottom | Left | Right,
}
public struct GamePhysicsHit
{
    public Vector2 point;
    public int pointGridIndex;
    public int entityId;
    public Vector2 normal;
    public EnDirection dir;
    public Vector2 inDir;
}
public struct GamePhysicsEntity
{
    public int entityId;
    public int localRow;
    public int localCol;
    public int centerIndex;
    public Vector2 center;
    public float disSqr;
    public EnDirection validEage;
    public Vector2 collistionPoint;
}
public static class GamePhysicsUtility
{
    private static int _MapId;
    private static Vector2 _GridSize;
    private static Vector2 _GridLbPos;
    private static Vector2Int _MaxGridRowCol;
    private static float _GridMaxRadius;

    private static List<int> _EntityIds = new(30);
    private static readonly List<(int, int)> _IndexIds = new(30);
    private static readonly HashSet<int> _EntityIdHas = new(30);
    private static List<GamePhysicsEntity> _EntityInfos = new(30);
    public static void InitParams(int mapId)
    {
        _MapId = mapId;
        _GridSize = GameMapMgr.Instance.GetMapGridSize();
        _MaxGridRowCol = GameMapMgr.Instance.GetMapGridRowCol();
        _GridLbPos = GameMapMgr.Instance.GetGridLbPosition();
        _GridMaxRadius = Vector2.Distance(_GridSize / 2f, Vector2.zero);
    }
    private static int RowColToIndex(in int row, in int col)
    {
        return row * _MaxGridRowCol.y + col;
    }
    private static bool RowColIsValid(int row, int col)
    {
        if (!RowIsValid(row))
            return false;
        if (!ColIsValid(col))
            return false;
        return true;
    }
    private static bool RowIsValid(in int row)
    {
        if (row < 0 || row >= _MaxGridRowCol.x)
            return false;
        return true;
    }
    private static bool ColIsValid(in int col)
    {
        if (col < 0 || col >= _MaxGridRowCol.y)
            return false;
        return true;
    }
    private static (int row, int col) IndexToRowCol(int index)
    {
        var row = (int)(index / _MaxGridRowCol.y);
        var col = (int)(index % _MaxGridRowCol.y);
        return (Mathf.Clamp(row, 0, _MaxGridRowCol.x - 1), col);
    }
    private static int WorldPosToGridIndex(in Vector2 worldPos)
    {
        var localPos = worldPos - _GridLbPos;
        var row = (int)(localPos.y / _GridSize.y);
        var col = (int)(localPos.x / _GridSize.x);

        var index = Mathf.Clamp(row, 0, _MaxGridRowCol.x - 1) * _MaxGridRowCol.y + Mathf.Clamp(col, 0, _MaxGridRowCol.y - 1);
        return index;
    }
    private static (int, int) WorldPosToRowCol(in Vector2 worldPos)
    {
        var localPos = worldPos - _GridLbPos;
        var row = (int)(localPos.y / _GridSize.y);
        var col = (int)(localPos.x / _GridSize.x);
        return (row, col);
    }
    private static Vector2 GetGridWorldPos(int row, int col)
    {
        var localPos = new Vector2(col + 0.5f, row + 0.5f) * _GridSize;
        return localPos + _GridLbPos;
    }
    public static int TryLineRowCast(in Vector2 point, float halfWidth, ref GamePhysicsHit[] hits)
    {
        var gridIndex = WorldPosToGridIndex(in point);
        var (row, col) = IndexToRowCol(gridIndex);
        var gridPos = GetGridWorldPos(row, col);
        var gridHeightOffset = _GridSize.y * 0.5f - halfWidth;
        var rowOffset = Mathf.CeilToInt(halfWidth / (_GridSize.y / 0.5f)) - 1;
        var startRow = Mathf.Clamp(row + (point.y < (gridPos.y - gridHeightOffset) ? -1 : 0), 0, _MaxGridRowCol.x - 1);
        var endRow = Mathf.Clamp(row + (point.y > (gridPos.y + gridHeightOffset) ? 1 : 0), 0, _MaxGridRowCol.x - 1);
        _EntityIds.Clear();
        _EntityIdHas.Clear();
        for (int i = startRow - rowOffset; i <= endRow + rowOffset; i++)
        {
            for (int j = 0; j < _MaxGridRowCol.y; j++)
            {
                var targetIndex = RowColToIndex(i, j);
                GameBattleGridMgr.Instance.GetGridEntityIds(targetIndex, ref _EntityIds);
            }
        }

        for (int i = 0; i < _EntityIds.Count; i++)
        {
            var entityId = _EntityIds[i];
            _EntityIdHas.Add(entityId);
        }

        var count = 0;
        foreach (var entityId in _EntityIdHas)
        {
            var gridData = EntityMgr.Instance.GetEntityData<IEntityGridPointInfo>(in entityId);
            var centerPos = gridData.GetCenterPointWorldPos();
            var dis = centerPos.y - point.y;
            if (Mathf.Abs(dis) > halfWidth + _GridSize.y / 2)
                continue;
            if (count >= hits.Length)
                return count;

            //var pointPosYOffset = halfWidth - dis;

            ref var item = ref hits[count++];
            item.point = centerPos;
            item.pointGridIndex = WorldPosToGridIndex(centerPos);
            item.entityId = entityId;
            item.normal = new Vector2(point.x - centerPos.x, 0).normalized;
            item.dir = item.normal.x > 0 ? EnDirection.Right : EnDirection.Left;
            item.inDir = new Vector2(centerPos.x - point.x, 0).normalized;
        }
        return count;
    }
    private static bool LineCrossRect(float A, float B, float C, float x0, float y0, float x1, float y1)
    {
        var v0 = A * x0 + B * y0 + C;
        var v1 = A * x1 + B * y0 + C;
        var v2 = A * x1 + B * y1 + C;
        var v3 = A * x0 + B * y1 + C;
        var minv = Mathf.Min(v0, v1);
        minv = Mathf.Min(minv, v2);
        minv = Mathf.Min(minv, v3);
        var maxv = Mathf.Max(v0, v1);
        maxv = Mathf.Max(maxv, v2);
        maxv = Mathf.Max(maxv, v3);
        // 正负都有，穿过矩形
        return minv <= 0.0001f && maxv >= -0.0001f;
    }
    private static bool TryGetLineEntityIds(in Vector2 point1, in Vector2 point2, float halfWidth, ref List<int> entityIds)
    {

        var (row1, col1) = WorldPosToRowCol(in point1);
        var (row2, col2) = WorldPosToRowCol(in point2);

        var lineA = point1.y - point2.y;
        var lineB = point2.x - point1.x;
        var lineC = point1.x * point2.y - point2.x * point1.y;

        int testRow = row1, testCol = col1;
        var nextRowOffset = row1 < row2 ? 1 : row1 > row2 ? -1 : 0;
        var nextColOffset = col1 < col2 ? 1 : col1 > col2 ? -1 : 0;

        while (!ColIsValid(testCol))
        {
            if (testCol == col2)
                return false;
            testCol += nextColOffset;
        }
        while (!RowIsValid(testRow))
        {
            if (testRow == row2)
                return false;
            testRow += nextRowOffset;
        }

        _IndexIds.Clear();
        entityIds.Clear();
        _EntityIdHas.Clear();
        _IndexIds.Add((testRow, testCol));

        do
        {
            if (nextRowOffset != 0 && testRow != row2)
            {
                var testCenter = GetGridWorldPos(testRow + nextRowOffset, testCol);
                var testLbPoint = testCenter - _GridSize / 2;
                var testRuPoint = testCenter + _GridSize / 2;
                if (LineCrossRect(lineA, lineB, lineC, testLbPoint.x, testLbPoint.y, testRuPoint.x, testRuPoint.y))
                {
                    testRow += nextRowOffset;
                    _IndexIds.Add((testRow, testCol));
                    continue;
                }
            }
            if (nextColOffset != 0 && testCol != col2)
            {
                var testCenter = GetGridWorldPos(testRow, testCol + nextColOffset);
                var testLbPoint = testCenter - _GridSize / 2;
                var testRuPoint = testCenter + _GridSize / 2;
                if (LineCrossRect(lineA, lineB, lineC, testLbPoint.x, testLbPoint.y, testRuPoint.x, testRuPoint.y))
                {
                    testCol += nextColOffset;
                    _IndexIds.Add((testRow, testCol));
                    continue;
                }
            }
            break;

        } while (testRow != row2 || testCol != col2);

        var extend = Mathf.CeilToInt(halfWidth / (_GridSize.x / 2));
        for (int k = 0; k < _IndexIds.Count; k++)
        {
            var (row, col) = _IndexIds[k];
            for (int i = -extend; i <= extend; i++)
            {
                if (!RowIsValid(row + i))
                    continue;
                for (int j = -extend; j <= extend; j++)
                {
                    if (!ColIsValid(col + j))
                        continue;
                    var gridIndex = RowColToIndex(row + i, col + j);
                    GameBattleGridMgr.Instance.GetGridEntityIds(gridIndex, ref entityIds);
                }
            }
        }

        for (int i = 0; i < entityIds.Count; i++)
        {
            var entityId = entityIds[i];
            _EntityIdHas.Add(entityId);
        }
        entityIds.Clear();
        entityIds.AddRange(_EntityIdHas);
        return entityIds.Count > 0;
    }
    public static int TryLineCast(in Vector2 point1, in Vector2 point2, float halfWidth, ref GamePhysicsHit[] hits)
    {
        if (!TryGetLineEntityIds(in point1, in point2, halfWidth, ref _EntityIds))
            return 0;
        var lineA = point1.y - point2.y;
        var lineB = point2.x - point1.x;
        var lineC = point1.x * point2.y - point2.x * point1.y;

        var count = 0;
        foreach (var entityId in _EntityIds)
        {
            var gridData = EntityMgr.Instance.GetEntityData<IEntityGridPointInfo>(in entityId);
            var centerPos = gridData.GetCenterPointWorldPos();
            ref readonly var mainGridIndex = ref gridData.GetMainGridIndex();
            var mainGridPos = gridData.GetMainGridPos();
            ref readonly var arrGridIndesx = ref gridData.GetGridIndexs();
            for (int i = 0; i < arrGridIndesx.Length; i += 2)
            {
                ref readonly var row = ref arrGridIndesx[i];
                ref readonly var col = ref arrGridIndesx[i + 1];

                var testCenterPos = mainGridPos + new Vector2(col, row) * _GridSize;
                var disSqr = Mathf.Pow(lineA * testCenterPos.x + lineB * testCenterPos.y + lineC, 2) / (Mathf.Pow(lineA, 2) + Mathf.Pow(lineB, 2));
                if (disSqr > Mathf.Pow(halfWidth + _GridSize.y / 2, 2))
                    continue;
                if (count >= hits.Length)
                    return count;

                ref var item = ref hits[count++];
                item.point = testCenterPos;
                item.pointGridIndex = mainGridIndex.index;
                item.entityId = entityId;
                item.normal = Vector2.up;
                item.dir = EnDirection.None;
                item.inDir = -item.normal;
                break;
            }
        }
        return count;
    }
    public static bool TryLineCastSort(in Vector2 point1, in Vector2 point2, float halfWidth, ref GamePhysicsHit hits)
    {
        if (!TryGetLineEntityIds(in point1, in point2, halfWidth, ref _EntityIds))
            return false;

        var minDis = float.MaxValue;
        var halfSize = _GridSize / 2;
        var lineDir = (point2 - point1).normalized;
        var rad = Mathf.Atan(lineDir.y / lineDir.x);
        Vector2 hitPoint1 = default, hitPoint2 = default;
        hits.entityId = -1;
        for (int j = 0; j < _EntityIds.Count; j++)
        {
            var entityId = _EntityIds[j];

            var gridData = EntityMgr.Instance.GetEntityData<IEntityGridPointInfo>(in entityId);
            ref readonly var mainGridIndex = ref gridData.GetMainGridIndex();
            var mainGridPos = gridData.GetMainGridPos();
            ref readonly var arrGridIndesx = ref gridData.GetGridIndexs();

            for (int i = 0; i < arrGridIndesx.Length; i += 2)
            {
                ref readonly var row = ref arrGridIndesx[i];
                ref readonly var col = ref arrGridIndesx[i + 1];

                var testCenterPos = mainGridPos + new Vector2(col, row) * _GridSize;
                if (!TryLineCaseRect(testCenterPos - halfSize, testCenterPos + halfSize, point1, rad, ref hitPoint1, ref hitPoint2))
                    continue;
                var testIndex = -1;
                if (DisTest(in hitPoint1, in point1, in minDis, out var disSqr))
                {
                    minDis = disSqr;
                    testIndex++;
                }
                if (DisTest(in hitPoint2, in point1, in minDis, out disSqr))
                {
                    minDis = disSqr;
                    testIndex++;
                }
                if (testIndex < 0)
                    continue;

                hits.point = testCenterPos;
                hits.pointGridIndex = mainGridIndex.index;
                hits.entityId = entityId;
                hits.normal = Vector2.up;
                hits.dir = EnDirection.None;
                hits.inDir = -hits.normal;
            }
        }
        static bool DisTest(in Vector2 point1, in Vector2 point2, in float value, out float dis)
        {
            dis = Vector2.SqrMagnitude(point1 - point2);
            if (dis >= value)
                return false;
            return true;
        }
        return hits.entityId > 0;
    }
    private static bool TrySphereCast(in Vector2 point, in float radius, ref List<GamePhysicsEntity> entityInfos)
    {
        entityInfos.Clear();
        _EntityIds.Clear();
        var gridIndex = WorldPosToGridIndex(point);
        var (row, col) = IndexToRowCol(gridIndex);
        var gridWorldPos = GameBattleGridMgr.Instance.GetGridWorldPos(gridIndex);

        var startOffsetRow = point.y > gridWorldPos.y ? 0 : -1;
        var endOffsetRow = point.y > gridWorldPos.y ? 1 : 0;
        var startOffsetCol = point.x > gridWorldPos.x ? 0 : -1;
        var endOffsetCol = point.x > gridWorldPos.x ? 1 : 0;

        var extendRange = Mathf.FloorToInt(radius / (_GridSize.x / 2));

        GetGridEntityIds(ref _EntityIds,
            row + startOffsetRow - extendRange, row + endOffsetRow + extendRange,
            col + startOffsetCol - extendRange, col + endOffsetCol + extendRange
            );
        if (_EntityIds.Count == 0)
            return false;
        FilterEntitys(ref entityInfos, in _EntityIds, in point, in radius);

        if (entityInfos.Count == 0)
            return false;
        return true;
    }
    public static bool TrySphereCast(in Vector2 point, in float radius, in Vector2 inDir, ref GamePhysicsHit hit)
    {
        if (!TrySphereCast(in point, in radius, ref _EntityInfos))
            return false;
        SortByDistance(ref _EntityInfos);

        for (int i = 0; i < _EntityInfos.Count; i++)
        {
            var entitiyInfo = _EntityInfos[i];

            var angle = Vector2.Dot(entitiyInfo.center - point, inDir);
            if (angle < 0)
                continue;

            var validEage = EnDirection.None;
            if (Mathf.Approximately(inDir.x, 0) && Mathf.Approximately(inDir.y, 0))
                validEage |= EnDirection.All;
            else
            {
                if (inDir.x >= 0)
                    validEage |= EnDirection.Left;
                if (inDir.x <= 0)
                    validEage |= EnDirection.Right;

                if (inDir.y >= 0)
                    validEage |= EnDirection.Bottom;
                if (inDir.y <= 0)
                    validEage |= EnDirection.Top;
            }
            var dir = point - entitiyInfo.center;
            var k = dir.y / dir.x;
            if (Mathf.Abs(k - 1) > 0.1f && Mathf.Abs(k - (-1)) > 0.1f)
            {
                if (k > 1 || k < -1)
                    validEage &= EnDirection.Top | EnDirection.Bottom;
                else
                    validEage &= EnDirection.Left | EnDirection.Right;
            }

            validEage &= entitiyInfo.validEage;
            if (!GameCollision2DUtil.TryGetCircleRectIntersectionPoints(in point, in radius, in entitiyInfo.center, in _GridSize, in validEage, out var rectInfo))
                continue;
            hit.point = rectInfo.collisionPoint;
            hit.entityId = entitiyInfo.entityId;
            hit.normal = rectInfo.normal;
            hit.dir = rectInfo.dir;
            hit.inDir = inDir;
            hit.pointGridIndex = entitiyInfo.centerIndex;

            return true;
        }
        return false;
    }
    public static int SphereCastAll(in Vector2 point, in float radius, ref GamePhysicsHit[] hits)
    {
        if (!TrySphereCast(in point, in radius, ref _EntityInfos))
            return 0;
        var count = 0;
        _EntityIdHas.Clear();
        for (int i = 0; i < _EntityInfos.Count; i++)
        {
            var entitiyInfo = _EntityInfos[i];
            if (!_EntityIdHas.Add(entitiyInfo.entityId))
                continue;

            ref var hit = ref hits[count++];
            hit.entityId = entitiyInfo.entityId;

            if (count >= hits.Length)
                return count;
        }
        return count;
    }
    private static void SortByDistance(ref List<GamePhysicsEntity> entityInfos)
    {
        entityInfos.Sort((entity1, entity2) =>
        {
            if (entity1.disSqr < entity2.disSqr)
                return -1;
            if (entity1.disSqr > entity2.disSqr)
                return 1;
            return 0;
        });
    }
    private static void FilterEntitys(
        ref List<GamePhysicsEntity> entityInfos,
        in List<int> entityIds,
        in Vector2 worldPos,
        in float radius)
    {
        for (int i = 0; i < entityIds.Count; i++)
        {
            var entityId = entityIds[i];
            var entity = EntityMgr.Instance.GetEntityData<IEntityGridPointInfo>(entityId);

            ref readonly var arrGridLocalPos = ref entity.GetGridIndexs();
            ref readonly var arrGridValidEdge = ref entity.GetGridValidEdges();
            var pos = entity.GetMainGridPos();
            ref readonly var gridIndex = ref entity.GetMainGridIndex();

            for (int j = 0; j < arrGridLocalPos.Length; j += 2)
            {
                var rowOffset = arrGridLocalPos[j];
                var colOffset = arrGridLocalPos[j + 1];
                var pos2 = pos + new Vector2(colOffset * _GridSize.x, rowOffset * _GridSize.y);
                if (!IsCircleIntersectRect(in worldPos, in radius, in pos2, in _GridSize, out var point, out var disSqr))
                    continue;
                var index = RowColToIndex(gridIndex.row + rowOffset, gridIndex.col + colOffset);
                entityInfos.Add(new GamePhysicsEntity()
                {
                    entityId = entityId,
                    localRow = rowOffset,
                    localCol = colOffset,
                    center = pos2,
                    disSqr = disSqr,
                    centerIndex = index,
                    validEage = (EnDirection)arrGridValidEdge[j / 2],
                    collistionPoint = point,
                });
            }
        }
    }
    public static bool IsCircleIntersectRect(in Vector2 circleCenter, in float radius, in Vector2 rectCenter, in Vector2 rectSize, out Vector2 point, out float disSqr)
    {
        // 计算矩形半长、半宽
        float halfWidth = rectSize.x * 0.5f;
        float halfHeight = rectSize.y * 0.5f;

        // 1. 找到矩形上距离圆心最近的点（将圆心钳制在矩形边界内）
        point.x = Mathf.Clamp(circleCenter.x, rectCenter.x - halfWidth, rectCenter.x + halfWidth);
        point.y = Mathf.Clamp(circleCenter.y, rectCenter.y - halfHeight, rectCenter.y + halfHeight);

        // 2. 计算圆心到最近点的距离平方（用平方比较，避免开方，提升性能）
        float dx = circleCenter.x - point.x;
        float dy = circleCenter.y - point.y;
        disSqr = dx * dx + dy * dy;

        // 3. 距离平方 ≤ 半径平方 → 相交
        return disSqr < radius * radius;
    }
    private static void GetGridEntityIds(ref List<int> entityIds,
        int startRow,
        int endRow,
        int startCol,
        int endCol)
    {
        for (int testRow = startRow; testRow <= endRow; testRow++)
        {
            if (testRow < 0 || testRow >= _MaxGridRowCol.x)
                continue;
            for (int testCol = startCol; testCol <= endCol; testCol++)
            {
                if (testCol < 0 || testCol >= _MaxGridRowCol.y)
                    continue;
                var testGridIndex = RowColToIndex(testRow, testCol);
                GameBattleGridMgr.Instance.GetGridEntityIds(testGridIndex, ref entityIds);
            }
        }
    }


    public static int CastBoxEage(in Vector2 boxLbPos, in Vector2 boxRuPos, in Vector2 startPoint, in Vector2 dir, in EnDirection validEage, ref GamePhysicsHit[] points)
    {
        var count = CastBoxEage(in boxLbPos, in boxRuPos, in startPoint, in dir, in validEage, ref points, points.Length);
        return count;
    }
    public static int CastBoxEage(in Vector2 boxLbPos, in Vector2 boxRuPos, in Vector2 startPoint, in Vector2 dir, in EnDirection validEage, ref GamePhysicsHit[] points, in int maxCount)
    {
        var k = dir.y / dir.x;
        var count = 0;
        if ((validEage & EnDirection.Top) != 0)
        {
            var testX = (boxRuPos.y - startPoint.y + k * startPoint.x) / k;
            if (testX >= boxLbPos.x && testX <= boxRuPos.x)
            {
                var point = new Vector2(testX, boxRuPos.y);
                if (count == maxCount)
                {
                    if (TryAddByDistance(ref points, in count, in startPoint, in point, out var changeIndex))
                    {
                        AddArray(ref points, changeIndex, in point, EnDirection.Top, Vector2.up);
                    }
                }
                else
                {
                    AddArray(ref points, count++, in point, EnDirection.Top, Vector2.up);
                }
            }
        }
        if ((validEage & EnDirection.Bottom) != 0)
        {
            var testX = (boxLbPos.y - startPoint.y + k * startPoint.x) / k;
            if (testX >= boxLbPos.x && testX <= boxRuPos.x)
            {
                var point = new Vector2(testX, boxLbPos.y);
                if (count == maxCount)
                {
                    if (TryAddByDistance(ref points, in count, in startPoint, in point, out var changeIndex))
                    {
                        AddArray(ref points, changeIndex, in point, EnDirection.Bottom, Vector2.down);
                    }
                }
                else
                {
                    AddArray(ref points, count++, in point, EnDirection.Bottom, Vector2.down);
                }
            }
        }
        if ((validEage & EnDirection.Left) != 0)
        {
            var testY = boxLbPos.x * k - k * startPoint.x + startPoint.y;
            if (testY >= boxLbPos.y && testY <= boxRuPos.y)
            {
                var point = new Vector2(boxLbPos.x, testY);
                if (count == maxCount)
                {
                    if (TryAddByDistance(ref points, in count, in startPoint, in point, out var changeIndex))
                    {
                        AddArray(ref points, changeIndex, in point, EnDirection.Left, Vector2.left);
                    }
                }
                else
                {
                    AddArray(ref points, count++, in point, EnDirection.Left, Vector2.left);
                }
            }
        }
        if ((validEage & EnDirection.Right) != 0)
        {
            var testY = boxRuPos.x * k - k * startPoint.x + startPoint.y;
            if (testY >= boxLbPos.y && testY <= boxRuPos.y)
            {
                var point = new Vector2(boxRuPos.x, testY);
                if (count == maxCount)
                {
                    if (TryAddByDistance(ref points, in count, in startPoint, in point, out var changeIndex))
                    {
                        AddArray(ref points, changeIndex, in point, EnDirection.Right, Vector2.right);
                    }
                }
                else
                {
                    AddArray(ref points, count++, in point, EnDirection.Right, Vector2.right);
                }
            }
        }
        static void AddArray(ref GamePhysicsHit[] points, in int index, in Vector2 point, in EnDirection dir, in Vector2 normal)
        {
            ref var hit = ref points[index];
            hit.point = point;
            hit.dir = dir;
            hit.normal = normal;
        }
        static bool TryAddByDistance(ref GamePhysicsHit[] points, in int count, in Vector2 startPoint, in Vector2 point, out int changeIndex)
        {
            changeIndex = 0;
            var minDis = float.MaxValue;
            for (int i = 0; i < count; i++)
            {
                var pos = points[i].point;
                var disSqr = Vector2.SqrMagnitude(pos - startPoint);
                if (disSqr < minDis)
                {
                    changeIndex = i;
                    minDis = disSqr;
                }
            }
            var testDis = Vector2.SqrMagnitude(point - startPoint);
            if (testDis > minDis)
                return false;
            return true;
        }
        return count;
    }

    public static int BoxCast(in Vector2 boxCenter, in Vector2 halfSize, ref GamePhysicsHit[] hits)
    {
        var count = 0;
        var (row, col) = WorldPosToRowCol(in boxCenter);
        var rowOffset = Mathf.CeilToInt(halfSize.y / _GridSize.y);
        var colOffset = Mathf.CeilToInt(halfSize.x / _GridSize.x);

        var boxLb = boxCenter - halfSize;
        var boxRu = boxCenter + halfSize;

        _EntityIds.Clear();
        _EntityIdHas.Clear();
        for (int i = row - rowOffset; i <= row + rowOffset; i++)
        {
            if (!RowIsValid(in i))
                continue;
            for (int j = col - colOffset; j <= col + colOffset; j++)
            {
                if (!ColIsValid(in j))
                    continue;
                var index = RowColToIndex(in i, in j);
                GameBattleGridMgr.Instance.GetGridEntityIds(in index, ref _EntityIds);
            }
        }
        foreach (var entityId in _EntityIds)
            _EntityIdHas.Add(entityId);

        foreach (var entityId in _EntityIdHas)
        {
            var pointInfo = EntityMgr.Instance.GetEntityData<IEntityGridPointInfo>(entityId);
            var mainPos = pointInfo.GetMainGridPos();
            var posArr = pointInfo.GetGridIndexs();

            for (int i = 0; i < posArr.Length; i += 2)
            {
                if (count >= hits.Length)
                    return count;
                var localRow = posArr[i];
                var localCol = posArr[i + 1];
                var lb = mainPos + _GridSize * new Vector2(localCol - 0.5f, localRow - 0.5f);
                var ru = mainPos + _GridSize * new Vector2(localCol + 0.5f, localRow + 0.5f);

                if (!RectIsContains(in boxLb, in boxRu, in lb, in ru))
                    continue;
                ref var hit = ref hits[count++];
                hit.entityId = entityId;
                break;
            }
        }
        return count;
    }

    public static bool RectIsContains(in Vector2 lb, in Vector2 ru, in Vector2 testLb, in Vector2 testRu)
    {
        if (lb.x > testLb.x && lb.x < testRu.x)
        {
            if (lb.y > testLb.y && lb.y < testRu.y)
                return true;
            if (ru.y > testLb.y && ru.y < testRu.y)
                return true;
        }
        if (ru.x > testLb.x && ru.x < testRu.x)
        {
            if (lb.y > testLb.y && lb.y < testRu.y)
                return true;
            if (ru.y > testLb.y && ru.y < testRu.y)
                return true;
        }
        if (testLb.x > lb.x && testLb.x < ru.x)
        {
            if (testLb.y > lb.y && testLb.y < ru.y)
                return true;
            if (testRu.y > lb.y && testRu.y < ru.y)
                return true;
        }
        if (testRu.x > lb.x && testRu.x < ru.x)
        {
            if (testLb.y > lb.y && testLb.y < ru.y)
                return true;
            if (testRu.y > lb.y && testRu.y < ru.y)
                return true;
        }
        return false;
    }


    public static bool TryLineCaseRect(in Vector2 lb, in Vector2 ru, in Vector2 target, in float rad, ref Vector2 point1, ref Vector2 point2)
    {
        var b = target.y - Mathf.Tan(rad) * target.x;
        var count = 0;
        { // left
            var y = Mathf.Tan(rad) * lb.x + b;
            if (y >= lb.y && y <= ru.y)
            {
                point1 = new Vector2(lb.x, y);
                count++;
            }
        }
        { // bottom
            var x = (lb.y - b) / Mathf.Tan(rad);
            if (x >= lb.x && x <= ru.x)
            {
                if (count == 0)
                {
                    point1 = new Vector2(x, lb.y);
                    count++;
                }
                else
                {
                    point2 = new Vector2(x, lb.y);
                    return true;
                }
            }
        }
        { // right
            var y = Mathf.Tan(rad) * ru.x + b;
            if (y >= lb.y && y <= ru.y)
            {
                if (count == 0)
                {
                    point1 = new Vector2(ru.x, y);
                    count++;
                }
                else
                {
                    point2 = new Vector2(ru.x, y);
                    return true;
                }
            }
        }
        { // up
            if (count == 0)
                return false;

            var x = (ru.y - b) / Mathf.Tan(rad);
            if (x >= lb.x && x <= ru.x)
            {
                point2 = new Vector2(x, ru.y);
                return true;
            }
        }
        return false;
    }
}
