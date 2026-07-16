
using System;
using UnityEngine;

public class GameMonsterGridIndexData : IClassPoolNone
{
    public int top;
    public int center;
    public int bottom;
    public Vector2 lastPos;

    public static bool operator ==(GameMonsterGridIndexData data, GameMonsterGridIndexData data2)
    {
        return data.top == data2.top && data.center == data2.center && data.bottom == data2.bottom;
    }
    public static bool operator !=(GameMonsterGridIndexData data, GameMonsterGridIndexData data2)
    {
        return data.top != data2.top || data.center != data2.center || data.bottom != data2.bottom;
    }
    public GameMonsterGridIndexData Copy()
    {
        return new GameMonsterGridIndexData()
        {
            top = top,
            center = center,
            bottom = bottom,
            lastPos = lastPos,
        };
    }

    public override bool Equals(object obj)
    {
        return obj is GameMonsterGridIndexData data &&
               top == data.top &&
               center == data.center &&
               bottom == data.bottom;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(top, center, bottom);
    }
}