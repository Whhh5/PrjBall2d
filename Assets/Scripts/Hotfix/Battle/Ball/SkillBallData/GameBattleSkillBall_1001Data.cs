using System.Collections.Generic;


public class GameBattleSkillBall_1001StarData : GameBattleSkillBallStarData
{
    public int damageValue;
    public float atkPercentage;
    public EnDamageType damageType;

    public IEntityBuffData buffData;

    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        ClassPoolMgr.Instance.Push(buffData);
        atkPercentage
            = damageValue
            = -1;
        damageType
            = EnDamageType.None;
        buffData = null;
    }

    protected override List<int> SerializeToIntArray2()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in damageValue);
        ISerializeToIntArray.SerializeToInt(ref list, in atkPercentage);
        ISerializeToIntArray.SerializeToInt(ref list, damageType);

        ISerializeToIntArray.SerializeToInt(ref list, in buffData);
        return list;
    }
    protected override void DeserializeFromIntArray2(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkPercentage);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageType);

        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out buffData, userData);
    }
}
public class GameBattleSkillBall_1001Data : GameBallData<GameBattleSkillBall_1001EntityData, GameBattleSkillBall_1001StarData>
{

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
                {
                    new GameBattleSkillBall_1001StarData(){
                        damageValue = 10,
                        damageType = EnDamageType.Physic,
                        atkPercentage = 0.1f,

                        buffData = new EntityVulnerableBuffData()
                        {
                            duration = 3,
                            damageType = EnDamageType.Physic,
                            donusDamagePercentage = 0.05f,
                            donusDamageType = EnDamageType.Magic,
                            maxCount = 3,
                        },
                    },
                    new GameBattleSkillBall_1001StarData(){
                        damageValue = 20,
                        damageType = EnDamageType.Physic,
                        atkPercentage = 0.2f,

                        buffData = new EntityVulnerableBuffData()
                        {
                            duration = 4,
                            damageType = EnDamageType.Physic,
                            donusDamagePercentage = 0.10f,
                            donusDamageType = EnDamageType.Magic,
                            maxCount = 3,
                        },
                    },
                    new GameBattleSkillBall_1001StarData(){
                        damageValue = 30,
                        damageType = EnDamageType.Physic,
                        atkPercentage = 0.3f,

                        buffData = new EntityVulnerableBuffData()
                        {
                            duration = 5,
                            damageType = EnDamageType.Physic,
                            donusDamagePercentage = 0.20f,
                            donusDamageType = EnDamageType.Magic,
                            maxCount = 3,
                        },

                        partInfos = new()
                        {
                            new GameBattleSkillBallPartInfo()
                            {
                                partId = 1001,
                                partStar = 1,
                            }
                        },
                    },
                };
    }
}
