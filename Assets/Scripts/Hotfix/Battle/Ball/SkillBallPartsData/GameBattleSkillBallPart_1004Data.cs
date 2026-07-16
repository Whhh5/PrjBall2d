using System.Collections.Generic;

public class GameBattleSkillBallPart_1004StarData : GameBattleSkillBallPartStarData
{
    public int killProbablity;
    public float atkMaxHpPercentage;
    public EnDamageType atkDamageType;

    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out killProbablity);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkMaxHpPercentage);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkDamageType);
    }

    public override List<int> SerializeToIntArray()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in killProbablity);
        ISerializeToIntArray.SerializeToInt(ref list, in atkMaxHpPercentage);
        ISerializeToIntArray.SerializeToInt(ref list, atkDamageType);
        return list;
    }
}
public class GameBattleSkillBallPart_1004Data : GameBattleSkillBallPartData<GameBattleSkillBallPart_1004EntityData, GameBattleSkillBallPart_1004StarData>
{
    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new()
        {
            new GameBattleSkillBallPart_1004StarData()
            {
                killProbablity = 10,
                atkMaxHpPercentage = 1,
                atkDamageType = EnDamageType.True,
            },
            new GameBattleSkillBallPart_1004StarData()
            {
                killProbablity = 20,
                atkMaxHpPercentage = 1,
                atkDamageType = EnDamageType.True,
            },
            new GameBattleSkillBallPart_1004StarData()
            {
                killProbablity = 30,
                atkMaxHpPercentage = 1,
                atkDamageType = EnDamageType.True,
            },
        };
    }
}
