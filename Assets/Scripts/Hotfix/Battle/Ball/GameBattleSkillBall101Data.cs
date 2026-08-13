using System.Collections.Generic;

public class GameBattleSkillBall101StarData : GameBattleSkillBallStarData
{
    public int damageValue;
    public float attPercentage;
    public EnDamageType damageType;
    public EnEntityAttributeId attId;

    public int maxCollisionCount;


    protected override List<int> SerializeToIntArray2()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in damageValue);
        ISerializeToIntArray.SerializeToInt(ref list, in attPercentage);
        ISerializeToIntArray.SerializeToInt(ref list, damageType);
        ISerializeToIntArray.SerializeToInt(ref list, attId);

        ISerializeToIntArray.SerializeToInt(ref list, maxCollisionCount);
        return list;
    }
    protected override void DeserializeFromIntArray2(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attPercentage);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attId);

        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out maxCollisionCount);
    }

    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);

        var damageTypeStr = LanguageUtility.GetLanguage_DamageTypeName(damageType);
        var langStr = LanguageUtility.GetLanguage_BallDescription(_BallId);
        var attName = LanguageUtility.GetLanguage_AttiributeName(attId);

        var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
        info.level = level;
        info.title = LanguageUtility.GetLanguage_BallName(_BallId);
        info.content = string.Format(langStr, damageValue, attPercentage * 100, attName, damageTypeStr, maxCollisionCount);
        descInfos.Add(info);
    }
}
public class BattleSkillBall_101UserData : GameBattleSkillBallEntityUserData
{
    public int star;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        star = -1;
    }
}
public class GameBattleSkillBall101Data : GameBattleSkillBallData<GameBattleSkillBall101EntityData, GameBattleSkillBall101StarData>
{

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
                {
                    new GameBattleSkillBall101StarData(){
                        damageValue = 5,
                        attPercentage = 0.25f,
                        damageType = EnDamageType.Physic,
                        maxCollisionCount = 1,
                        attId = EnEntityAttributeId.Attack,
                    },
                    new GameBattleSkillBall101StarData(){
                        damageValue = 10,
                        attPercentage = 0.4f,
                        damageType = EnDamageType.Physic,
                        maxCollisionCount = 2,
                        attId = EnEntityAttributeId.Attack,
                    },
                    new GameBattleSkillBall101StarData(){
                        damageValue = 20,
                        attPercentage = 0.8f,
                        damageType = EnDamageType.Physic,
                        maxCollisionCount = 3,
                        attId = EnEntityAttributeId.Attack,
                    },
                };
    }
}
