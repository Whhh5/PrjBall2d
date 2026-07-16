using System.Collections.Generic;

public class GameBattleSkillBall_1005StarData : GameBattleSkillBallStarData
{
    public int damageValue;
    public float atkPercentage;
    public EnDamageType damageType;

    public EnGameEffectId effectId;
    public int effectRowCount;
    public int effectColCount;
    public int effectAtkDamageValue;
    public float effectAtkPercentage;
    public EnDamageType effectAtkDamageType;


    protected override List<int> SerializeToIntArray2()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in damageValue);
        ISerializeToIntArray.SerializeToInt(ref list, in atkPercentage);
        ISerializeToIntArray.SerializeToInt(ref list, damageType);

        ISerializeToIntArray.SerializeToInt(ref list, effectId);
        ISerializeToIntArray.SerializeToInt(ref list, in effectRowCount);
        ISerializeToIntArray.SerializeToInt(ref list, in effectColCount);
        ISerializeToIntArray.SerializeToInt(ref list, in effectAtkDamageValue);
        ISerializeToIntArray.SerializeToInt(ref list, in effectAtkPercentage);
        ISerializeToIntArray.SerializeToInt(ref list, effectAtkDamageType);
        return list;
    }
    protected override void DeserializeFromIntArray2(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkPercentage);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageType);

        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectId);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectRowCount);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectColCount);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectAtkDamageValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectAtkPercentage);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectAtkDamageType);
    }
}
public class GameBattleSkillBall_1005Data : GameBallData<GameBattleSkillBall_1005EntityData, GameBattleSkillBall_1005StarData>
{

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
                {
                    new GameBattleSkillBall_1005StarData(){
                        damageValue = 20,
                        atkPercentage = 1f,
                        damageType = EnDamageType.Physic,

                        effectId = EnGameEffectId.Eff_buff_dizhen,
                        effectRowCount = 3,
                        effectColCount = 3,
                        effectAtkDamageValue = 10,
                        effectAtkPercentage = 0.1f,
                        effectAtkDamageType = EnDamageType.Magic,
                    },
                    new GameBattleSkillBall_1005StarData(){
                        damageValue = 40,
                        atkPercentage = 1.2f,
                        damageType = EnDamageType.Physic,

                        effectId = EnGameEffectId.Eff_buff_dizhen,
                        effectRowCount = 3,
                        effectColCount = 3,
                        effectAtkDamageValue = 20,
                        effectAtkPercentage = 0.2f,
                        effectAtkDamageType = EnDamageType.Magic,
                    },
                    new GameBattleSkillBall_1005StarData(){
                        damageValue = 80,
                        atkPercentage = 1.4f,
                        damageType = EnDamageType.Physic,

                        effectId = EnGameEffectId.Eff_buff_dizhen,
                        effectRowCount = 3,
                        effectColCount = 3,
                        effectAtkDamageValue = 30,
                        effectAtkPercentage = 0.3f,
                        effectAtkDamageType = EnDamageType.Magic,

                        partInfos = new()
                        {
                            new()
                            {
                                partId = 1005,
                                partStar = 1,
                            },
                        }
                    },
                };
    }
}
