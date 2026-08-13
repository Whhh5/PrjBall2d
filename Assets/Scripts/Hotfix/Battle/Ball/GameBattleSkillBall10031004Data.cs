using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameBattleSkillBall10031004StarData : GameBattleSkillBallStarData
{
    public int damageValue;
    public float attPercentage01;
    public EnDamageType damageType;
    public EnEntityAttributeId attId;

    public float atkInterval;
    public float atkRadius;
    public float moveTime;

    public float star2DamagePercentage01;
    public EnDamageType star2DamageType;

    protected override void DeserializeFromIntArray2(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attPercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attId);

        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkInterval);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkRadius);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out moveTime);

        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out star2DamagePercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out star2DamageType);

    }

    protected override List<int> SerializeToIntArray2()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in damageValue);
        ISerializeToIntArray.SerializeToInt(ref list, in attPercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, damageType);
        ISerializeToIntArray.SerializeToInt(ref list, attId);

        ISerializeToIntArray.SerializeToInt(ref list, atkInterval);
        ISerializeToIntArray.SerializeToInt(ref list, atkRadius);
        ISerializeToIntArray.SerializeToInt(ref list, moveTime);

        ISerializeToIntArray.SerializeToInt(ref list, star2DamagePercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, star2DamageType);

        return list;
    }
    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);
        {
            var damageTypeName = LanguageUtility.GetLanguage_DamageTypeName(damageType);
            var attName = LanguageUtility.GetLanguage_AttiributeName(attId);
            var ballCfg = GameSchedule.Instance.GetSkillBallCfg0(_BallId);

            var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
            descInfos.Add(info);
            info.title = LanguageUtility.GetLanguage_BallName(_BallId);
            info.level = level;
            info.content = string.Format(LanguageUtility.GetLanguage_BallDescription(_BallId),
                atkRadius, moveTime, atkInterval, damageValue, attPercentage01 * 100, attName, damageTypeName, ballCfg.fDelayFireTime);
        }
        if (star2DamagePercentage01 > 0)
        {
            var damageTypeName = LanguageUtility.GetLanguage_DamageTypeName(star2DamageType);
            var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
            descInfos.Add(info);
            info.title = LanguageUtility.GetLanguage_10010();
            info.level = level;
            info.content = LanguageUtility.GetLanguage(1210603);

            var info2 = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
            descInfos.Add(info2);
            info2.title = LanguageUtility.GetLanguage(1210603);
            info2.level = level + 1;
            info2.content = string.Format(LanguageUtility.GetLanguage(1210604), star2DamagePercentage01 * 100, damageTypeName);
        }
    }
}
public class GameBattleSkillBall10031004Data : GameBattleSkillBallData<GameBattleSkillBall10031004EntityData, GameBattleSkillBall10031004StarData>
{
    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
        {
            new GameBattleSkillBall10031004StarData()
            {
                damageValue = 20,
                attPercentage01 = 0.1f,
                damageType = EnDamageType.Magic,
                attId = EnEntityAttributeId.Attack,

                atkInterval = 0.2f,
                atkRadius = 1f,
                moveTime = 5f,
            },
            new GameBattleSkillBall10031004StarData()
            {
                damageValue = 20,
                attPercentage01 = 0.3f,
                damageType = EnDamageType.Magic,
                attId = EnEntityAttributeId.Attack,

                atkInterval = 0.2f,
                atkRadius = 1f,
                moveTime = 5f,

                star2DamagePercentage01 = 0.2f,
                star2DamageType = EnDamageType.True,
            },
            new GameBattleSkillBall10031004StarData()
            {
                damageValue = 20,
                attPercentage01 = 0.6f,
                damageType = EnDamageType.Magic,
                attId = EnEntityAttributeId.Attack,

                atkInterval = 0.2f,
                atkRadius = 1f,
                moveTime = 5f,

                star2DamagePercentage01 = 0.5f,
                star2DamageType = EnDamageType.True,

                partInfos = new()
                {
                    new GameBattleSkillBallPartInfo()
                    {
                        partId = 10031004,
                        partStar = 1,
                    }
                },
            },
        };
    }
}
