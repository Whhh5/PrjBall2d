using System.Collections.Generic;

public class GameBattleSkillBall1002StarData : GameBattleSkillBallStarData
{
    public int damageValue;
    public float attPercentage01;
    public EnEntityAttributeId attId;
    public EnDamageType damageType;

    public int minCount;
    public int maxCount;
    public int fireProbability0100;
    public int subBallId;
    
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        minCount
            = maxCount
            = fireProbability0100
            = subBallId
            = -1;
        attId = EnEntityAttributeId.None;
    }

    protected override List<int> SerializeToIntArray2()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in damageValue);
        ISerializeToIntArray.SerializeToInt(ref list, in attPercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, damageType);
        ISerializeToIntArray.SerializeToInt(ref list, attId);

        ISerializeToIntArray.SerializeToInt(ref list, in minCount);
        ISerializeToIntArray.SerializeToInt(ref list, in maxCount);
        ISerializeToIntArray.SerializeToInt(ref list, in fireProbability0100);
        ISerializeToIntArray.SerializeToInt(ref list, in subBallId);
        return list;
    }
    protected override void DeserializeFromIntArray2(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attPercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attId);

        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out minCount);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out maxCount);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out fireProbability0100);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out subBallId);
    }

    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);

        var damageTypeStr = LanguageUtility.GetLanguage_DamageTypeName(damageType);
        var ballCfg = GameSchedule.Instance.GetSkillBallCfg0(_BallId);
        var langStr = LanguageUtility.GetLanguage(ballCfg.nDescLangId);

        var attName = LanguageUtility.GetLanguage_AttiributeName(attId);

        var subBallCfg = GameSchedule.Instance.GetSkillBallCfg0(subBallId);
        var subBallName = LanguageUtility.GetLanguage(subBallCfg.nNameLangId);

        var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
        info.level = level;
        info.title = LanguageUtility.GetLanguage(ballCfg.nNameLangId);
        info.content = string.Format(langStr, damageValue, attPercentage01 * 100, attName, damageTypeStr, fireProbability0100, minCount, maxCount, subBallName, _Star);
        descInfos.Add(info);

        var subBallStarData = GameBattleBallMgr.Instance.GetBallStarData(subBallId, _Star);
        subBallStarData.GetDescriptionList(ref descInfos, level + 1);
    }

}
public class GameBattleSkillBall1002Data : GameBattleSkillBallData<GameBattleSkillBall1002EntityData, GameBattleSkillBall1002StarData>
{

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new()
        {
            new GameBattleSkillBall1002StarData()
            {
                minCount = 1,
                maxCount = 2,
                fireProbability0100 = 50,
                subBallId = 101,

                damageValue = 20,
                attPercentage01 = 1.25f,
                damageType = EnDamageType.Physic,
                attId = EnEntityAttributeId.Attack,
            },
            new GameBattleSkillBall1002StarData()
            {
                minCount = 1,
                maxCount = 3,
                fireProbability0100 = 75,
                subBallId = 101,

                damageValue = 40,
                attPercentage01 = 1.5f,
                damageType = EnDamageType.Physic,
                attId = EnEntityAttributeId.Attack,
            },
            new GameBattleSkillBall1002StarData()
            {
                minCount = 2,
                maxCount = 5,
                fireProbability0100 = 100,
                subBallId = 101,

                damageValue = 80,
                attPercentage01 = 2f,
                damageType = EnDamageType.Physic,
                attId = EnEntityAttributeId.Attack,

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
