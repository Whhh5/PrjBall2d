using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBall10071012StarData : GameBattleSkillBallStarData
{
    public int damageValue;
    public float attPercentage01;
    public EnDamageType damageType;
    public EnEntityAttributeId attId;

    public int leechDamageValue;
    public float leechAttPercentage01;
    public EnDamageType leechDamageType;

    public float storingPercentage01;

    public int createCount;
    public float atkInterval;
    public int maxStoringValue;
    public float addStoringPercentage01;
    public EnEntityAttributeId addAttribute;


    // star2
    public int star2TestStoringValue;
    public EntityBleedBuffData star2BuffData;

    protected override void DeserializeFromIntArray2(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attPercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attId);

        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out leechDamageValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out leechAttPercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out storingPercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out leechDamageType);

        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out maxStoringValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out addStoringPercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkInterval);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out createCount);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out addAttribute);


        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out star2TestStoringValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out star2BuffData, userData);
    }

    protected override List<int> SerializeToIntArray2()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in damageValue);
        ISerializeToIntArray.SerializeToInt(ref list, in attPercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, damageType);
        ISerializeToIntArray.SerializeToInt(ref list, attId);

        ISerializeToIntArray.SerializeToInt(ref list, in leechDamageValue);
        ISerializeToIntArray.SerializeToInt(ref list, in leechAttPercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, in storingPercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, leechDamageType);

        ISerializeToIntArray.SerializeToInt(ref list, in maxStoringValue);
        ISerializeToIntArray.SerializeToInt(ref list, in addStoringPercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, in atkInterval);
        ISerializeToIntArray.SerializeToInt(ref list, in createCount);
        ISerializeToIntArray.SerializeToInt(ref list, addAttribute);


        ISerializeToIntArray.SerializeToInt(ref list, in star2TestStoringValue);
        ISerializeToIntArray.SerializeToInt(ref list, in star2BuffData);

        return list;
    }
    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);
        {
            var leechName = LanguageUtility.GetLanguage(1210403);

            {
                var attName = LanguageUtility.GetLanguage_AttiributeName(attId);
                var damageTypeName = LanguageUtility.GetLanguage_DamageTypeName(damageType);
                var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
                descInfos.Add(info);
                info.title = LanguageUtility.GetLanguage_BallName(_BallId);
                info.level = level;
                info.content = string.Format(LanguageUtility.GetLanguage_BallDescription(_BallId),
                    damageValue, attPercentage01 * 100, attName, damageTypeName, createCount, leechName);

            }
            {
                var addAttName = LanguageUtility.GetLanguage_AttiributeName(addAttribute);
                var leechDamageTypeName = LanguageUtility.GetLanguage_DamageTypeName(leechDamageType);
                var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
                descInfos.Add(info);
                info.title = leechName;
                info.level = level + 1;
                info.content = string.Format(LanguageUtility.GetLanguage(1210404),
                    atkInterval, leechDamageValue, leechAttPercentage01 * 100, storingPercentage01 * 100, leechDamageTypeName, maxStoringValue, addStoringPercentage01 * 100, addAttName);

            }
        }
        if (star2TestStoringValue > 0)
        {
            var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
            descInfos.Add(info);
            info.title = LanguageUtility.GetLanguage_10010();
            info.level = level;
            info.content = LanguageUtility.GetLanguage(1210405);

            var buffName = LanguageUtility.GetLanguage_BuffStyleName(star2BuffData.GetBuff());
            var info3 = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
            descInfos.Add(info3);
            info3.title = LanguageUtility.GetLanguage(1210405);
            info3.level = level + 1;
            info3.content = string.Format(LanguageUtility.GetLanguage(1210406), star2TestStoringValue, buffName);

            star2BuffData.GetDescriptionList(ref descInfos, level + 2);
        }
    }
}
public class GameBattleSkillBall10071012Data : GameBattleSkillBallData<GameBattleSkillBall10071012EntityData, GameBattleSkillBall10071012StarData>
{
    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
        {
            new GameBattleSkillBall10071012StarData(){
                damageValue = 21,
                attPercentage01 = 1.1f,
                damageType = EnDamageType.Physic,
                attId = EnEntityAttributeId.Attack,

                leechDamageValue = 20,
                leechAttPercentage01 = 1f,
                storingPercentage01 = 0.2f,
                leechDamageType = EnDamageType.Magic,

                createCount = 1,
                atkInterval = 1f,
                maxStoringValue = 50,
                addStoringPercentage01 = 0.1f,
                addAttribute = EnEntityAttributeId.Hp,
            },
            new GameBattleSkillBall10071012StarData(){
                damageValue = 20,
                attPercentage01 = 1f,
                damageType = EnDamageType.Physic,
                attId = EnEntityAttributeId.Attack,

                leechDamageValue = 40,
                leechAttPercentage01 = 1.2f,
                storingPercentage01 = 0.3f,
                leechDamageType = EnDamageType.Magic,

                createCount = 1,
                atkInterval = 1f,
                maxStoringValue = 100,
                addStoringPercentage01 = 0.2f,
                addAttribute = EnEntityAttributeId.Hp,

                star2TestStoringValue = 50,
                star2BuffData = new()
                {
                    duration = 5,
                    damageType = EnDamageType.True,
                    damageValuePercentage01 = 0.01f,
                    maxDamageValue = 100,
                    interval = 0.2f,
                    maxCount = 5,
                    targetAttribute = EnEntityAttributeId.LimitHp,
                },

            },
            new GameBattleSkillBall10071012StarData(){
                damageValue = 20,
                attPercentage01 = 1f,
                damageType = EnDamageType.Physic,
                attId = EnEntityAttributeId.Attack,

                leechDamageValue = 80,
                leechAttPercentage01 = 1.4f,
                storingPercentage01 = 0.4f,
                leechDamageType = EnDamageType.Magic,

                createCount = 1,
                atkInterval = 1f,
                maxStoringValue = 200,
                addStoringPercentage01 = 0.3f,
                addAttribute = EnEntityAttributeId.Hp,

                star2TestStoringValue = 100,
                star2BuffData = new()
                {
                    duration = 5,
                    damageType = EnDamageType.True,
                    damageValuePercentage01 = 0.02f,
                    maxDamageValue = 100,
                    interval = 0.2f,
                    maxCount = 5,
                    targetAttribute = EnEntityAttributeId.LimitHp,
                },


                partInfos = new()
                {
                    new()
                    {
                        partId = 10071012,
                        partStar = 1,
                    },
                },
            },
        };
    }
}
