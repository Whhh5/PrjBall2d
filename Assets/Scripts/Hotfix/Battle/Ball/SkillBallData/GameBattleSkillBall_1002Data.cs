using System.Collections.Generic;

public class GameBattleSkillBall_1002StarData : GameBattleSkillBallStarData
{
    public int damageValue;
    public float atkPercentage;
    public EnDamageType damageType;

    public int minCount;
    public int maxCount;
    public int fireProbability;
    public int subBallId;
    
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        minCount
            = maxCount
            = fireProbability
            = subBallId
            = -1;
    }

    protected override List<int> SerializeToIntArray2()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in damageValue);
        ISerializeToIntArray.SerializeToInt(ref list, in atkPercentage);
        ISerializeToIntArray.SerializeToInt(ref list, damageType);

        ISerializeToIntArray.SerializeToInt(ref list, in minCount);
        ISerializeToIntArray.SerializeToInt(ref list, in maxCount);
        ISerializeToIntArray.SerializeToInt(ref list, in fireProbability);
        ISerializeToIntArray.SerializeToInt(ref list, in subBallId);
        return list;
    }
    protected override void DeserializeFromIntArray2(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkPercentage);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageType);

        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out minCount);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out maxCount);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out fireProbability);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out subBallId);
    }
}
public class GameBattleSkillBall_1002Data : GameBallData<GameBattleSkillBall_1002EntityData, GameBattleSkillBall_1002StarData>
{

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new()
        {
            new GameBattleSkillBall_1002StarData()
            {
                minCount = 1,
                maxCount = 2,
                fireProbability = 50,
                subBallId = 101,

                damageValue = 20,
                atkPercentage = 1.25f,
                damageType = EnDamageType.Physic,
            },
            new GameBattleSkillBall_1002StarData()
            {
                minCount = 1,
                maxCount = 3,
                fireProbability = 75,
                subBallId = 101,

                damageValue = 40,
                atkPercentage = 1.5f,
                damageType = EnDamageType.Physic,
            },
            new GameBattleSkillBall_1002StarData()
            {
                minCount = 2,
                maxCount = 5,
                fireProbability = 100,
                subBallId = 101,

                damageValue = 80,
                atkPercentage = 2f,
                damageType = EnDamageType.Physic,

                partInfos = new List<GameBattleSkillBallPartInfo>()
                {
                    new GameBattleSkillBallPartInfo()
                    {
                        partId = 1002,
                        partStar = 1,
                    }
                },
            },
        };
    }
}
