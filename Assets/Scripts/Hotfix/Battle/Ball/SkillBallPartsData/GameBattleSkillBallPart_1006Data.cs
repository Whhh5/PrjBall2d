using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBallPart_1006StarData : GameBattleSkillBallPartStarData
{
    public float duration;
    public EnBuff testBuffId;
    public int testBuffCount;
    public int testProbability;
    public IEntityBuffData buffData;

    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out duration);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out testBuffId);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out testBuffCount);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out testProbability);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out buffData, userData);
    }

    public override List<int> SerializeToIntArray()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in duration);
        ISerializeToIntArray.SerializeToInt(ref list, testBuffId);
        ISerializeToIntArray.SerializeToInt(ref list, in testBuffCount);
        ISerializeToIntArray.SerializeToInt(ref list, in testProbability);
        ISerializeToIntArray.SerializeToInt(ref list, in buffData);
        return list;
    }
}
public class GameBattleSkillBallPart_1006Data : GameBattleSkillBallPartData<GameBattleSkillBallPart_1006EntityData, GameBattleSkillBallPart_1006StarData>
{
    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new()
        {
            new GameBattleSkillBallPart_1006StarData()
            {
                duration = 0.3f,
                testBuffId = EnBuff.Crystal,
                testBuffCount = 3,
                testProbability = 20,
                buffData = new EntityFreezeBuffData()
                {
                    duration = 1f,
                    speedReducePercentage = 1,
                },
            },
            new GameBattleSkillBallPart_1006StarData()
            {
                duration = 0.5f,
                testBuffId = EnBuff.Crystal,
                testBuffCount = 3,
                testProbability = 30,
                buffData = new EntityFreezeBuffData()
                {
                    duration = 1f,
                    speedReducePercentage = 1,
                },
            },
            new GameBattleSkillBallPart_1006StarData()
            {
                duration = 0.8f,
                testBuffId = EnBuff.Crystal,
                testBuffCount = 3,
                testProbability = 50,
                buffData = new EntityFreezeBuffData()
                {
                    duration = 1f,
                    speedReducePercentage = 1,
                },
            },
        };
    }
}
