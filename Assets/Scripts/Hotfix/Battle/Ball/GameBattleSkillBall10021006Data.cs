using System.Collections.Generic;


public class GameBattleSkillBall10021006StarData : GameBattleSkillBallStarData
{
    public int damageValue;
    public float attPercentage01;
    public EnDamageType damageType;
    public EnEntityAttributeId attId;

    public int lciclcDamageValue;
    public float lciclcAttPercentage01;
    public EnDamageType lciclcDamageType;

    public int lciclcCount;
    public EntityCrystalBuffData star2BuffData;
    public int star3BuffProbability0100;
    public EntityFreezeBuffData star3BuffData;
    public float radius;
    protected override void DeserializeFromIntArray2(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attPercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attId);

        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out lciclcDamageValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out lciclcAttPercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out lciclcDamageType);

        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out lciclcCount);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out star2BuffData, userData);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out star3BuffProbability0100);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out star3BuffData, userData);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out radius);
    }

    protected override List<int> SerializeToIntArray2()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in damageValue);
        ISerializeToIntArray.SerializeToInt(ref list, in attPercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, damageType);
        ISerializeToIntArray.SerializeToInt(ref list, attId);

        ISerializeToIntArray.SerializeToInt(ref list, in lciclcDamageValue);
        ISerializeToIntArray.SerializeToInt(ref list, in lciclcAttPercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, lciclcDamageType);
        ISerializeToIntArray.SerializeToInt(ref list, in lciclcCount);
        ISerializeToIntArray.SerializeToInt(ref list, in star2BuffData);
        ISerializeToIntArray.SerializeToInt(ref list, in star3BuffProbability0100);
        ISerializeToIntArray.SerializeToInt(ref list, in star3BuffData);
        ISerializeToIntArray.SerializeToInt(ref list, in radius);
        return list;
    }

    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);

        {
            var lciclcName = LanguageUtility.GetLanguage(1210303);
            {
                var damageTypeName = LanguageUtility.GetLanguage_DamageTypeName(damageType);
                var attName = LanguageUtility.GetLanguage_AttiributeName(attId);

                var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
                descInfos.Add(info);
                info.title = LanguageUtility.GetLanguage_BallName(_BallId);
                info.level = level;
                info.content = string.Format(LanguageUtility.GetLanguage_BallDescription(_BallId),
                    damageValue, attPercentage01 * 100, attName, damageTypeName, lciclcCount, lciclcName);
            }
            {
                var lciclcDamageTypeName = LanguageUtility.GetLanguage_DamageTypeName(lciclcDamageType);

                var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
                descInfos.Add(info);
                info.title = lciclcName;
                info.level = level + 1;
                info.content = string.Format(LanguageUtility.GetLanguage(1210304),
                    lciclcDamageValue, lciclcAttPercentage01 * 100, lciclcDamageTypeName);
            }
        }
        if (star2BuffData != null)
        {
            var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
            descInfos.Add(info);
            info.title = LanguageUtility.GetLanguage_10010();
            info.level = level;
            info.content = LanguageUtility.GetLanguage(1210305);

            var buffName = LanguageUtility.GetLanguage_BuffStyleName(star2BuffData.GetBuff());
            var info3 = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
            descInfos.Add(info3);
            info3.title = LanguageUtility.GetLanguage(1210305);
            info3.level = level + 1;
            info3.content = string.Format(LanguageUtility.GetLanguage(1210306), buffName);

            star2BuffData.GetDescriptionList(ref descInfos, level + 2);
        }
        if (star3BuffProbability0100 > 0)
        {
            var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
            descInfos.Add(info);
            info.title = LanguageUtility.GetLanguage_10010();
            info.level = level;
            info.content = LanguageUtility.GetLanguage(1210307);

            var buffName = LanguageUtility.GetLanguage_BuffStyleName(star3BuffData.GetBuff());
            var info3 = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
            descInfos.Add(info3);
            info3.title = LanguageUtility.GetLanguage(1210307);
            info3.level = level + 1;
            info3.content = string.Format(LanguageUtility.GetLanguage(1210308), star3BuffProbability0100, buffName);

            star3BuffData.GetDescriptionList(ref descInfos, level + 2);
        }
    }
}
public class GameBattleSkillBall10021006Data : GameBattleSkillBallData<GameBattleSkillBall10021006EntityData, GameBattleSkillBall10021006StarData>
{
    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
        {
            new GameBattleSkillBall10021006StarData(){
                damageValue = 21,
                attPercentage01 = 1.1f,
                damageType = EnDamageType.Physic,
                attId = EnEntityAttributeId.Attack,

                lciclcDamageValue = 20,
                lciclcAttPercentage01 = 1f,
                lciclcDamageType = EnDamageType.Magic,

                lciclcCount = 3,
                radius = 0.12f,
            },
            new GameBattleSkillBall10021006StarData(){
                damageValue = 21,
                attPercentage01 = 1.1f,
                damageType = EnDamageType.Physic,
                attId = EnEntityAttributeId.Attack,

                lciclcDamageValue = 40,
                lciclcAttPercentage01 = 1.2f,
                lciclcDamageType = EnDamageType.Magic,

                lciclcCount = 4,
                radius = 0.12f,
                star2BuffData = new()
                {
                    duration = 1f,
                    attId = EnEntityAttributeId.MoveSpeed,
                    maxCount = 3,
                    speedReducePercentage01 = 0.1f,
                },
            },
            new GameBattleSkillBall10021006StarData(){
                damageValue = 21,
                attPercentage01 = 1.1f,
                damageType = EnDamageType.Physic,
                attId = EnEntityAttributeId.Attack,

                lciclcDamageValue = 80,
                lciclcAttPercentage01 = 1.4f,
                lciclcDamageType = EnDamageType.Magic,

                lciclcCount = 5,
                radius = 0.12f,
                star2BuffData = new()
                {
                    duration = 1f,
                    attId = EnEntityAttributeId.MoveSpeed,
                    maxCount = 3,
                    speedReducePercentage01 = 0.1f,
                },
                star3BuffProbability0100 = 100,
                star3BuffData = new()
                {
                    duration = 1f,
                    attId = EnEntityAttributeId.MoveSpeed,
                    speedReducePercentage01 = 1,
                },

                partInfos = new()
                {
                    new()
                    {
                        partId = 10021006,
                        partStar = 1,
                    },
                },
            },
        };
    }
}
