using System.Collections.Generic;

public class GameBattleSkillBall1001StarData : GameBattleSkillBallStarData
{
    public int damageValue;
    public float attPercentage01;
    public EnDamageType damageType;
    public EnEntityAttributeId attId;

    public EntityVulnerableBuffData buffData;

    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        ClassPoolMgr.Instance.Push(buffData);
        attPercentage01
            = damageValue
            = -1;
        damageType
            = EnDamageType.None;
        attId = EnEntityAttributeId.None;
        buffData = null;
    }

    protected override List<int> SerializeToIntArray2()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in damageValue);
        ISerializeToIntArray.SerializeToInt(ref list, in attPercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, damageType);
        ISerializeToIntArray.SerializeToInt(ref list, attId);

        ISerializeToIntArray.SerializeToInt(ref list, in buffData);
        return list;
    }
    protected override void DeserializeFromIntArray2(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attPercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attId);

        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out buffData, userData);
    }

    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);
        var damageTypeStr = LanguageUtility.GetLanguage_DamageTypeName(damageType);

        var nameStr = LanguageUtility.GetLanguage_BallName(_BallId);
        var descStr = LanguageUtility.GetLanguage_BallDescription(_BallId);
        var bffName = LanguageUtility.GetLanguage_BuffStyleName(buffData.GetBuff());
        var attName = LanguageUtility.GetLanguage_AttiributeName(attId);

        var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
        info.level = level;
        info.title = nameStr;
        info.content = string.Format(descStr, damageValue, attPercentage01 * 100, attName, damageTypeStr, bffName);
        descInfos.Add(info);

        buffData.GetDescriptionList(ref descInfos, level + 1);
    }
}
public class GameBattleSkillBall1001Data : GameBattleSkillBallData<GameBattleSkillBall1001EntityData, GameBattleSkillBall1001StarData>
{

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
                {
                    new GameBattleSkillBall1001StarData(){
                        damageValue = 10,
                        damageType = EnDamageType.Physic,
                        attPercentage01 = 0.1f,
                        attId = EnEntityAttributeId.Attack,

                        buffData = new EntityVulnerableBuffData()
                        {
                            duration = 3,
                            damageType = EnDamageType.Physic,
                            donusDamagePercentage = 0.05f,
                            donusDamageType = EnDamageType.Magic,
                            maxCount = 3,
                        },
                    },
                    new GameBattleSkillBall1001StarData(){
                        damageValue = 20,
                        damageType = EnDamageType.Physic,
                        attPercentage01 = 0.2f,
                        attId = EnEntityAttributeId.Attack,

                        buffData = new EntityVulnerableBuffData()
                        {
                            duration = 4,
                            damageType = EnDamageType.Physic,
                            donusDamagePercentage = 0.10f,
                            donusDamageType = EnDamageType.Magic,
                            maxCount = 3,
                        },
                    },
                    new GameBattleSkillBall1001StarData(){
                        damageValue = 30,
                        damageType = EnDamageType.Physic,
                        attPercentage01 = 0.3f,
                        attId = EnEntityAttributeId.Attack,

                        buffData = new EntityVulnerableBuffData()
                        {
                            duration = 5,
                            damageType = EnDamageType.Physic,
                            donusDamagePercentage = 0.20f,
                            donusDamageType = EnDamageType.Magic,
                            maxCount = 3,
                        },

                        partInfos = new()
                        {
                            new GameBattleSkillBallPartInfo()
                            {
                                partId = 1001,
                                partStar = 1,
                            }
                        },
                    },
                };
    }
}
