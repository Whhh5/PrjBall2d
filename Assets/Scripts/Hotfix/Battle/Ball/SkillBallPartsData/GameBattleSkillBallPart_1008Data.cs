using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBallPart_1008StarData : GameBattleSkillBallPartStarData
{
    public float repelDistance;
    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out repelDistance);
    }

    public override List<int> SerializeToIntArray()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in repelDistance);
        return list;
    }
}
public class GameBattleSkillBallPart_1008Data : GameBattleSkillBallPartData<GameBattleSkillBallPart_1008EntityData, GameBattleSkillBallPart_1008StarData>
{
    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new()
        {
            new GameBattleSkillBallPart_1008StarData()
            {
                repelDistance = 0.5f,
            },
            new GameBattleSkillBallPart_1008StarData()
            {
                repelDistance = 0.75f,
            },
            new GameBattleSkillBallPart_1008StarData()
            {
                repelDistance = 1f,
            },
        };
    }
}
