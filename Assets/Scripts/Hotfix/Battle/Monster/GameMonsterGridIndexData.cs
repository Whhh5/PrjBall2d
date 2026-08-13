using UnityEngine;

public class GameMonsterGridIndexData : IClassPoolNone
{
    public int top;
    public int center;
    public int bottom;
    public Vector2 lastPos;

    public static bool Equals(GameMonsterGridIndexData data, GameMonsterGridIndexData data2)
    {
        return data.top == data2.top && data.center == data2.center && data.bottom == data2.bottom;
    }
}