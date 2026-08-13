using System.Collections.Generic;

public class GameBattleSkillBall1007StarData : GameBattleSkillBallStarData
{
    public int damageValue;
    public float attPercentage01;
    public EnDamageType damageType;
    public EnEntityAttributeId attId;

    protected override List<int> SerializeToIntArray2()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in damageValue);
        ISerializeToIntArray.SerializeToInt(ref list, in attPercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, damageType);
        ISerializeToIntArray.SerializeToInt(ref list, attId);
        return list;
    }
    protected override void DeserializeFromIntArray2(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attPercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attId);
    }
    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);
        var damageTypeName = LanguageUtility.GetLanguage_DamageTypeName(damageType);
        var attName = LanguageUtility.GetLanguage_AttiributeName(attId);

        var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
        descInfos.Add(info);
        info.title = LanguageUtility.GetLanguage_BallName(_BallId);
        info.level = level;
        info.content = string.Format(LanguageUtility.GetLanguage_BallDescription(_BallId),
            damageValue, attPercentage01 * 100, attName, damageTypeName);

    }
}
public class GameBattleSkillBall1007Data : GameBattleSkillBallData<GameBattleSkillBall1007EntityData, GameBattleSkillBall1007StarData>
{

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
                {
                    new GameBattleSkillBall1007StarData(){
                        damageValue = 20,
                        attPercentage01 = 1f,
                        damageType = EnDamageType.Physic,
                        attId = EnEntityAttributeId.Attack,
                    },
                    new GameBattleSkillBall1007StarData(){
                        damageValue = 40,
                        attPercentage01 = 1.2f,
                        damageType = EnDamageType.Physic,
                        attId = EnEntityAttributeId.Attack,
                    },
                    new GameBattleSkillBall1007StarData(){
                        damageValue = 80,
                        attPercentage01 = 1.4f,
                        damageType = EnDamageType.Physic,
                        attId = EnEntityAttributeId.Attack,

                        partInfos = new()
                        {
                            new()
                            {
                                partId = 1007,
                                partStar = 1,
                            },
                        },
                    },
                };
    }
}
