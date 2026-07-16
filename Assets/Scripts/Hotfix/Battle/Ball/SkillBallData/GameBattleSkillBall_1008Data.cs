using System.Collections.Generic;

public class GameBattleSkillBall_1008StarData : GameBattleSkillBallStarData
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
public class GameBattleSkillBall_1008Data : GameBallData<GameBattleSkillBall_1008EntityData, GameBattleSkillBall_1008StarData>
{

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
                {
                    new GameBattleSkillBall_1008StarData(){
                        damageValue = 20,
                        atkPercentage = 1.5f,
                        damageType = EnDamageType.Physic,
                    },
                    new GameBattleSkillBall_1008StarData(){
                        damageValue = 40,
                        atkPercentage = 2f,
                        damageType = EnDamageType.Physic,
                    },
                    new GameBattleSkillBall_1008StarData(){
                        damageValue = 80,
                        atkPercentage = 2.5f,
                        damageType = EnDamageType.Physic,

                        partInfos = new()
                        {
                            new()
                            {
                                partId = 1008,
                                partStar = 1,
                            }
                        }
                    },
                };
    }
}
