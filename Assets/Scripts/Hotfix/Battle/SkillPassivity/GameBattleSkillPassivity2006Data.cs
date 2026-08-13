
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class GameBattleSkillPassivity2006StarData : GameBattleSkillPassivityStarData
{
    public int killCount;
    public float addAttValue;
    public EnEntityAttributeId addAttId;

    public override List<int> SerializeToIntArray()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in killCount);
        ISerializeToIntArray.SerializeToInt(ref list, in addAttValue);
        ISerializeToIntArray.SerializeToInt(ref list, addAttId);
        return list;
    }
    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out killCount);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out addAttValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out addAttId);
    }
    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);

        var attName = LanguageUtility.GetLanguage_AttiributeName(addAttId);
        var attValue = LanguageUtility.GetLanguage_AttributeValue(addAttId, addAttValue);

        var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
        descInfos.Add(info);
        info.title = LanguageUtility.GetLanguage_SkillPassivityName(_SkillPassivityId);
        info.level = level;
        info.content = string.Format(LanguageUtility.GetLanguage_SkillPassivityDescription(_SkillPassivityId),
            killCount, attValue, attName);

    }
}
public class GameBattleSkillPassivity2006Data : GameBattleSkillPassivityData<GameBattleSkillPassivity2006StarData>
{
    private int _DieCount = 0;
    private int _RoundCount;
    public override void OnDisable()
    {
        EventMgr.Instance.Unregister<GameBattleMonsterDieEventData>(EnABBEvent.EVENT_BATTLE_MONSTER_DIE, 0, 0, OnEvent_MonsterDie);
        _DieCount = 0;
        _RoundCount = 0;
    }

    public override void OnEnable(int star)
    {
        EventMgr.Instance.Register<GameBattleMonsterDieEventData>(EnABBEvent.EVENT_BATTLE_MONSTER_DIE, 0, 0, OnEvent_MonsterDie);
    }

    public override void OnUpstar(int lastStar, int curStar)
    {
        var lastStarData = GetStarUserData(lastStar);
        var starData = GetStarUserData(curStar);
        var changeAttValue = starData.addAttValue - lastStarData.addAttValue;

        var curRound = Mathf.FloorToInt(_DieCount / starData.killCount);
        var changeRound = curRound - _RoundCount;

        var finalChangeAttValue = changeRound * starData.addAttValue + changeAttValue * _RoundCount;

        GameAttributeUtility.AddAddtionalValue(_OwnerEntityId, starData.addAttId, finalChangeAttValue);
        _RoundCount = curRound;
    }

    private void OnEvent_MonsterDie(in int entityId, in int _, in GameBattleMonsterDieEventData data)
    {
        _DieCount++;
        var starData = GetStarUserData();
        var round = Mathf.FloorToInt(_DieCount / starData.killCount);
        if (round != _RoundCount)
        {
            var addRound = round - _RoundCount;
            var finalChangeAttValue = addRound * starData.addAttValue;

            GameAttributeUtility.AddAddtionalValue(in _OwnerEntityId, in starData.addAttId, in finalChangeAttValue);
            _RoundCount = round;
        }
    }
    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);
        var starData = GetStarUserData();
        var attName = LanguageUtility.GetLanguage_AttiributeName(starData.addAttId);
        var valueStr = LanguageUtility.GetLanguage_AttributeValue(starData.addAttId, _RoundCount * starData.addAttValue);

        var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
        descInfos.Add(info);
        info.title = LanguageUtility.GetLanguage_10013();
        info.level = level;
        info.content = string.Format(LanguageUtility.GetLanguage_SkillPassivityTipsDescription(_SkillPassivityId),
            _DieCount, valueStr, attName);
    }

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new()
        {
            new GameBattleSkillPassivity2006StarData()
            {
                killCount = 5,
                addAttValue = 0.5f,
                addAttId = EnEntityAttributeId.Critical,
            },
            new GameBattleSkillPassivity2006StarData()
            {
                killCount = 5,
                addAttValue =  0.75f,
                addAttId = EnEntityAttributeId.Critical,
            },
            new GameBattleSkillPassivity2006StarData()
            {
                killCount = 5,
                addAttValue = 1,
                addAttId = EnEntityAttributeId.Critical,
            },
        };
    }
}