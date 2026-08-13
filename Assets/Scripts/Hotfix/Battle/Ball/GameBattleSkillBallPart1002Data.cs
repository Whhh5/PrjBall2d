using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBallPart1002StarData : GameBattleSkillBallPartStarData
{
    public int atkDamage;
    public float atkPercentage01;
    public EnDamageType atkDamageType;
    public EnEntityAttributeId attId;
    public float radius;
    public int fireProbability0100;

    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkDamage);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkPercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkDamageType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attId);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out radius);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out fireProbability0100);
    }


    public override List<int> SerializeToIntArray()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in atkDamage);
        ISerializeToIntArray.SerializeToInt(ref list, in atkPercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, atkDamageType);
        ISerializeToIntArray.SerializeToInt(ref list, attId);
        ISerializeToIntArray.SerializeToInt(ref list, in radius);
        ISerializeToIntArray.SerializeToInt(ref list, in fireProbability0100);

        return list;
    }

    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);

        var damageTypeName = LanguageUtility.GetLanguage_DamageTypeName(atkDamageType);
        var attName = LanguageUtility.GetLanguage_AttiributeName(attId);

        var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
        info.level = level;
        info.title = LanguageUtility.GetLanguage_BallPartStyleName(_PartId);
        info.content = string.Format(LanguageUtility.GetLanguage_BallPartDescription(_PartId),
            fireProbability0100, radius, atkDamage, atkPercentage01 * 100, attName, damageTypeName);
        descInfos.Add(info);
    }
}
public class GameBattleSkillBallPart1002Data : GameBattleSkillBallPartData<GameBattleSkillBallPart1002EntityData, GameBattleSkillBallPart1002StarData>
{
    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
        {
            new GameBattleSkillBallPart1002StarData()
            {
                atkDamage = 10,
                atkDamageType = EnDamageType.Magic,
                atkPercentage01 = 0.5f,
                attId = EnEntityAttributeId.Attack,
                radius = 1.5f,
                fireProbability0100 = 20,
            },
            new GameBattleSkillBallPart1002StarData()
            {
                atkDamage = 20,
                atkDamageType = EnDamageType.Magic,
                atkPercentage01 = 0.5f,
                attId = EnEntityAttributeId.Attack,
                radius = 1.5f,
                fireProbability0100 = 40,
            },
            new GameBattleSkillBallPart1002StarData()
            {
                atkDamage = 30,
                atkDamageType = EnDamageType.Magic,
                atkPercentage01 = 0.5f,
                attId = EnEntityAttributeId.Attack,
                radius = 1.5f,
                fireProbability0100 = 80,
            },
        };
    }
}
