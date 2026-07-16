using System.Collections.Generic;

public class GameBattleSkillBallPart_1003StarData : GameBattleSkillBallPartStarData
{
    public int atkDamage;
    public EnDamageType atkDamageType;
    public float atkDamagePercentage;

    public EnBuff buffId;
    public int buffCount;
    public float atkDuration;
    public float atkInterval;

    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out buffId);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out buffCount);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkDamagePercentage);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkDamage);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkDamageType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkDuration);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkInterval);
    }

    public override List<int> SerializeToIntArray()
    {
        var list = new List<int>();

        ISerializeToIntArray.SerializeToInt(ref list, buffId);
        ISerializeToIntArray.SerializeToInt(ref list, in buffCount);
        ISerializeToIntArray.SerializeToInt(ref list, in atkDamagePercentage);
        ISerializeToIntArray.SerializeToInt(ref list, in atkDamage);
        ISerializeToIntArray.SerializeToInt(ref list, atkDamageType);
        ISerializeToIntArray.SerializeToInt(ref list, in atkDuration);
        ISerializeToIntArray.SerializeToInt(ref list, in atkInterval);

        return list;
    }
}
public class GameBattleSkillBallPart_1003Data : GameBattleSkillBallPartData<GameBattleSkillBallPart_1003EntityData, GameBattleSkillBallPart_1003StarData>
{
    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
        {
            new GameBattleSkillBallPart_1003StarData()
            {
                atkDamage = 10,
                atkDamageType = EnDamageType.Magic,
                atkDamagePercentage = 0.30f,
                buffId = EnBuff.Burn,
                buffCount = 3,
                atkDuration = 5,
                atkInterval = 1f,
            },
            new GameBattleSkillBallPart_1003StarData()
            {
                atkDamage = 20,
                atkDamageType = EnDamageType.Magic,
                atkDamagePercentage = 0.50f,
                buffId = EnBuff.Burn,
                buffCount = 3,
                atkDuration = 7,
                atkInterval = 0.6f,
            },
            new GameBattleSkillBallPart_1003StarData()
            {
                atkDamage = 30,
                atkDamageType = EnDamageType.Magic,
                atkDamagePercentage = 0.80f,
                buffId = EnBuff.Burn,
                buffCount = 3,
                atkDuration = 10,
                atkInterval = 0.3f,
            },
        };
    }
}
