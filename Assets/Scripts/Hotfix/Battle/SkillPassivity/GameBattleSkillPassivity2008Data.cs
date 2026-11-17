
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class GameBattleSkillPassivity2008StarData : GameBattleSkillPassivityStarData
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

        var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
        descInfos.Add(info);
        info.title = LanguageUtility.GetLanguage_SkillPassivityName(_SkillPassivityId);
        info.level = level;
        info.content = LanguageUtility.GetLanguage_SkillPassivityDescription(_SkillPassivityId);

    }
}
public class GameBattleSkillPassivity2008Data : GameBattleSkillPassivityData<GameBattleSkillPassivity2008StarData>
{
    public override void OnDisable()
    {
        var starData = GetStarUserData();

        var ownerEntityId = GameSkillPassivityMgr.Instance.GetOwnerEntityId(_SkillPassivityId);
        GameAttributeUtility.RemoveAddtionalValue(ownerEntityId, starData.addAttId, starData.addAttValue);
    }

    public override void OnEnable(int star)
    {
        var starData = GetStarUserData();
        var ownerEntityId = GameSkillPassivityMgr.Instance.GetOwnerEntityId(_SkillPassivityId);
        GameAttributeUtility.AddAddtionalValue(ownerEntityId, starData.addAttId, starData.addAttValue);
    }

    public override void OnUpstar(int lastStar, int curStar)
    {

    }

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new()
        {
            new GameBattleSkillPassivity2008StarData()
            {
                addAttValue = 1,
                addAttId = EnEntityAttributeId.MagicCritical,
            },
        };
    }
}