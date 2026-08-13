using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class GameBattleSkillBallPart10071011StarData : GameBattleSkillBallPartStarData
{
    public float delayTime;
    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out delayTime);
    }

    public override List<int> SerializeToIntArray()
    {
        var list = new List<int>();

        ISerializeToIntArray.SerializeToInt(ref list, in delayTime);
        return list;
    }

    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);

        var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
        info.level = level;
        info.title = LanguageUtility.GetLanguage_BallPartStyleName(_PartId);
        info.content = string.Format(LanguageUtility.GetLanguage_BallPartDescription(_PartId), delayTime);
        descInfos.Add(info);
    }
}
public class GameBattleSkillBallPart10071011Data : GameBattleSkillBallPartData<GameBattleSkillBallPart10071011EntityData, GameBattleSkillBallPart10071011StarData>
{
    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new()
        {
            new GameBattleSkillBallPart10071011StarData()
            {
                delayTime = 0.5f,
            },
            new GameBattleSkillBallPart10071011StarData()
            {
                delayTime = 0.5f,
            },
            new GameBattleSkillBallPart10071011StarData()
            {
                delayTime = 0.5f,
            },
        };
    }
}
