using System.Collections.Generic;

public class GameBattleSkillBall_1011StarData : GameBattleSkillBallStarData
{
    public int damageValue;
    public float atkPercentage;
    public EnDamageType damageType;

    public int effectRowCount;
    public int effectColCount;
    public float effectAtkPercentage;
    public int effectAtkDamage;
    public EnDamageType effectAtkDamageType;
    public int effectAtkCount;
    public float effectInterval;


    protected override List<int> SerializeToIntArray2()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in damageValue);
        ISerializeToIntArray.SerializeToInt(ref list, in atkPercentage);
        ISerializeToIntArray.SerializeToInt(ref list, damageType);

        ISerializeToIntArray.SerializeToInt(ref list, in effectRowCount);
        ISerializeToIntArray.SerializeToInt(ref list, in effectColCount);
        ISerializeToIntArray.SerializeToInt(ref list, in effectAtkPercentage);
        ISerializeToIntArray.SerializeToInt(ref list, in effectAtkDamage);
        ISerializeToIntArray.SerializeToInt(ref list, in effectAtkCount);
        ISerializeToIntArray.SerializeToInt(ref list, effectAtkDamageType);
        ISerializeToIntArray.SerializeToInt(ref list, in effectInterval);
        return list;
    }
    protected override void DeserializeFromIntArray2(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkPercentage);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageType);

        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectRowCount);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectColCount);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectAtkPercentage);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectAtkDamage);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectAtkCount);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectAtkDamageType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectInterval);
    }
}
public class GameBattleSkillBall_1011Data : GameBallData<GameBattleSkillBall_1011EntityData, GameBattleSkillBall_1011StarData>
{

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
                {
                    new GameBattleSkillBall_1011StarData(){
                        damageValue = 20,
                        atkPercentage = 1f,
                        damageType = EnDamageType.Physic,

                        effectRowCount = 3,
                        effectColCount = 3,
                        effectAtkPercentage = 0.1f,
                        effectAtkDamage = 20,
                        effectAtkDamageType = EnDamageType.Magic,
                        effectAtkCount = 3,
                        effectInterval = 0.1f,
                    },
                    new GameBattleSkillBall_1011StarData(){
                        damageValue = 40,
                        atkPercentage = 1.2f,
                        damageType = EnDamageType.Physic,

                        effectRowCount = 3,
                        effectColCount = 3,
                        effectAtkPercentage = 0.2f,
                        effectAtkDamage = 30,
                        effectAtkDamageType = EnDamageType.Magic,
                        effectAtkCount = 4,
                        effectInterval = 0.1f,
                    },
                    new GameBattleSkillBall_1011StarData(){
                        damageValue = 80,
                        atkPercentage = 1.4f,
                        damageType = EnDamageType.Physic,

                        effectRowCount = 5,
                        effectColCount = 5,
                        effectAtkPercentage = 0.3f,
                        effectAtkDamage = 40,
                        effectAtkDamageType = EnDamageType.Magic,
                        effectAtkCount = 5,
                        effectInterval = 0.1f,

                        partInfos = new()
                        {
                            new GameBattleSkillBallPartInfo()
                            {
                                partId = 1011,
                                partStar = 1,
                            }
                        },
                    },
                };
    }
}
