using System.Collections.Generic;

public class GameBattleSkillBall1006StarData : GameBattleSkillBallStarData
{
    public int damageValue;
    public float attPercentage01;
    public EnDamageType damageType;
    public EnEntityAttributeId attId;

    public EntityCrystalBuffData buffUserData;

    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        ClassPoolMgr.Instance.Push(buffUserData);
        buffUserData = null;
        attPercentage01
            = damageValue
            = -1;
    }
    protected override List<int> SerializeToIntArray2()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in damageValue);
        ISerializeToIntArray.SerializeToInt(ref list, in attPercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, damageType);
        ISerializeToIntArray.SerializeToInt(ref list, attId);

        ISerializeToIntArray.SerializeToInt(ref list, in buffUserData);
        return list;
    }
    protected override void DeserializeFromIntArray2(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attPercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attId);

        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out buffUserData, in userData);
    }

    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);

        var buffName = LanguageUtility.GetLanguage_BuffStyleName(buffUserData.GetBuff());
        var damageTypeName = LanguageUtility.GetLanguage_DamageTypeName(damageType);
        var attName = LanguageUtility.GetLanguage_AttiributeName(attId);

        var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
        descInfos.Add(info);
        info.title = LanguageUtility.GetLanguage_BallName(_BallId);
        info.level = level;
        info.content = string.Format(LanguageUtility.GetLanguage_BallDescription(_BallId),
            damageValue, attPercentage01 * 100, attName, damageTypeName, buffName);

        buffUserData.GetDescriptionList(ref descInfos, level + 1);
    }
}
public class GameBattleSkillBall1006Data : GameBattleSkillBallData<GameBattleSkillBall1006EntityData, GameBattleSkillBall1006StarData>
{

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
                {
                    new GameBattleSkillBall1006StarData(){
                        damageValue = 20,
                        attPercentage01 = 1.1f,
                        damageType = EnDamageType.Physic,
                        attId = EnEntityAttributeId.Attack,

                        buffUserData = new EntityCrystalBuffData()
                        {
                            duration = 3,
                            speedReducePercentage01 = 0.1f,
                            maxCount = 3,
                            attId = EnEntityAttributeId.MoveSpeed,
                        },
                    },
                    new GameBattleSkillBall1006StarData(){
                        damageValue = 40,
                        attPercentage01 = 1.2f,
                        damageType = EnDamageType.Physic,
                        attId = EnEntityAttributeId.Attack,

                        buffUserData = new EntityCrystalBuffData()
                        {
                            duration = 4,
                            speedReducePercentage01 = 0.2f,
                            maxCount = 3,
                            attId = EnEntityAttributeId.MoveSpeed,
                        },
                    },
                    new GameBattleSkillBall1006StarData(){
                        damageValue = 80,
                        attPercentage01 = 1.3f,
                        damageType = EnDamageType.Physic,
                        attId = EnEntityAttributeId.Attack,

                        buffUserData = new EntityCrystalBuffData()
                        {
                            duration = 5,
                            speedReducePercentage01 = 0.3f,
                            maxCount = 3,
                            attId = EnEntityAttributeId.MoveSpeed,
                        },

                        partInfos = new()
                        {
                            new()
                            {
                                partId = 1006,
                                partStar = 1,
                            }
                        }
                    },
                };
    }
}
