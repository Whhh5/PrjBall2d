
using System.Collections.Generic;
using UnityEngine;

public class GameBattleUplevelSkillBallFussionItemSelect : IBattleEnergyUplevelItemSelect
{
    private List<int> _FussionInfos = new();

    private readonly HashSet<int> _IgnoreHistory = new();
    public void Destroy()
    {
        _IgnoreHistory.Clear();
        _FussionInfos.Clear();
    }

    public void Init(int mapId)
    {
    }

    public void Reset()
    {
        _IgnoreHistory.Clear();
        _FussionInfos.Clear();

        var curSkillList = GameBattleBallMgr.Instance.GetBallIdsByType(EnBallType.Skill);
        if (GameBattleBallMgr.Instance.TryFussion(EnGameBallFussionCondition.All, ref _FussionInfos, curSkillList.ToArray()))
        {

        }
    }
    public int GetTestValue()
    {
        return _FussionInfos.FindAll(id => !_IgnoreHistory.Contains(id)).Count;
    }
    public bool IsValid()
    {
        return _IgnoreHistory.Count < _FussionInfos.Count;
    }

    public bool TrySelect(out IUIBattleUplevelWindow_ItemUserData userData, bool isIgnoreHistory, HashSet<int> extraIgnoree)
    {
        userData = default;
        var newFussionPool = extraIgnoree?.Count > 0
            ? _FussionInfos.FindAll(ballId => !extraIgnoree.Contains(ballId))
            : _FussionInfos;
        newFussionPool = isIgnoreHistory
            ? newFussionPool.FindAll(ballId => !_IgnoreHistory.Contains(ballId))
            : newFussionPool;
        if (newFussionPool.Count == 0)
            return false;
        var selectIndex = Random.Range(0, newFussionPool.Count);
        var fussionId = newFussionPool[selectIndex];
        _IgnoreHistory.Add(fussionId);

        var star = GameBattleBallMgr.Instance.GetBallStar(fussionId);
        var data = ClassPoolMgr.Instance.Pull<UIBattleUplevelWindow_SkillBallFussionItemUserData>();
        data.fussionId = fussionId;
        userData = data;

        return true;
    }
}