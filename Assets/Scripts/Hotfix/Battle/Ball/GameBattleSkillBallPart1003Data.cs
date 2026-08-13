using System.Collections.Generic;

public class GameBattleSkillBallPart1003StarData : GameBattleSkillBallPartStarData
{
    public int atkDamage;
    public EnDamageType atkDamageType;
    public float attPercentage01;
    public EnEntityAttributeId attId;

    public EnBuff buffId;
    public int buffCount;
    public float atkDuration;
    public float atkInterval;

    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out buffId);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out buffCount);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attId);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attPercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkDamage);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkDamageType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkDuration);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkInterval);
    }

    public override List<int> SerializeToIntArray()
    {
        var list = new List<int>();

        ISerializeToIntArray.SerializeToInt(ref list, buffId);
        ISerializeToIntArray.SerializeToInt(ref list, in buffCount);
        ISerializeToIntArray.SerializeToInt(ref list, attId);
        ISerializeToIntArray.SerializeToInt(ref list, in attPercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, in atkDamage);
        ISerializeToIntArray.SerializeToInt(ref list, atkDamageType);
        ISerializeToIntArray.SerializeToInt(ref list, in atkDuration);
        ISerializeToIntArray.SerializeToInt(ref list, in atkInterval);

        return list;
    }
    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);

        var buffStyleName = LanguageUtility.GetLanguage_BuffStyleName(buffId);
        var damageTypeName = LanguageUtility.GetLanguage_DamageTypeName(atkDamageType);

        var attName = LanguageUtility.GetLanguage_AttiributeName(attId);

        var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
        info.level = level;
        info.title = LanguageUtility.GetLanguage_BallPartStyleName(_PartId);
        info.content = string.Format(LanguageUtility.GetLanguage_BallPartDescription(_PartId),
            buffStyleName, buffCount, atkInterval, atkDamage, attPercentage01 * 100, attName, damageTypeName, atkDuration);
        descInfos.Add(info);
    }
}
public class GameBattleSkillBallPart1003Data : GameBattleSkillBallPartData<GameBattleSkillBallPart1003EntityData, GameBattleSkillBallPart1003StarData>
{
    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
        {
            new GameBattleSkillBallPart1003StarData()
            {
                atkDamage = 10,
                atkDamageType = EnDamageType.Magic,
                attId = EnEntityAttributeId.Attack,
                attPercentage01 = 0.30f,
                buffId = EnBuff.Burn,
                buffCount = 3,
                atkDuration = 5,
                atkInterval = 1f,
            },
            new GameBattleSkillBallPart1003StarData()
            {
                atkDamage = 20,
                atkDamageType = EnDamageType.Magic,
                attId = EnEntityAttributeId.Attack,
                attPercentage01 = 0.50f,
                buffId = EnBuff.Burn,
                buffCount = 3,
                atkDuration = 7,
                atkInterval = 0.6f,
            },
            new GameBattleSkillBallPart1003StarData()
            {
                atkDamage = 30,
                atkDamageType = EnDamageType.Magic,
                attId = EnEntityAttributeId.Attack,
                attPercentage01 = 0.80f,
                buffId = EnBuff.Burn,
                buffCount = 3,
                atkDuration = 10,
                atkInterval = 0.3f,
            },
        };
    }
}
