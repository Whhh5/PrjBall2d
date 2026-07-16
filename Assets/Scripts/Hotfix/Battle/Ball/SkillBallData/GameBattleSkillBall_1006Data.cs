using System.Collections.Generic;

public class GameBattleSkillBall_1006StarData : GameBattleSkillBallStarData
{
    public int damageValue;
    public float atkPercentage;
    public EnDamageType damageType;

    public IEntityBuffData buffUserData;

    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        ClassPoolMgr.Instance.Push(buffUserData);
        buffUserData = null;
        atkPercentage
            = damageValue
            = -1;
    }
    protected override List<int> SerializeToIntArray2()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in damageValue);
        ISerializeToIntArray.SerializeToInt(ref list, in atkPercentage);
        ISerializeToIntArray.SerializeToInt(ref list, damageType);

        ISerializeToIntArray.SerializeToInt(ref list, in buffUserData);
        return list;
    }
    protected override void DeserializeFromIntArray2(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkPercentage);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageType);

        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out buffUserData, in userData);
    }

}
public class GameBattleSkillBall_1006Data : GameBallData<GameBattleSkillBall_1006EntityData, GameBattleSkillBall_1006StarData>
{

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
                {
                    new GameBattleSkillBall_1006StarData(){
                        damageValue = 20,
                        atkPercentage = 1.1f,
                        damageType = EnDamageType.Physic,

                        buffUserData = new EntityCrystalBuffData()
                        {
                            duration = 3,
                            speedReducePercentage = 0.1f,
                            maxCount = 3,
                        },
                    },
                    new GameBattleSkillBall_1006StarData(){
                        damageValue = 40,
                        atkPercentage = 1.2f,
                        damageType = EnDamageType.Physic,

                        buffUserData = new EntityCrystalBuffData()
                        {
                            duration = 4,
                            speedReducePercentage = 0.2f,
                            maxCount = 3,
                        },
                    },
                    new GameBattleSkillBall_1006StarData(){
                        damageValue = 80,
                        atkPercentage = 1.3f,
                        damageType = EnDamageType.Physic,

                        buffUserData = new EntityCrystalBuffData()
                        {
                            duration = 5,
                            speedReducePercentage = 0.3f,
                            maxCount = 3,
                        },

                        partInfos = new()
                        {
                            new()
                            {
                                partId = 1006,
                                partStar = 1,
                            }
                        }
                    },
                };
    }
}
