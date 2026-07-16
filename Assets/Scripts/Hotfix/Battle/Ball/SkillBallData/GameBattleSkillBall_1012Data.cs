using System.Collections.Generic;

public class GameBattleSkillBall_1012StarData : GameBattleSkillBallStarData
{
    public int damageValue;
    public float atkPercentage;
    public EnDamageType damageType;

    public float effectInvokePercentage;
    public float effectRestoreHealthPercentage;

    protected override List<int> SerializeToIntArray2()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in damageValue);
        ISerializeToIntArray.SerializeToInt(ref list, in atkPercentage);
        ISerializeToIntArray.SerializeToInt(ref list, damageType);

        ISerializeToIntArray.SerializeToInt(ref list, effectInvokePercentage);
        ISerializeToIntArray.SerializeToInt(ref list, effectRestoreHealthPercentage);
        return list;
    }
    protected override void DeserializeFromIntArray2(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkPercentage);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageType);

        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectInvokePercentage);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectRestoreHealthPercentage);
    }

}
public class GameBattleSkillBall_1012Data : GameBallData<GameBattleSkillBall_1012EntityData, GameBattleSkillBall_1012StarData>
{

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
                {
                    new GameBattleSkillBall_1012StarData(){
                        damageValue = 20,
                        atkPercentage = 1f,
                        damageType = EnDamageType.Physic,

                        effectInvokePercentage = 0.05f,
                        effectRestoreHealthPercentage = 0.01f,
                    },
                    new GameBattleSkillBall_1012StarData(){
                        damageValue = 40,
                        atkPercentage = 1.2f,
                        damageType = EnDamageType.Physic,

                        effectInvokePercentage = 0.1f,
                        effectRestoreHealthPercentage = 0.03f,
                    },
                    new GameBattleSkillBall_1012StarData(){
                        damageValue = 80,
                        atkPercentage = 1.4f,
                        damageType = EnDamageType.Physic,

                        effectInvokePercentage = 0.2f,
                        effectRestoreHealthPercentage = 0.05f,

                        partInfos = new()
                        {
                            new GameBattleSkillBallPartInfo()
                            {
                                partId = 1012,
                                partStar = 1,
                            }
                        },
                    },
                };
    }
}
