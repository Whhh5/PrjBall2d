using System.Collections.Generic;

public class GameBattleSkillBallPart10011008StarData : GameBattleSkillBallPartStarData
{
    public int extraSwordCount;
    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out extraSwordCount);
    }

    public override List<int> SerializeToIntArray()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in extraSwordCount);
        return list;
    }
    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);

        var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
        descInfos.Add(info);
        info.title = LanguageUtility.GetLanguage_BallPartStyleName(_PartId);
        info.level = level;
        info.content = string.Format(LanguageUtility.GetLanguage_BallPartDescription(_PartId), extraSwordCount);

    }
}

public class GameBattleSkillBallPart10011008Data : GameBattleSkillBallPartData<GameBattleSkillBallPart10011008EntityData, GameBattleSkillBallPart10011008StarData>
{

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
        {
            new GameBattleSkillBallPart10011008StarData(){
                extraSwordCount = 1,
            },
            new GameBattleSkillBallPart10011008StarData(){
                extraSwordCount = 2,
            },
            new GameBattleSkillBallPart10011008StarData(){
                extraSwordCount = 3,
            },
        };
    }
}
