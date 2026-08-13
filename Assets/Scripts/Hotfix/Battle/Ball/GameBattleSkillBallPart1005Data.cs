using System.Collections.Generic;

public class GameBattleSkillBallPart1005StarData : GameBattleSkillBallPartStarData
{
    public EntityShatteredArmorBuffData buffData;
    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out buffData, in userData);
    }

    public override List<int> SerializeToIntArray()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in buffData);
        return list;
    }

    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);

        var buffName = LanguageUtility.GetLanguage_BuffStyleName(buffData.GetBuff());
        var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
        descInfos.Add(info);

        info.level = level;
        info.title = LanguageUtility.GetLanguage_BallPartStyleName(_PartId);
        info.content = string.Format(LanguageUtility.GetLanguage_BallPartDescription(_PartId), buffName);

        buffData.GetDescriptionList(ref descInfos, level + 1);
    }
}
public class GameBattleSkillBallPart1005Data : GameBattleSkillBallPartData<GameBattleSkillBallPart1005EntityData, GameBattleSkillBallPart1005StarData>
{
    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new()
        {
            new GameBattleSkillBallPart1005StarData()
            {
                buffData = new EntityShatteredArmorBuffData()
                {
                    duration = 3,
                    maxCount= 3,
                    shatteredArmorPercentage01 = 0.1f,
                    attId = EnEntityAttributeId.PhysicsArmor,
                },
            },
            new GameBattleSkillBallPart1005StarData()
            {
                buffData = new EntityShatteredArmorBuffData()
                {
                    duration = 5,
                    maxCount= 3,
                    shatteredArmorPercentage01 = 0.2f,
                    attId = EnEntityAttributeId.PhysicsArmor,
                },
            },
            new GameBattleSkillBallPart1005StarData()
            {
                buffData = new EntityShatteredArmorBuffData()
                {
                    duration = 8,
                    maxCount= 3,
                    shatteredArmorPercentage01 = 0.3f,
                    attId = EnEntityAttributeId.PhysicsArmor,
                },
            },
        };
    }
}
