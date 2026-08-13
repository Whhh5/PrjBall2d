using System.Collections.Generic;

public class GameBattleSkillBall1011StarData : GameBattleSkillBallStarData
{
    public int damageValue;
    public float attPercentage01;
    public EnDamageType damageType;
    public EnEntityAttributeId attId;

    public int effectRowCount;
    public int effectColCount;
    public float effectAtkPercentage01;
    public int effectAtkDamage;
    public EnDamageType effectAtkDamageType;
    public int effectAtkCount;
    public float effectInterval;


    protected override List<int> SerializeToIntArray2()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in damageValue);
        ISerializeToIntArray.SerializeToInt(ref list, in attPercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, damageType);
        ISerializeToIntArray.SerializeToInt(ref list, attId);

        ISerializeToIntArray.SerializeToInt(ref list, in effectRowCount);
        ISerializeToIntArray.SerializeToInt(ref list, in effectColCount);
        ISerializeToIntArray.SerializeToInt(ref list, in effectAtkPercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, in effectAtkDamage);
        ISerializeToIntArray.SerializeToInt(ref list, in effectAtkCount);
        ISerializeToIntArray.SerializeToInt(ref list, effectAtkDamageType);
        ISerializeToIntArray.SerializeToInt(ref list, in effectInterval);
        return list;
    }
    protected override void DeserializeFromIntArray2(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attPercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attId);

        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectRowCount);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectColCount);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectAtkPercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectAtkDamage);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectAtkCount);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectAtkDamageType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectInterval);
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
            damageValue, attPercentage01 * 100, attName, damageTypeName, effectRowCount, effectColCount, effectAtkCount, effectInterval, effectAtkDamage, effectAtkPercentage01 * 100, effectDamageTypeName);

    }
}
public class GameBattleSkillBall1011Data : GameBattleSkillBallData<GameBattleSkillBall1011EntityData, GameBattleSkillBall1011StarData>
{

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
                {
                    new GameBattleSkillBall1011StarData(){
                        damageValue = 20,
                        attPercentage01 = 1f,
                        damageType = EnDamageType.Physic,
                        attId = EnEntityAttributeId.Attack,

                        effectRowCount = 3,
                        effectColCount = 3,
                        effectAtkPercentage01 = 0.1f,
                        effectAtkDamage = 20,
                        effectAtkDamageType = EnDamageType.Magic,
                        effectAtkCount = 3,
                        effectInterval = 0.1f,
                    },
                    new GameBattleSkillBall1011StarData(){
                        damageValue = 40,
                        attPercentage01 = 1.2f,
                        damageType = EnDamageType.Physic,
                        attId = EnEntityAttributeId.Attack,

                        effectRowCount = 3,
                        effectColCount = 3,
                        effectAtkPercentage01 = 0.2f,
                        effectAtkDamage = 30,
                        effectAtkDamageType = EnDamageType.Magic,
                        effectAtkCount = 4,
                        effectInterval = 0.1f,
                    },
                    new GameBattleSkillBall1011StarData(){
                        damageValue = 80,
                        attPercentage01 = 1.4f,
                        damageType = EnDamageType.Physic,
                        attId = EnEntityAttributeId.Attack,

                        effectRowCount = 5,
                        effectColCount = 5,
                        effectAtkPercentage01 = 0.3f,
                        effectAtkDamage = 40,
                        effectAtkDamageType = EnDamageType.Magic,
                        effectAtkCount = 5,
                        effectInterval = 0.1f,

                        partInfos = new()
                        {
                            new GameBattleSkillBallPartInfo()
                            {
                                partId = 1011,
                                partStar = 1,
                            }
                        },
                    },
                };
    }
}
