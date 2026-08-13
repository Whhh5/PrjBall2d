using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBallPart1007StarData : GameBattleSkillBallPartStarData
{
    public EnEntityAttributeId attId;
    public float speedPercentage01;
    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out speedPercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out attId);
    }

    public override List<int> SerializeToIntArray()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in speedPercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, attId);
        return list;
    }



    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);

        var attName = LanguageUtility.GetLanguage_AttiributeName(attId);

        var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
        info.level = level;
        info.title = LanguageUtility.GetLanguage_BallPartStyleName(_PartId);
        info.content = string.Format(LanguageUtility.GetLanguage_BallPartDescription(_PartId),
            speedPercentage01 * 100, attName);
        descInfos.Add(info);
    }
}
public class GameBattleSkillBallPart1007Data : GameBattleSkillBallPartData<GameBattleSkillBallPart1007EntityData, GameBattleSkillBallPart1007StarData>
{
    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new()
        {
            new GameBattleSkillBallPart1007StarData()
            {
                speedPercentage01 = 0.1f,
                attId = EnEntityAttributeId.MoveSpeed,
            },
            new GameBattleSkillBallPart1007StarData()
            {
                speedPercentage01 = 0.15f,
                attId = EnEntityAttributeId.MoveSpeed,
            },
            new GameBattleSkillBallPart1007StarData()
            {
                speedPercentage01 = 0.2f,
                attId = EnEntityAttributeId.MoveSpeed,
            },
        };
    }
}
