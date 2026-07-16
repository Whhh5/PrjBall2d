using System.Collections.Generic;

public class GameBattleSkillBall_1010StarData : GameBattleSkillBallStarData
{
    public int damageValue;
    public float atkPercentage;
    public EnDamageType damageType;

    public float effectWidth;
    public int effectAtkDamage;
    public EnDamageType effectAtkDamageType;
    public float effectAtkPercentage;


    protected override List<int> SerializeToIntArray2()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in damageValue);
        ISerializeToIntArray.SerializeToInt(ref list, in atkPercentage);
        ISerializeToIntArray.SerializeToInt(ref list, damageType);

        ISerializeToIntArray.SerializeToInt(ref list, in effectWidth);
        ISerializeToIntArray.SerializeToInt(ref list, in effectAtkDamage);
        ISerializeToIntArray.SerializeToInt(ref list, effectAtkDamageType);
        ISerializeToIntArray.SerializeToInt(ref list, in effectAtkPercentage);
        return list;
    }
    protected override void DeserializeFromIntArray2(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkPercentage);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageType);

        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectWidth);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectAtkDamage);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectAtkDamageType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectAtkPercentage);
    }
}
public class GameBattleSkillBall_1010Data : GameBallData<GameBattleSkillBall_1010EntityData, GameBattleSkillBall_1010StarData>
{

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
                {
                    new GameBattleSkillBall_1010StarData(){
                        damageValue = 100,
                        atkPercentage = 1f,
                        damageType = EnDamageType.Physic,

                        effectWidth = 0.4f,
                        effectAtkDamage = 10,
                        effectAtkDamageType = EnDamageType.Magic,
                        effectAtkPercentage = 0.3f,
                    },
                    new GameBattleSkillBall_1010StarData(){
                        damageValue = 1,
                        atkPercentage = 1.2f,
                        damageType = EnDamageType.Physic,

                        effectWidth = 0.4f,
                        effectAtkDamage = 20,
                        effectAtkDamageType = EnDamageType.Magic,
                        effectAtkPercentage = 0.4f,
                    },
                    new GameBattleSkillBall_1010StarData(){
                        damageValue = 80,
                        atkPercentage = 1.4f,
                        damageType = EnDamageType.Physic,

                        effectWidth = 1f,
                        effectAtkDamage = 30,
                        effectAtkDamageType = EnDamageType.Magic,
                        effectAtkPercentage = 0.5f,

                        partInfos = new()
                        {
                            new GameBattleSkillBallPartInfo()
                            {
                                partId = 1010,
                                partStar = 1,
                            }
                        },
                    },
                };
    }
}
