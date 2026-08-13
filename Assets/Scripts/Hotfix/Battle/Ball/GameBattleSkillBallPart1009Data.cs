using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class GameBattleSkillBallPart1009StarData : GameBattleSkillBallPartStarData
{
    public int damageValue;
    public float atkPercentage01;
    public EnDamageType damageType;
    public EnEntityAttributeId attId;

    public float width;
    public int chargeCount;
    public float atkInterval;
    public int maxChargeCount;

    public override List<int> SerializeToIntArray()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in damageValue);
        ISerializeToIntArray.SerializeToInt(ref list, in atkPercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, damageType);
        ISerializeToIntArray.SerializeToInt(ref list, attId);

        ISerializeToIntArray.SerializeToInt(ref list, in width);
        ISerializeToIntArray.SerializeToInt(ref list, in chargeCount);
        ISerializeToIntArray.SerializeToInt(ref list, in atkInterval);
        ISerializeToIntArray.SerializeToInt(ref list, in maxChargeCount);
        return list;
    }
    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkPercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attId);

        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out width);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out chargeCount);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkInterval);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out maxChargeCount);
    }
    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);

        var depBallName = LanguageUtility.GetLanguage_BallName(1009);
        var damageTypeName = LanguageUtility.GetLanguage_DamageTypeName(damageType);
        var attName = LanguageUtility.GetLanguage_AttiributeName(attId);

        var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
        info.level = level;
        info.title = LanguageUtility.GetLanguage_BallPartStyleName(_PartId);
        info.content = string.Format(LanguageUtility.GetLanguage_BallPartDescription(_PartId),
            depBallName, chargeCount, maxChargeCount, atkInterval, width, damageValue, atkPercentage01 * 100, attName, damageTypeName);
        descInfos.Add(info);
    }
}
public class GameBattleSkillBallPart1009Data : GameBattleSkillBallPartData<GameBattleSkillBallPart1009EntityData, GameBattleSkillBallPart1009StarData>
{

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
        {
            new GameBattleSkillBallPart1009StarData()
            {
                 damageValue = 10,
                 damageType = EnDamageType.Magic,
                 atkPercentage01 = 0.45f,
                 attId = EnEntityAttributeId.Attack,
                 width = 0.4f,
                 atkInterval = 1f,
                 chargeCount = 1,
                 maxChargeCount = 3,
            },
            new GameBattleSkillBallPart1009StarData()
            {
                 damageValue = 20,
                 damageType = EnDamageType.Magic,
                 atkPercentage01 = 0.6f,
                 attId = EnEntityAttributeId.Attack,
                 width = 0.6f,
                 atkInterval = 0.6f,
                 chargeCount = 2,
                 maxChargeCount = 10,
            },
            new GameBattleSkillBallPart1009StarData()
            {
                 damageValue = 30,
                 damageType = EnDamageType.Magic,
                 atkPercentage01 = 1f,
                 attId = EnEntityAttributeId.Attack,
                 width = 1f,
                 atkInterval = 0.2f,
                 chargeCount = 3,
                 maxChargeCount = 20,
            },
        };
    }
}
