using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBallPart1001StarData : GameBattleSkillBallPartStarData
{
    public EnBuff buff;
    public int atkDamage;
    public EnDamageType atkDamageType;
    public float radius;

    public float atkVulnerableDamagePercentage;

    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out buff);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkDamage);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkDamageType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out radius);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkVulnerableDamagePercentage);
    }

    public override List<int> SerializeToIntArray()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, buff);
        ISerializeToIntArray.SerializeToInt(ref list, in atkDamage);
        ISerializeToIntArray.SerializeToInt(ref list, atkDamageType);
        ISerializeToIntArray.SerializeToInt(ref list, in radius);
        ISerializeToIntArray.SerializeToInt(ref list, in atkVulnerableDamagePercentage);

        return list;
    }

    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);


        var buffStyleName = LanguageUtility.GetLanguage_BuffStyleName(buff);
        var damageTypeName = LanguageUtility.GetLanguage_DamageTypeName(atkDamageType);

        var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
        info.level = level;
        info.title = LanguageUtility.GetLanguage_BallPartStyleName(_PartId);
        info.content = string.Format(LanguageUtility.GetLanguage_BallPartDescription(_PartId),
            buffStyleName, radius, atkDamage, atkVulnerableDamagePercentage * 100, damageTypeName);
        descInfos.Add(info);
    }
}
public class GameBattleSkillBallPart1001Data : GameBattleSkillBallPartData<GameBattleSkillBallPart1001EntityData, GameBattleSkillBallPart1001StarData>
{
    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
        {
            new GameBattleSkillBallPart1001StarData()
            {
                buff = EnBuff.Vulnerable,
                radius = 1.5f,
                atkDamage = 10,
                atkDamageType = EnDamageType.Magic,
                atkVulnerableDamagePercentage = 1,
            },
            new GameBattleSkillBallPart1001StarData()
            {
                buff = EnBuff.Vulnerable,
                radius = 1.5f,
                atkDamage = 20,
                atkDamageType = EnDamageType.Magic,
                atkVulnerableDamagePercentage = 2,
            },
            new GameBattleSkillBallPart1001StarData()
            {
                buff = EnBuff.Vulnerable,
                radius = 1.5f,
                atkDamage = 30,
                atkDamageType = EnDamageType.Magic,
                atkVulnerableDamagePercentage = 3,
            },
        };
    }
}
