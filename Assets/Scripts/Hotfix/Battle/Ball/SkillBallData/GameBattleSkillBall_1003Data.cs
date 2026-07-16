using System.Collections.Generic;

public class GameBattleSkillBall_1003StarData : GameBattleSkillBallStarData
{
    public int damageValue;
    public float atkPercentage;
    public EnDamageType damageType;

    public IEntityBuffData buffData;


    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        ClassPoolMgr.Instance.Push(buffData);
        buffData = null;
        atkPercentage
            = damageValue
            = -1;
        damageType = EnDamageType.None;
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

        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out buffData, in userData);
    }
}
public class GameBattleSkillBall_1003Data : GameBallData<GameBattleSkillBall_1003EntityData, GameBattleSkillBall_1003StarData>
{

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new()
        {
            new GameBattleSkillBall_1003StarData()
            {
                damageValue = 10,
                atkPercentage = 0.45f,
                damageType = EnDamageType.Physic,

                buffData = new EntityBurnBuffData()
                {
                    duration = 3,
                    damageType = EnDamageType.Magic,
                    damageValuePercentage = 0.01f,
                    maxDamageValue = 20,
                    interval = 0.5f,
                    maxCount = 3,
                },
            },
            new GameBattleSkillBall_1003StarData()
            {
                damageValue = 10,
                atkPercentage = 0.6f,
                damageType = EnDamageType.Physic,

                buffData = new EntityBurnBuffData()
                {
                    duration = 3,
                    damageType = EnDamageType.Magic,
                    damageValuePercentage = 0.02f,
                    maxDamageValue = 40,
                    interval = 0.5f,
                    maxCount = 3,
                },
            },
            new GameBattleSkillBall_1003StarData()
            {
                damageValue = 10,
                atkPercentage = 0.8f,
                damageType = EnDamageType.Physic,

                buffData = new EntityBurnBuffData()
                {
                    duration = 3,
                    damageType = EnDamageType.Magic,
                    damageValuePercentage = 0.03f,
                    maxDamageValue = 60,
                    interval = 0.5f,
                    maxCount = 3,
                },

                partInfos = new List<GameBattleSkillBallPartInfo>()
                {
                    new GameBattleSkillBallPartInfo()
                    {
                        partId = 1003,
                        partStar = 1,
                    }
                },
            },
        };
    }
}
