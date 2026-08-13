using System.Collections.Generic;

public class GameBattleSkillBall1012StarData : GameBattleSkillBallStarData
{
    public int damageValue;
    public float attPercentage01;
    public EnDamageType damageType;
    public EnEntityAttributeId attId;

    public int effectInvokProbability0100;
    public int effectAddValue;
    public float effectRestorePercentage01;
    public EnEntityAttributeId effectAddAttId;

    protected override List<int> SerializeToIntArray2()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in damageValue);
        ISerializeToIntArray.SerializeToInt(ref list, in attPercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, damageType);
        ISerializeToIntArray.SerializeToInt(ref list, attId);

        ISerializeToIntArray.SerializeToInt(ref list, in effectInvokProbability0100);
        ISerializeToIntArray.SerializeToInt(ref list, in effectAddValue);
        ISerializeToIntArray.SerializeToInt(ref list, in effectRestorePercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, effectAddAttId);
        return list;
    }
    protected override void DeserializeFromIntArray2(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attPercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attId);

        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectInvokProbability0100);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectAddValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectRestorePercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out effectAddAttId);
    }

    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);
        var damageTypeName = LanguageUtility.GetLanguage_DamageTypeName(damageType);
        var effectAddAttName = LanguageUtility.GetLanguage_AttiributeName(effectAddAttId);
        var attName = LanguageUtility.GetLanguage_AttiributeName(attId);

        var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
        descInfos.Add(info);
        info.title = LanguageUtility.GetLanguage_BallName(_BallId);
        info.level = level;
        info.content = string.Format(LanguageUtility.GetLanguage_BallDescription(_BallId),
            damageValue, attPercentage01 * 100, attName, damageTypeName, effectInvokProbability0100, effectAddValue, effectRestorePercentage01 * 100, effectAddAttName);

    }
}
public class GameBattleSkillBall1012Data : GameBattleSkillBallData<GameBattleSkillBall1012EntityData, GameBattleSkillBall1012StarData>
{

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
                {
                    new GameBattleSkillBall1012StarData(){
                        damageValue = 20,
                        attPercentage01 = 1f,
                        damageType = EnDamageType.Physic,
                        attId = EnEntityAttributeId.Attack,

                        effectInvokProbability0100 = 2,
                        effectAddValue = 2,
                        effectRestorePercentage01 = 0.01f,
                        effectAddAttId = EnEntityAttributeId.Hp,
                    },
                    new GameBattleSkillBall1012StarData(){
                        damageValue = 40,
                        attPercentage01 = 1.2f,
                        damageType = EnDamageType.Physic,
                        attId = EnEntityAttributeId.Attack,

                        effectInvokProbability0100 = 5,
                        effectAddValue = 4,
                        effectRestorePercentage01 = 0.03f,
                        effectAddAttId = EnEntityAttributeId.Hp,
                    },
                    new GameBattleSkillBall1012StarData(){
                        damageValue = 80,
                        attPercentage01 = 1.4f,
                        damageType = EnDamageType.Physic,
                        attId = EnEntityAttributeId.Attack,

                        effectInvokProbability0100 = 10,
                        effectAddValue = 8,
                        effectRestorePercentage01 = 0.05f,
                        effectAddAttId = EnEntityAttributeId.Hp,

                        partInfos = new()
                        {
                            new GameBattleSkillBallPartInfo()
                            {
                                partId = 1012,
                                partStar = 1,
                            }
                        },
                    },
                };
    }
}
