using System.Collections.Generic;

public class GameBattleSkillBall_1007StarData : GameBattleSkillBallStarData
{
    public int damageValue;
    public float atkPercentage;
    public EnDamageType damageType;

    protected override List<int> SerializeToIntArray2()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in damageValue);
        ISerializeToIntArray.SerializeToInt(ref list, in atkPercentage);
        ISerializeToIntArray.SerializeToInt(ref list, damageType);
        return list;
    }
    protected override void DeserializeFromIntArray2(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkPercentage);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageType);
    }
}
public class GameBattleSkillBall_1007Data : GameBallData<GameBattleSkillBall_1007EntityData, GameBattleSkillBall_1007StarData>
{

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
                {
                    new GameBattleSkillBall_1007StarData(){
                        damageValue = 20,
                        atkPercentage = 1f,
                        damageType = EnDamageType.Physic,
                    },
                    new GameBattleSkillBall_1007StarData(){
                        damageValue = 40,
                        atkPercentage = 1.2f,
                        damageType = EnDamageType.Physic,
                    },
                    new GameBattleSkillBall_1007StarData(){
                        damageValue = 80,
                        atkPercentage = 1.4f,
                        damageType = EnDamageType.Physic,

                        partInfos = new()
                        {
                            new()
                            {
                                partId = 1007,
                                partStar = 1,
                            },
                        },
                    },
                };
    }
}
