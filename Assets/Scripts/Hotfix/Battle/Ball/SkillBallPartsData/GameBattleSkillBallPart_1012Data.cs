using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBallPart_1012StarData : GameBattleSkillBallPartStarData
{
    public int collisionCount;
    public float restoreHealthPercentage01;

    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out collisionCount);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out restoreHealthPercentage01);
    }

    public override List<int> SerializeToIntArray()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in collisionCount);
        ISerializeToIntArray.SerializeToInt(ref list, in restoreHealthPercentage01);
        return list;
    }
}
public class GameBattleSkillBallPart_1012Data : GameBattleSkillBallPartData<GameBattleSkillBallPart_1012EntityData, GameBattleSkillBallPart_1012StarData>
{

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new()
        {
            new GameBattleSkillBallPart_1012StarData()
            {
                collisionCount = 10,
                restoreHealthPercentage01 = 0.1f,
            },
            new GameBattleSkillBallPart_1012StarData()
            {
                collisionCount = 8,
                restoreHealthPercentage01 = 0.1f,
            },
            new GameBattleSkillBallPart_1012StarData()
            {
                collisionCount = 6,
                restoreHealthPercentage01 = 0.1f,
            },
        };
    }
}
