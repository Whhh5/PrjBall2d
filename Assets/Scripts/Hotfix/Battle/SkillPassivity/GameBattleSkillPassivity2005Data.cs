
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class GameBattleSkillPassivity2005StarData : GameBattleSkillPassivityStarData
{
    public float addAttValue;
    public EnEntityAttributeId addAttId;

    public override List<int> SerializeToIntArray()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in addAttValue);
        ISerializeToIntArray.SerializeToInt(ref list, addAttId);
        return list;
    }
    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out addAttValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out addAttId);
    }
    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);

        var attName = LanguageUtility.GetLanguage_AttiributeName(addAttId);

        var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
        descInfos.Add(info);
        info.title = LanguageUtility.GetLanguage_SkillPassivityName(_SkillPassivityId);
        info.level = level;
        info.content = string.Format(LanguageUtility.GetLanguage_SkillPassivityDescription(_SkillPassivityId),
            addAttValue * 100, attName);

    }
}
public class GameBattleSkillPassivity2005Data : GameBattleSkillPassivityData<GameBattleSkillPassivity2005StarData>
{
    public override void OnDisable()
    {
        EventMgr.Instance.Unregister<GameSkillBallReflectEventData>(EnABBEvent.EVENT_BATTLE_BALL_REFLECT, 0, 0, OnEvent_BallReflect);
    }

    public override void OnEnable(int star)
    {
        EventMgr.Instance.Register<GameSkillBallReflectEventData>(EnABBEvent.EVENT_BATTLE_BALL_REFLECT, 0, 0, OnEvent_BallReflect);
    }

    public override void OnUpstar(int lastStar, int curStar)
    {
    }

    private void OnEvent_BallReflect(in int ballEntityId, in int _, in GameSkillBallReflectEventData data)
    {
        var starData = GetStarUserData();
        GameAttributeUtility.AddAddtionalPercentage(data.ballEntityId, starData.addAttId, starData.addAttValue);
    }

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new()
        {
            new GameBattleSkillPassivity2005StarData()
            {
                addAttValue = 0.05f,
                addAttId = EnEntityAttributeId.MoveSpeed,
            },
            new GameBattleSkillPassivity2005StarData()
            {
                addAttValue =  0.07f,
                addAttId = EnEntityAttributeId.MoveSpeed,
            },
            new GameBattleSkillPassivity2005StarData()
            {
                addAttValue = 0.1f,
                addAttId = EnEntityAttributeId.MoveSpeed,
            },
        };
    }
}