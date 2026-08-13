using System.Collections.Generic;

public class GameBattleSkillBallPart1004StarData : GameBattleSkillBallPartStarData
{
    public int killProbablity0100;
    public float attPercentage01;
    public EnDamageType atkDamageType;
    public EnEntityAttributeId attId;

    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out killProbablity0100);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attPercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkDamageType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attId);
    }

    public override List<int> SerializeToIntArray()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in killProbablity0100);
        ISerializeToIntArray.SerializeToInt(ref list, in attPercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, atkDamageType);
        ISerializeToIntArray.SerializeToInt(ref list, attId);
        return list;
    }

    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);

        var attName = LanguageUtility.GetLanguage_AttiributeName(attId);
        var damageTypeName = LanguageUtility.GetLanguage_DamageTypeName(atkDamageType);

        var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
        info.title = LanguageUtility.GetLanguage_BallPartStyleName(_PartId);
        info.level = level;
        info.content = string.Format(LanguageUtility.GetLanguage_BallPartDescription(_PartId),
            killProbablity0100, attPercentage01 * 100, attName, damageTypeName);
        descInfos.Add(info);
    }
}
public class GameBattleSkillBallPart1004Data : GameBattleSkillBallPartData<GameBattleSkillBallPart1004EntityData, GameBattleSkillBallPart1004StarData>
{
    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new()
        {
            new GameBattleSkillBallPart1004StarData()
            {
                killProbablity0100 = 10,
                attPercentage01 = 1,
                atkDamageType = EnDamageType.True,
                attId = EnEntityAttributeId.LimitHp,
            },
            new GameBattleSkillBallPart1004StarData()
            {
                killProbablity0100 = 20,
                attPercentage01 = 1,
                atkDamageType = EnDamageType.True,
                attId = EnEntityAttributeId.LimitHp,
            },
            new GameBattleSkillBallPart1004StarData()
            {
                killProbablity0100 = 30,
                attPercentage01 = 1,
                atkDamageType = EnDamageType.True,
                attId = EnEntityAttributeId.LimitHp,
            },
        };
    }
}
