using System.Collections.Generic;

public class GameBattleSkillBallPart1011StarData : GameBattleSkillBallPartStarData
{
    public EntityElectricBuffData buffData;
    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out buffData, in userData);
    }

    public override List<int> SerializeToIntArray()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, buffData);
        return list;
    }
    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);

        var buffName = LanguageUtility.GetLanguage_BuffStyleName(buffData.GetBuff());

        var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
        info.level = level;
        info.title = LanguageUtility.GetLanguage_BallPartStyleName(_PartId);
        info.content = string.Format(LanguageUtility.GetLanguage_BallPartDescription(_PartId), buffName);
        descInfos.Add(info);

        buffData.GetDescriptionList(ref descInfos, level + 1);
    }
}
public class GameBattleSkillBallPart1011Data : GameBattleSkillBallPartData<GameBattleSkillBallPart1011EntityData, GameBattleSkillBallPart1011StarData>
{

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new()
        {
            new GameBattleSkillBallPart1011StarData()
            {
                buffData = new EntityElectricBuffData()
                {
                    duration = 3,
                    maxCount = 3,
                    reducePercentage01 = 0.05f,
                    att1 = EnEntityAttributeId.PhysicsArmor,
                    att2 = EnEntityAttributeId.MagicArmor,
                },
            },
            new GameBattleSkillBallPart1011StarData()
            {
                buffData = new EntityElectricBuffData()
                {
                    duration = 4,
                    maxCount = 4,
                    reducePercentage01 = 0.05f,
                    att1 = EnEntityAttributeId.PhysicsArmor,
                    att2 = EnEntityAttributeId.MagicArmor,
                },
            },
            new GameBattleSkillBallPart1011StarData()
            {
                buffData = new EntityElectricBuffData()
                {
                    duration = 5,
                    maxCount = 5,
                    reducePercentage01 = 0.05f,
                    att1 = EnEntityAttributeId.PhysicsArmor,
                    att2 = EnEntityAttributeId.MagicArmor,
                },
            },
        };
    }
}
