using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBallPart_1007StarData : GameBattleSkillBallPartStarData
{
    public float speedPercentage01;
    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out speedPercentage01);
    }

    public override List<int> SerializeToIntArray()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in speedPercentage01);
        return list;
    }
}
public class GameBattleSkillBallPart_1007Data : GameBattleSkillBallPartData<GameBattleSkillBallPart_1007EntityData, GameBattleSkillBallPart_1007StarData>
{
    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new()
        {
            new GameBattleSkillBallPart_1007StarData()
            {
                speedPercentage01 = 0.1f,
            },
            new GameBattleSkillBallPart_1007StarData()
            {
                speedPercentage01 = 0.15f,
            },
            new GameBattleSkillBallPart_1007StarData()
            {
                speedPercentage01 = 0.2f,
            },
        };
    }
}
