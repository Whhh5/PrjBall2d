using UnityEngine;
using System.Collections.Generic;


public struct GameCollistionRectInfo
{
    public Vector2 collisionPoint;
    public EnDirection dir;
    public Vector2 normal;

    public GameCollistionRectInfo(in Vector2 collisionPoint, in EnDirection dir, in Vector2 normal)
    {
        this.collisionPoint = collisionPoint;
        this.dir = dir;
        this.normal = normal;
    }
}
public static class GameCollision2DUtil
{
    private static List<GameCollistionRectInfo> _TempVector2 = new(10);
    public static bool TryGetCircleRectIntersectionPoints(
        in Vector2 circleCenter,
        in float radius,
        in Vector2 rectCenter,
        in Vector2 rectSize,
        in EnDirection validEage,
        out GameCollistionRectInfo result)
    {
        var count = GetCircleRectIntersectionPoints(in circleCenter, in radius, in rectCenter, in rectSize, in validEage, ref _TempVector2);

        result = new();
        var addValue = Vector2.zero;
        var normalValue = Vector2.zero;
        for (int i = 0; i < count; i++)
        {
            var collInfo = _TempVector2[i];
            addValue += collInfo.collisionPoint;
            normalValue += collInfo.normal;
            result.dir |= collInfo.dir;
        }
        result.collisionPoint = addValue / count;
        result.normal = (normalValue / count).normalized;

        return count > 0;
    }
    /// <summary>
    /// 计算圆与轴对齐矩形的所有边缘交点
    /// </summary>
    /// <param name="circleCenter">圆心世界坐标</param>
    /// <param name="radius">圆半径</param>
    /// <param name="rectCenter">矩形中心点世界坐标</param>
    /// <param name="rectSize">矩形完整长宽</param>
    /// <returns>所有边缘交点的世界坐标列表</returns>
    public static int GetCircleRectIntersectionPoints(
        in Vector2 circleCenter,
        in float radius,
        in Vector2 rectCenter,
        in Vector2 rectSize,
        in EnDirection validEage,
        ref List<GameCollistionRectInfo> results)
    {
        results.Clear();
        float halfW = rectSize.x * 0.5f;
        float halfH = rectSize.y * 0.5f;

        // 矩形四条边界
        float left = rectCenter.x - halfW;
        float right = rectCenter.x + halfW;
        float bottom = rectCenter.y - halfH;
        float top = rectCenter.y + halfH;

        float ox = circleCenter.x;
        float oy = circleCenter.y;
        float rSq = radius * radius;
        const float epsilon = 1e-6f;

        // 1. 左边（垂直线段 x = left）
        if ((validEage & EnDirection.Left) > 0)
        {
            var dx = left - ox;
            var dySq = rSq - dx * dx;
            if (dySq >= 0f)
            {
                float sqrtDy = Mathf.Sqrt(dySq);
                float y1 = oy + sqrtDy;
                float y2 = oy - sqrtDy;

                if (y1 >= bottom - epsilon && y1 <= top + epsilon)
                    results.Add(new(new Vector2(left, y1), EnDirection.Left, Vector2.left));
                if (sqrtDy > epsilon && y2 >= bottom - epsilon && y2 <= top + epsilon)
                    results.Add(new(new Vector2(left, y2), EnDirection.Left, Vector2.left));
            }
        }

        // 2. 右边（垂直线段 x = right）
        if ((validEage & EnDirection.Right) > 0)
        {
            var dx = right - ox;
            var dySq = rSq - dx * dx;
            if (dySq >= 0f)
            {
                float sqrtDy = Mathf.Sqrt(dySq);
                float y1 = oy + sqrtDy;
                float y2 = oy - sqrtDy;

                if (y1 >= bottom - epsilon && y1 <= top + epsilon)
                    results.Add(new(new Vector2(right, y1), EnDirection.Right, Vector2.right));
                if (sqrtDy > epsilon && y2 >= bottom - epsilon && y2 <= top + epsilon)
                    results.Add(new(new Vector2(right, y2), EnDirection.Right, Vector2.right));
            }
        }

        // 3. 上边（水平线段 y = top）
        if ((validEage & EnDirection.Top) > 0)
        {
            var dy = top - oy;
            var dxSq = rSq - dy * dy;
            if (dxSq >= 0f)
            {
                float sqrtDx = Mathf.Sqrt(dxSq);
                float x1 = ox + sqrtDx;
                float x2 = ox - sqrtDx;

                if (x1 >= left - epsilon && x1 <= right + epsilon)
                    results.Add(new(new Vector2(x1, top), EnDirection.Top, Vector2.up));
                if (sqrtDx > epsilon && x2 >= left - epsilon && x2 <= right + epsilon)
                    results.Add(new(new Vector2(x2, top), EnDirection.Top, Vector2.up));
            }
        }

        // 4. 下边（水平线段 y = bottom）
        if ((validEage & EnDirection.Bottom) > 0)
        {
            var dy = bottom - oy;
            var dxSq = rSq - dy * dy;
            if (dxSq >= 0f)
            {
                float sqrtDx = Mathf.Sqrt(dxSq);
                float x1 = ox + sqrtDx;
                float x2 = ox - sqrtDx;

                if (x1 >= left - epsilon && x1 <= right + epsilon)
                    results.Add(new(new Vector2(x1, bottom), EnDirection.Bottom, Vector2.down));
                if (sqrtDx > epsilon && x2 >= left - epsilon && x2 <= right + epsilon)
                    results.Add(new(new Vector2(x2, bottom), EnDirection.Bottom, Vector2.down));
            }
        }

        // 角点去重：圆刚好切到矩形顶点时，两条边会重复计算同一点
        for (int i = results.Count - 1; i >= 0; i--)
        {
            for (int j = i - 1; j >= 0; j--)
            {
                var disSqr = Vector2.SqrMagnitude(results[i].collisionPoint - results[j].collisionPoint);
                if (Mathf.Approximately(disSqr, 0))
                {
                    results.RemoveAt(i);
                    break;
                }
            }
        }

        return results.Count;
    }
}