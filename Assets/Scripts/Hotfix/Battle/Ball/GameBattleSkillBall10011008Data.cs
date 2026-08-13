using System.Collections.Generic;



public class GameBattleSkillBall10011008StarData : GameBattleSkillBallStarData
{
    public int damageValue;
    public float attPercentage01;
    public EnDamageType damageType;
    public EnEntityAttributeId attId;

    public int swordDamageValue;
    public float swordAttPercentage01;
    public EnDamageType swordDamageType;

    public int swordCount;
    public float atkRadius;

    public float star2BuffProbability0100;
    public EntityVulnerableBuffData star2BuffData;
    protected override void DeserializeFromIntArray2(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attPercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attId);

        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out swordDamageValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out swordAttPercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out swordDamageType);

        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out swordCount);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkRadius);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out star2BuffProbability0100);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out star2BuffData, userData);
    }

    protected override List<int> SerializeToIntArray2()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in damageValue);
        ISerializeToIntArray.SerializeToInt(ref list, in attPercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, damageType);
        ISerializeToIntArray.SerializeToInt(ref list, attId);

        ISerializeToIntArray.SerializeToInt(ref list, in swordDamageValue);
        ISerializeToIntArray.SerializeToInt(ref list, in swordAttPercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, swordDamageType);

        ISerializeToIntArray.SerializeToInt(ref list, in swordCount);
        ISerializeToIntArray.SerializeToInt(ref list, in atkRadius);
        ISerializeToIntArray.SerializeToInt(ref list, in star2BuffProbability0100);
        ISerializeToIntArray.SerializeToInt(ref list, in star2BuffData);
        return list;
    }
    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);
        {
            var swordName = LanguageUtility.GetLanguage(1210503);

            {
                var damageTypeName = LanguageUtility.GetLanguage_DamageTypeName(damageType);
                var attName = LanguageUtility.GetLanguage_AttiributeName(attId);
                var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
                descInfos.Add(info);
                info.title = LanguageUtility.GetLanguage_BallName(_BallId);
                info.level = level;
                info.content = string.Format(LanguageUtility.GetLanguage_BallDescription(_BallId),
                    damageValue, attPercentage01 * 100, attName, damageTypeName, swordCount, swordName);
            }
            {
                var swordDamageTypeName = LanguageUtility.GetLanguage_DamageTypeName(swordDamageType);
                var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
                descInfos.Add(info);
                info.title = swordName;
                info.level = level + 1;
                info.content = string.Format(LanguageUtility.GetLanguage(1210504),
                    swordDamageValue, swordAttPercentage01 * 100, swordDamageTypeName);
            }
        }

        if (star2BuffProbability0100 > 0)
        {
            var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
            descInfos.Add(info);
            info.title = LanguageUtility.GetLanguage_10010();
            info.level = level;
            info.content = LanguageUtility.GetLanguage(1210505);

            var buffName = LanguageUtility.GetLanguage_BuffStyleName(star2BuffData.GetBuff());
            var info3 = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
            descInfos.Add(info3);
            info3.title = LanguageUtility.GetLanguage(1210503);
            info3.level = level + 1;
            info3.content = string.Format(LanguageUtility.GetLanguage(1210506), star2BuffProbability0100, buffName);

            star2BuffData.GetDescriptionList(ref descInfos, level + 2);
        }
    }
}
public class GameBattleSkillBall10011008Data : GameBattleSkillBallData<GameBattleSkillBall10011008EntityData, GameBattleSkillBall10011008StarData>
{

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
        {
            new GameBattleSkillBall10011008StarData()
            {
                damageValue = 21,
                attPercentage01 = 1.1f,
                damageType = EnDamageType.Physic,
                attId = EnEntityAttributeId.Attack,

                swordDamageValue = 20,
                swordAttPercentage01 = 1f,
                swordDamageType = EnDamageType.Physic,

                swordCount = 1,
                atkRadius = 0.5f,
            },
            new GameBattleSkillBall10011008StarData()
            {
                damageValue = 20,
                attPercentage01 = 1f,
                damageType = EnDamageType.Physic,
                attId = EnEntityAttributeId.Attack,

                swordDamageValue = 20,
                swordAttPercentage01 = 1.2f,
                swordDamageType = EnDamageType.Physic,

                swordCount = 1,
                atkRadius = 0.5f,

                star2BuffProbability0100 = 50,
                star2BuffData = new()
                {
                    duration = 3,
                    damageType = EnDamageType.Physic,
                    donusDamagePercentage = 0.2f,
                    donusDamageType = EnDamageType.Magic,
                    maxCount = 5,
                },
            },
            new GameBattleSkillBall10011008StarData()
            {
                damageValue = 20,
                attPercentage01 = 1f,
                damageType = EnDamageType.Physic,
                attId = EnEntityAttributeId.Attack,

                swordDamageValue = 20,
                swordAttPercentage01 = 1.4f,
                swordDamageType = EnDamageType.Physic,

                swordCount = 1,
                atkRadius = 0.5f,

                star2BuffProbability0100 = 100,
                star2BuffData = new()
                {
                    duration = 3,
                    damageType = EnDamageType.Physic,
                    donusDamagePercentage = 0.5f,
                    donusDamageType = EnDamageType.Magic,
                    maxCount = 5,
                },

                partInfos = new()
                {
                    new GameBattleSkillBallPartInfo()
                    {
                        partId = 10011008,
                        partStar = 1,
                    }
                },
            },
        };
    }
}
