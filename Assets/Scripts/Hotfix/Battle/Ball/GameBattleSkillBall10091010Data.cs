using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBall10091010StarData : GameBattleSkillBallStarData
{
    public int damageValue;
    public float attPercentage01;
    public EnDamageType damageType;
    public EnEntityAttributeId attId;

    public int laserDamageValue;
    public float laserAttPercentage01;
    public EnDamageType laserDamageType;

    public float width;

    protected override void DeserializeFromIntArray2(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attPercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attId);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out laserDamageValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out laserAttPercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out laserDamageType);

        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out width);
    }

    protected override List<int> SerializeToIntArray2()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in damageValue);
        ISerializeToIntArray.SerializeToInt(ref list, in attPercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, damageType);
        ISerializeToIntArray.SerializeToInt(ref list, attId);
        ISerializeToIntArray.SerializeToInt(ref list, in laserDamageValue);
        ISerializeToIntArray.SerializeToInt(ref list, in laserAttPercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, laserDamageType);

        ISerializeToIntArray.SerializeToInt(ref list, in width);

        return list;
    }
    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);

        var laserName = LanguageUtility.GetLanguage(1210203);
        {
            var damageTypeName = LanguageUtility.GetLanguage_DamageTypeName(damageType);
            var attName = LanguageUtility.GetLanguage_AttiributeName(attId);
            var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
            descInfos.Add(info);
            info.title = LanguageUtility.GetLanguage_BallName(_BallId);
            info.level = level;
            info.content = string.Format(LanguageUtility.GetLanguage_BallDescription(_BallId),
                laserDamageValue, laserAttPercentage01 * 100, attName, damageTypeName, laserName, width);
        }

        {
            var damageTypeName = LanguageUtility.GetLanguage_DamageTypeName(laserDamageType);
            var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
            descInfos.Add(info);
            info.title = laserName;
            info.level = level;
            info.content = string.Format(LanguageUtility.GetLanguage(1210204),
                laserDamageValue, laserAttPercentage01 * 100, damageTypeName);
        }

    }
}
public class GameBattleSkillBall10091010Data : GameBattleSkillBallData<GameBattleSkillBall10091010EntityData, GameBattleSkillBall10091010StarData>
{

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
        {
            new GameBattleSkillBall10091010StarData(){
                damageValue = 21,
                attPercentage01 = 1.1f,
                damageType = EnDamageType.Physic,
                attId = EnEntityAttributeId.Attack,

                laserDamageValue = 20,
                laserAttPercentage01 = 1f,
                laserDamageType = EnDamageType.Magic,

                width = 0.3f,
            },
            new GameBattleSkillBall10091010StarData(){
                damageValue = 21,
                attPercentage01 = 1.1f,
                damageType = EnDamageType.Physic,
                attId = EnEntityAttributeId.Attack,

                laserDamageValue = 40,
                laserAttPercentage01 = 1.2f,
                laserDamageType = EnDamageType.Magic,

                width = 0.3f,
            },
            new GameBattleSkillBall10091010StarData(){
                damageValue = 21,
                attPercentage01 = 1.1f,
                damageType = EnDamageType.Physic,
                attId = EnEntityAttributeId.Attack,

                laserDamageValue = 80,
                laserAttPercentage01 = 1.4f,
                laserDamageType = EnDamageType.Magic,

                width = 0.3f,

                partInfos = new()
                {
                    new()
                    {
                        partId = 10091010,
                        partStar = 1,
                    },
                },
            },
        };
    }
}
