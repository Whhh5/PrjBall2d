using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;


public class GameBattleSkillBallPart10091010StarData : GameBattleSkillBallPartStarData
{
    public int laserCount;
    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out laserCount);
    }

    public override List<int> SerializeToIntArray()
    {
        var list = new List<int>();

        ISerializeToIntArray.SerializeToInt(ref list, in laserCount);
        return list;
    }
    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);

        var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
        info.level = level;
        info.title = LanguageUtility.GetLanguage_BallPartStyleName(_PartId);
        info.content = string.Format(LanguageUtility.GetLanguage_BallPartDescription(_PartId), laserCount);
        descInfos.Add(info);
    }
}
public class GameBattleSkillBallPart10091010Data : GameBattleSkillBallPartData<GameBattleSkillBallPart10091010EntityData, GameBattleSkillBallPart10091010StarData>
{
    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new()
        {
            new GameBattleSkillBallPart10091010StarData()
            {
                laserCount = 1,
            },
            new GameBattleSkillBallPart10091010StarData()
            {
                laserCount = 2,
            },
            new GameBattleSkillBallPart10091010StarData()
            {
                laserCount = 3,
            },
        };
    }
}
