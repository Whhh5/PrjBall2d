using System.Collections.Generic;

public class GameBattleSkillBallPart10071012StarData : GameBattleSkillBallPartStarData
{
    public float radius;
    public float radiusStoringPercentage01;
    public int atkValue;
    public float attPercentage01;
    public float atkStoringPercentage01;
    public EnDamageType atkDamageType;
    public EnEntityAttributeId atkAttId;

    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out radius);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out radiusStoringPercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attPercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkStoringPercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkDamageType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkAttId);
    }

    public override List<int> SerializeToIntArray()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in radius);
        ISerializeToIntArray.SerializeToInt(ref list, in radiusStoringPercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, in atkValue);
        ISerializeToIntArray.SerializeToInt(ref list, in attPercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, in atkStoringPercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, atkDamageType);
        ISerializeToIntArray.SerializeToInt(ref list, atkAttId);
        return list;
    }

    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);

        var damageTypeName = LanguageUtility.GetLanguage_DamageTypeName(atkDamageType);
        var attName = LanguageUtility.GetLanguage_AttiributeName(atkAttId);

        var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
        descInfos.Add(info);
        info.title = LanguageUtility.GetLanguage_BallPartStyleName(_PartId);
        info.level = level;
        info.content = string.Format(LanguageUtility.GetLanguage_BallPartDescription(_PartId),
            radius, radiusStoringPercentage01 * 100, atkValue, attPercentage01 * 100, attName, atkStoringPercentage01 * 100, damageTypeName);

    }
}
public class GameBattleSkillBallPart10071012Data : GameBattleSkillBallPartData<GameBattleSkillBallPart10071012EntityData, GameBattleSkillBallPart10071012StarData>
{
    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
        {
            new GameBattleSkillBallPart10071012StarData()
            {
                radius = 0.5f,
                radiusStoringPercentage01 = 0.01f,
                atkValue = 10,
                attPercentage01 = 0.1f,
                atkStoringPercentage01 = 0.3f,
                atkDamageType = EnDamageType.True,
                atkAttId = EnEntityAttributeId.Attack,
            },
            new GameBattleSkillBallPart10071012StarData()
            {
                radius = 0.5f,
                radiusStoringPercentage01 = 0.01f,
                atkValue = 10,
                attPercentage01 = 0.1f,
                atkStoringPercentage01 = 0.3f,
                atkDamageType = EnDamageType.True,
                atkAttId = EnEntityAttributeId.Attack,
            },
            new GameBattleSkillBallPart10071012StarData()
            {
                radius = 0.5f,
                radiusStoringPercentage01 = 0.01f,
                atkValue = 10,
                attPercentage01 = 0.1f,
                atkStoringPercentage01 = 0.3f,
                atkDamageType = EnDamageType.True,
                atkAttId = EnEntityAttributeId.Attack,
            },
        };
    }
}
