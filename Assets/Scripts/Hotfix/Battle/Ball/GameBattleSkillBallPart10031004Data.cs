using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBallPart10031004StarData : GameBattleSkillBallPartStarData
{
    public int atkDamage;
    public float atkPercentage01;
    public EnDamageType atkDamageType;
    public EnEntityAttributeId attId;

    public int duration;
    public float interval;
    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkDamage);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkPercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkDamageType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attId);

        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out interval);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out duration);
    }

    public override List<int> SerializeToIntArray()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in atkDamage);
        ISerializeToIntArray.SerializeToInt(ref list, in atkPercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, atkDamageType);
        ISerializeToIntArray.SerializeToInt(ref list, attId);

        ISerializeToIntArray.SerializeToInt(ref list, in interval);
        ISerializeToIntArray.SerializeToInt(ref list, in duration);
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
            duration, interval, atkDamage, atkPercentage01 * 100, attName, damageTypeName);
        descInfos.Add(info);

    }
}
public class GameBattleSkillBallPart10031004Data : GameBattleSkillBallPartData<GameBattleSkillBallPart10031004EntityData, GameBattleSkillBallPart10031004StarData>
{
    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
        {
            new GameBattleSkillBallPart10031004StarData(){
                atkDamage = 10,
                atkDamageType = EnDamageType.Magic,
                atkPercentage01 = 0.2f,
                attId = EnEntityAttributeId.Attack,

                duration = 2,
                interval = 0.5f,
            },
            new GameBattleSkillBallPart10031004StarData(){
                atkDamage = 10,
                atkDamageType = EnDamageType.Magic,
                atkPercentage01 = 0.4f,
                attId = EnEntityAttributeId.Attack,

                duration = 2,
                interval = 0.5f,
            },
            new GameBattleSkillBallPart10031004StarData(){
                atkDamage = 10,
                atkDamageType = EnDamageType.Magic,
                atkPercentage01 = 0.6f,
                attId = EnEntityAttributeId.Attack,

                duration = 2,
                interval = 0.5f,
            },
        };
    }
}
