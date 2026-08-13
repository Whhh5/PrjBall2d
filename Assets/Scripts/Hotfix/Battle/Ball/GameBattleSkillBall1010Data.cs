using System.Collections.Generic;

public class GameBattleSkillBall1010StarData : GameBattleSkillBallStarData
{
    public int damageValue;
    public float attPercentage01;
    public EnDamageType damageType;
    public EnEntityAttributeId attId;

    public float effectWidth;
    public int effectAtkDamage;
    public EnDamageType effectAtkDamageType;
    public float effectAtkPercentage01;


    protected override List<int> SerializeToIntArray2()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in damageValue);
        ISerializeToIntArray.SerializeToInt(ref list, in attPercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, damageType);
        ISerializeToIntArray.SerializeToInt(ref list, attId);

        ISerializeToIntArray.SerializeToInt(ref list, in effectWidth);
        ISerializeToIntArray.SerializeToInt(ref list, in effectAtkDamage);
        ISerializeToIntArray.SerializeToInt(ref list, effectAtkDamageType);
        ISerializeToIntArray.SerializeToInt(ref list, in effectAtkPercentage01);
        return list;
    }
    protected override void DeserializeFromIntArray2(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attPercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attId);

        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectWidth);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectAtkDamage);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectAtkDamageType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectAtkPercentage01);
    }
    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);
        var damageTypeName = LanguageUtility.GetLanguage_DamageTypeName(damageType);
        var effectDamageTypeName = LanguageUtility.GetLanguage_DamageTypeName(effectAtkDamageType);
        var attName = LanguageUtility.GetLanguage_AttiributeName(attId);

        var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
        descInfos.Add(info);
        info.title = LanguageUtility.GetLanguage_BallName(_BallId);
        info.level = level;
        info.content = string.Format(LanguageUtility.GetLanguage_BallDescription(_BallId),
            damageValue, attPercentage01 * 100, attName, damageTypeName, effectWidth, effectAtkDamage, effectAtkPercentage01 * 100, effectDamageTypeName);

    }
}
public class GameBattleSkillBall1010Data : GameBattleSkillBallData<GameBattleSkillBall1010EntityData, GameBattleSkillBall1010StarData>
{

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
                {
                    new GameBattleSkillBall1010StarData(){
                        damageValue = 100,
                        attPercentage01 = 1f,
                        damageType = EnDamageType.Physic,
                        attId = EnEntityAttributeId.Attack,

                        effectWidth = 0.4f,
                        effectAtkDamage = 10,
                        effectAtkDamageType = EnDamageType.Magic,
                        effectAtkPercentage01 = 0.3f,
                    },
                    new GameBattleSkillBall1010StarData(){
                        damageValue = 1,
                        attPercentage01 = 1.2f,
                        damageType = EnDamageType.Physic,
                        attId = EnEntityAttributeId.Attack,

                        effectWidth = 0.4f,
                        effectAtkDamage = 20,
                        effectAtkDamageType = EnDamageType.Magic,
                        effectAtkPercentage01 = 0.4f,
                    },
                    new GameBattleSkillBall1010StarData(){
                        damageValue = 80,
                        attPercentage01 = 1.4f,
                        damageType = EnDamageType.Physic,
                        attId = EnEntityAttributeId.Attack,

                        effectWidth = 1f,
                        effectAtkDamage = 30,
                        effectAtkDamageType = EnDamageType.Magic,
                        effectAtkPercentage01 = 0.5f,

                        partInfos = new()
                        {
                            new GameBattleSkillBallPartInfo()
                            {
                                partId = 1010,
                                partStar = 1,
                            }
                        },
                    },
                };
    }
}
