using System.Collections.Generic;

public class GameBattleSkillBallPart_1005StarData : GameBattleSkillBallPartStarData
{
    public IEntityBuffData buffData;
    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out buffData, in userData);
    }

    public override List<int> SerializeToIntArray()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in buffData);
        return list;
    }
}
public class GameBattleSkillBallPart_1005Data : GameBattleSkillBallPartData<GameBattleSkillBallPart_1005EntityData, GameBattleSkillBallPart_1005StarData>
{
    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new()
        {
            new GameBattleSkillBallPart_1005StarData()
            {
                buffData = new EntityShatteredArmorBuffData()
                {
                    duration = 3,
                    maxCount= 3,
                    shatteredArmorPercentage = 0.1f,
                },
            },
            new GameBattleSkillBallPart_1005StarData()
            {
                buffData = new EntityShatteredArmorBuffData()
                {
                    duration = 5,
                    maxCount= 3,
                    shatteredArmorPercentage = 0.2f,
                },
            },
            new GameBattleSkillBallPart_1005StarData()
            {
                buffData = new EntityShatteredArmorBuffData()
                {
                    duration = 8,
                    maxCount= 3,
                    shatteredArmorPercentage = 0.3f,
                },
            },
        };
    }
}
