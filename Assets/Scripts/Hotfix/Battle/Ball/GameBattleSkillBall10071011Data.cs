using System.Collections.Generic;

public class GameBattleSkillBall10071011StarData : GameBattleSkillBallStarData
{
    public int damageValue;
    public float attPercentage01;
    public EnDamageType damageType;
    public EnEntityAttributeId attId;

    public EntityElectricBuffData buff;
    public float radius;
    public float interval;
    public int atkCount;
    public float pernetrateRadius;

    protected override List<int> SerializeToIntArray2()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in damageValue);
        ISerializeToIntArray.SerializeToInt(ref list, in attPercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, damageType);
        ISerializeToIntArray.SerializeToInt(ref list, attId);
        ISerializeToIntArray.SerializeToInt(ref list, in buff);
        ISerializeToIntArray.SerializeToInt(ref list, in radius);
        ISerializeToIntArray.SerializeToInt(ref list, in interval);
        ISerializeToIntArray.SerializeToInt(ref list, in atkCount);
        ISerializeToIntArray.SerializeToInt(ref list, in pernetrateRadius);
        return list;
    }
    protected override void DeserializeFromIntArray2(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attPercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attId);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out buff, userData);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out radius);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out interval);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkCount);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out pernetrateRadius);
    }
    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);
        {
            var damageTypeName = LanguageUtility.GetLanguage_DamageTypeName(damageType);
            var attName = LanguageUtility.GetLanguage_AttiributeName(attId);
            var buffName = LanguageUtility.GetLanguage_BuffStyleName(buff.GetBuff());

            var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
            descInfos.Add(info);
            info.title = LanguageUtility.GetLanguage_BallName(_BallId);
            info.level = level;
            info.content = string.Format(LanguageUtility.GetLanguage_BallDescription(_BallId),
                radius, interval, atkCount, damageValue, attPercentage01 * 100, attName, damageTypeName, buffName);
        }
        if (pernetrateRadius > 0)
        {
            var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
            descInfos.Add(info);
            info.title = LanguageUtility.GetLanguage_10010();
            info.level = level;
            info.content = LanguageUtility.GetLanguage(1210103);

            var info3 = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
            descInfos.Add(info3);
            info3.title = LanguageUtility.GetLanguage(1210103);
            info3.level = level + 1;
            info3.content = string.Format(LanguageUtility.GetLanguage(1210104), pernetrateRadius);
        }

        buff.GetDescriptionList(ref descInfos, level + 1);
    }
}

public class GameBattleSkillBall10071011Data : GameBattleSkillBallData<GameBattleSkillBall10071011EntityData, GameBattleSkillBall10071011StarData>
{

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
                {
                    new GameBattleSkillBall10071011StarData(){
                        damageValue = 20,
                        attPercentage01 = 1f,
                        damageType = EnDamageType.Magic,
                        attId = EnEntityAttributeId.Attack,

                        radius = 2f,
                        interval = 0.3f,
                        atkCount = 1,
                        pernetrateRadius = 0f,

                        buff = new()
                        {
                            duration = 3f,
                            maxCount = 5,
                            reducePercentage01 = 0.1f,
                            att1 = EnEntityAttributeId.PhysicsArmor,
                            att2 = EnEntityAttributeId.PhysicsArmor,
                        },
                    },
                    new GameBattleSkillBall10071011StarData(){
                        damageValue = 40,
                        attPercentage01 = 1.2f,
                        damageType = EnDamageType.Magic,
                        attId = EnEntityAttributeId.Attack,

                        radius = 2f,
                        interval = 0.3f,
                        atkCount = 2,
                        pernetrateRadius = 0.1f,

                        buff = new()
                        {
                            duration = 4f,
                            maxCount = 5,
                            reducePercentage01 = 0.1f,
                            att1 = EnEntityAttributeId.PhysicsArmor,
                            att2 = EnEntityAttributeId.PhysicsArmor,
                        },
                    },
                    new GameBattleSkillBall10071011StarData(){
                        damageValue = 80,
                        attPercentage01 = 1.4f,
                        damageType = EnDamageType.Magic,
                        attId = EnEntityAttributeId.Attack,

                        radius = 2f,
                        interval = 0.3f,
                        atkCount = 3,
                        pernetrateRadius = 0.2f,

                        buff = new()
                        {
                            duration = 5f,
                            maxCount = 5,
                            reducePercentage01 = 0.1f,
                            att1 = EnEntityAttributeId.PhysicsArmor,
                            att2 = EnEntityAttributeId.PhysicsArmor,
                        },

                        partInfos = new()
                        {
                            new()
                            {
                                partId = 10071011,
                                partStar = 1,
                            },
                        },
                    },
                };
    }
}
