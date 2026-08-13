using System.Collections.Generic;

public class GameBattleSkillBall1003StarData : GameBattleSkillBallStarData
{
    public int damageValue;
    public float attPercentage01;
    public EnDamageType damageType;
    public EnEntityAttributeId attId;

    public EntityBurnBuffData buffData;


    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        ClassPoolMgr.Instance.Push(buffData);
        buffData = null;
        attPercentage01
            = damageValue
            = -1;
        damageType = EnDamageType.None;
        attId = EnEntityAttributeId.None;
    }

    protected override List<int> SerializeToIntArray2()
    {
        var list = new List<int>();

        ISerializeToIntArray.SerializeToInt(ref list, in damageValue);
        ISerializeToIntArray.SerializeToInt(ref list, in attPercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, damageType);
        ISerializeToIntArray.SerializeToInt(ref list, attId);

        ISerializeToIntArray.SerializeToInt(ref list, in buffData);

        return list;
    }
    protected override void DeserializeFromIntArray2(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attPercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attId);

        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out buffData, in userData);
    }

    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);
        var damageTypeStr = LanguageUtility.GetLanguage_DamageTypeName(damageType);
        var ballCfg = GameSchedule.Instance.GetSkillBallCfg0(_BallId);
        var langStr = LanguageUtility.GetLanguage(ballCfg.nDescLangId);
        var buffCfg = GameSchedule.Instance.GetBuffCfg0((int)buffData.GetBuff());
        var bffName = LanguageUtility.GetLanguage(buffCfg.nStyleNameLangId);

        var attName = LanguageUtility.GetLanguage_AttiributeName(attId);

        var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
        info.level = level;
        info.title = LanguageUtility.GetLanguage(ballCfg.nNameLangId);
        info.content = string.Format(langStr, damageValue, attPercentage01 * 100, attName, damageTypeStr, bffName);
        descInfos.Add(info);

        buffData.GetDescriptionList(ref descInfos, level + 1);
    }
}
public class GameBattleSkillBall1003Data : GameBattleSkillBallData<GameBattleSkillBall1003EntityData, GameBattleSkillBall1003StarData>
{

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new()
        {
            new GameBattleSkillBall1003StarData()
            {
                damageValue = 10,
                attPercentage01 = 0.45f,
                damageType = EnDamageType.Physic,
                attId = EnEntityAttributeId.Attack,

                buffData = new EntityBurnBuffData()
                {
                    duration = 3,
                    damageType = EnDamageType.Magic,
                    damageValuePercentage01 = 0.01f,
                    targetAttribute = EnEntityAttributeId.LimitHp,
                    maxDamageValue = 20,
                    interval = 0.5f,
                    maxCount = 3,
                },
            },
            new GameBattleSkillBall1003StarData()
            {
                damageValue = 10,
                attPercentage01 = 0.6f,
                damageType = EnDamageType.Physic,
                attId = EnEntityAttributeId.Attack,

                buffData = new EntityBurnBuffData()
                {
                    duration = 3,
                    damageType = EnDamageType.Magic,
                    damageValuePercentage01 = 0.02f,
                    targetAttribute = EnEntityAttributeId.LimitHp,
                    maxDamageValue = 40,
                    interval = 0.5f,
                    maxCount = 3,
                },
            },
            new GameBattleSkillBall1003StarData()
            {
                damageValue = 10,
                attPercentage01 = 0.8f,
                damageType = EnDamageType.Physic,
                attId = EnEntityAttributeId.Attack,

                buffData = new EntityBurnBuffData()
                {
                    duration = 3,
                    damageType = EnDamageType.Magic,
                    damageValuePercentage01 = 0.03f,
                    targetAttribute = EnEntityAttributeId.LimitHp,
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
