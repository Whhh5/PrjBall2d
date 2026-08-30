

using System.Collections.Generic;
using UnityEngine;

public class GameBattleUplevelAttributeItemSelect : IBattleEnergyUplevelItemSelect
{
    private readonly List<int> _AttributePool = new();

    private readonly HashSet<int> _IgnoreHistory = new();
    public void Destroy()
    {
        _AttributePool.Clear();
        _IgnoreHistory.Clear();
    }

    public void Init(int chapterId)
    {
        var chapterCfg = GameSchedule.Instance.GetChapterCfg0(chapterId);
        _AttributePool.AddRange(chapterCfg.arrAttributeIds);
    }
    public int GetTestValue()
    {
        return 1;
    }

    public void Reset()
    {
        _IgnoreHistory.Clear();
    }
    public bool IsValid()
    {
        var curBallCount = GameBattleBallMgr.Instance.GetBallCountByType(EnBallType.Skill);
        return curBallCount >= GameGlobal.SkillBallCount && _IgnoreHistory.Count < _AttributePool.Count;
    }

    public bool TrySelect(out IUIBattleUplevelWindow_ItemUserData userData, bool isIgnoreHistory, HashSet<int> extraIgnoree)
    {
        userData = default;
        var newAttPool = extraIgnoree?.Count > 0
            ? _AttributePool.FindAll(ballId => !extraIgnoree.Contains(ballId))
            : _AttributePool;
        newAttPool = isIgnoreHistory
            ? newAttPool.FindAll(ballId => !_IgnoreHistory.Contains(ballId))
            : newAttPool;
        if (newAttPool.Count == 0)
            return false;
        var selectIndex = Random.Range(0, newAttPool.Count);
        var attId = newAttPool[selectIndex];
        _IgnoreHistory.Add(attId);


        var attCfg = GameSchedule.Instance.GetAttributeCfg0(attId);
        var attValueType = (EnValueType)attCfg.nValueType;
        var data = ClassPoolMgr.Instance.Pull<UIBattleUplevelWindow_AttributeItemUserData>();
        data.attId = (EnEntityAttributeId)attId;
        data.star = Random.Range(1, 3 + 1);
        data.valueType = attValueType switch
        {
            EnValueType.Percentage => EnValueType.Percentage,
            _ => (EnValueType)Random.Range((int)EnValueType.None + 1, (int)EnValueType.Count),
        };
        data.addValue = data.valueType switch
        {
            EnValueType.Percentage => Random.Range(10 * data.star, 10 * (data.star + 1)) / 100f,
            _ => Random.Range(10 * data.star, 10 * (data.star + 1)),
        };

        userData = data;

        return true;
    }
}