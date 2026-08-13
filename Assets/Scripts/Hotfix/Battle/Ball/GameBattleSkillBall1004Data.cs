using System.Collections.Generic;

public class GameBattleSkillBall1004StarData : GameBattleSkillBallStarData
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
        var damageTypeStr = LanguageUtility.GetLanguage_DamageTypeName(damageType);

        var nameStr = LanguageUtility.GetLanguage_BallName(_BallId);
        var descStr = LanguageUtility.GetLanguage_BallDescription(_BallId);
        var attName = LanguageUtility.GetLanguage_AttiributeName(attId);

        var ballCfg = GameSchedule.Instance.GetSkillBallCfg0(_BallId);

        var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
        info.level = level;
        info.title = nameStr;
        info.content = string.Format(descStr, damageValue, attPercentage01 * 100, attName, damageTypeStr, ballCfg.fDelayFireTime);
        descInfos.Add(info);
    }

}
public class GameBattleSkillBall1004Data : GameBattleSkillBallData<GameBattleSkillBall1004EntityData, GameBattleSkillBall1004StarData>
{

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
                {
                    new GameBattleSkillBall1004StarData(){
                        damageValue = 20,
                        attPercentage01 = 1,
                        damageType = EnDamageType.Physic,
                        attId = EnEntityAttributeId.Attack,
                    },
                    new GameBattleSkillBall1004StarData(){
                        damageValue = 40,
                        attPercentage01 = 1.2f,
                        damageType = EnDamageType.Physic,
                        attId = EnEntityAttributeId.Attack,
                    },
                    new GameBattleSkillBall1004StarData(){
                        damageValue = 80,
                        attPercentage01 = 1.4f,
                        damageType = EnDamageType.Physic,
                        attId = EnEntityAttributeId.Attack,

                        partInfos = new List<GameBattleSkillBallPartInfo>()
                        {
                            new GameBattleSkillBallPartInfo()
                            {
                                partId = 1004,
                                partStar = 1,
                            }
                        },
                    },
                };
    }
}
