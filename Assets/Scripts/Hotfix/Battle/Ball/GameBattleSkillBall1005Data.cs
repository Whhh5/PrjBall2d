using System.Collections.Generic;

public class GameBattleSkillBall1005StarData : GameBattleSkillBallStarData
{
    public int damageValue;
    public float attPercentage01;
    public EnDamageType damageType;
    public EnEntityAttributeId attId;

    public int effectRowCount;
    public int effectColCount;
    public int effectAtkDamageValue;
    public float effectAtkPercentage01;
    public EnDamageType effectAtkDamageType;


    protected override List<int> SerializeToIntArray2()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in damageValue);
        ISerializeToIntArray.SerializeToInt(ref list, in attPercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, damageType);
        ISerializeToIntArray.SerializeToInt(ref list, attId);

        ISerializeToIntArray.SerializeToInt(ref list, in effectRowCount);
        ISerializeToIntArray.SerializeToInt(ref list, in effectColCount);
        ISerializeToIntArray.SerializeToInt(ref list, in effectAtkDamageValue);
        ISerializeToIntArray.SerializeToInt(ref list, in effectAtkPercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, effectAtkDamageType);
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
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectAtkDamageValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectAtkPercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectAtkDamageType);
    }
    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);

        var attName = LanguageUtility.GetLanguage_AttiributeName(attId);
        var damageTypeName = LanguageUtility.GetLanguage_DamageTypeName(damageType);
        var effectDamageTypeName = LanguageUtility.GetLanguage_DamageTypeName(effectAtkDamageType);

        var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
        descInfos.Add(info);

        info.level = level;
        info.title = LanguageUtility.GetLanguage_BallName(_BallId);
        info.content = string.Format(LanguageUtility.GetLanguage_BallDescription(_BallId),
            damageValue, attPercentage01 * 100, attName, damageTypeName, effectRowCount, effectColCount, effectAtkDamageValue, effectAtkPercentage01 * 100, effectDamageTypeName);

    }
}
public class GameBattleSkillBall1005Data : GameBattleSkillBallData<GameBattleSkillBall1005EntityData, GameBattleSkillBall1005StarData>
{

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
                {
                    new GameBattleSkillBall1005StarData(){
                        damageValue = 20,
                        attPercentage01 = 1f,
                        damageType = EnDamageType.Physic,
                        attId = EnEntityAttributeId.Attack,

                        effectRowCount = 3,
                        effectColCount = 3,
                        effectAtkDamageValue = 10,
                        effectAtkPercentage01 = 0.5f,
                        effectAtkDamageType = EnDamageType.Magic,
                    },
                    new GameBattleSkillBall1005StarData(){
                        damageValue = 40,
                        attPercentage01 = 1.2f,
                        damageType = EnDamageType.Physic,
                        attId = EnEntityAttributeId.Attack,

                        effectRowCount = 3,
                        effectColCount = 3,
                        effectAtkDamageValue = 20,
                        effectAtkPercentage01 = 0.75f,
                        effectAtkDamageType = EnDamageType.Magic,
                    },
                    new GameBattleSkillBall1005StarData(){
                        damageValue = 80,
                        attPercentage01 = 1.4f,
                        damageType = EnDamageType.Physic,
                        attId = EnEntityAttributeId.Attack,

                        effectRowCount = 3,
                        effectColCount = 3,
                        effectAtkDamageValue = 30,
                        effectAtkPercentage01 = 1f,
                        effectAtkDamageType = EnDamageType.Magic,

                        partInfos = new()
                        {
                            new()
                            {
                                partId = 1005,
                                partStar = 1,
                            },
                        }
                    },
                };
    }
}
