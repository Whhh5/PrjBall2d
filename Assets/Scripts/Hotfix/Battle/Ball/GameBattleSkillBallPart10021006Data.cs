using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBallPart10021006StarData : GameBattleSkillBallPartStarData
{
    public int damageValue;
    public EnDamageType damageType;
    public float damageProcentage01;
    public float radius;
    public int atkProbability0100;

    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageProcentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out radius);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkProbability0100);
    }

    public override List<int> SerializeToIntArray()
    {
        var list = new List<int>();

        ISerializeToIntArray.SerializeToInt(ref list, in damageValue);
        ISerializeToIntArray.SerializeToInt(ref list, damageType);
        ISerializeToIntArray.SerializeToInt(ref list, in damageProcentage01);
        ISerializeToIntArray.SerializeToInt(ref list, in radius);
        ISerializeToIntArray.SerializeToInt(ref list, in atkProbability0100);
        return list;
    }
    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);

        var attName = LanguageUtility.GetLanguage_DamageTypeName(damageType);

        var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
        info.level = level;
        info.title = LanguageUtility.GetLanguage_BallPartStyleName(_PartId);
        info.content = string.Format(LanguageUtility.GetLanguage_BallPartDescription(_PartId), atkProbability0100, radius, damageValue, damageProcentage01 * 100, attName);
        descInfos.Add(info);
    }
}
public class GameBattleSkillBallPart10021006Data : GameBattleSkillBallPartData<GameBattleSkillBallPart10021006EntityData, GameBattleSkillBallPart10021006StarData>
{
    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new()
        {
            new GameBattleSkillBallPart10021006StarData()
            {
                damageValue = 10,
                damageType = EnDamageType.True,
                damageProcentage01 = 1,
                radius = 1.5f,
                atkProbability0100 = 50,
            },
            new GameBattleSkillBallPart10021006StarData()
            {
                damageValue = 20,
                damageType = EnDamageType.True,
                damageProcentage01 = 1,
                radius = 1.5f,
                atkProbability0100 = 75,
            },
            new GameBattleSkillBallPart10021006StarData()
            {
                damageValue = 30,
                damageType = EnDamageType.True,
                damageProcentage01 = 1,
                radius = 3f,
                atkProbability0100 = 100,
            },
        };
    }
}
