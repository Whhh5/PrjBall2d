using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBallPart1008StarData : GameBattleSkillBallPartStarData
{
    public float repelDistance;
    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out repelDistance);
    }

    public override List<int> SerializeToIntArray()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in repelDistance);
        return list;
    }

    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);

        var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
        info.level = level;
        info.title = LanguageUtility.GetLanguage_BallPartStyleName(_PartId);
        info.content = string.Format(LanguageUtility.GetLanguage_BallPartDescription(_PartId), repelDistance);
        descInfos.Add(info);
    }
}
public class GameBattleSkillBallPart1008Data : GameBattleSkillBallPartData<GameBattleSkillBallPart1008EntityData, GameBattleSkillBallPart1008StarData>
{
    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new()
        {
            new GameBattleSkillBallPart1008StarData()
            {
                repelDistance = 0.5f,
            },
            new GameBattleSkillBallPart1008StarData()
            {
                repelDistance = 0.75f,
            },
            new GameBattleSkillBallPart1008StarData()
            {
                repelDistance = 1f,
            },
        };
    }
}
