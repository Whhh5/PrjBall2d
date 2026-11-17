
using System.Collections.Generic;
using UnityEngine;

public class GameBattleUplevelSkillBallPartItemSelect : IBattleEnergyUplevelItemSelect
{
    private List<int> _SkillBallPartPool = new();

    private readonly HashSet<int> _IgnoreHistory = new();
    public void Destroy()
    {
        _SkillBallPartPool.Clear();
        _IgnoreHistory.Clear();
    }

    public void Init(int chapterId)
    {
    }

    public void Reset()
    {
        _IgnoreHistory.Clear();
        _SkillBallPartPool.Clear();

        if (GameBattleBallPartsMgr.Instance.TryGetCurrentCanUplevelPartIds(ref _SkillBallPartPool))
        {

        }
    }

    public int GetTestValue()
    {
        return _SkillBallPartPool.FindAll(id=>!_IgnoreHistory.Contains(id)).Count * 2;
    }

    public bool IsValid()
    {
        return _IgnoreHistory.Count < _SkillBallPartPool.Count;
    }

    public bool TrySelect(out IUIBattleUplevelWindow_ItemUserData userData, bool isIgnoreHistory, HashSet<int> extraIgnoree)
    {
        userData = default;
        var newBallPool = extraIgnoree?.Count > 0
            ? _SkillBallPartPool.FindAll(ballId => !extraIgnoree.Contains(ballId))
            : _SkillBallPartPool;
        newBallPool = isIgnoreHistory
            ? newBallPool.FindAll(ballId => !_IgnoreHistory.Contains(ballId))
            : newBallPool;
        if (newBallPool.Count == 0)
            return false;
        var selectIndex = Random.Range(0, newBallPool.Count);
        var partId = newBallPool[selectIndex];
        _IgnoreHistory.Add(partId);

        var star = GameBattleBallPartsMgr.Instance.GetPartStar(partId);
        var data = ClassPoolMgr.Instance.Pull<UIBattleUplevelWindow_SkillBallPartItemUserData>();
        data.partId = partId;
        data.star = star + 1;
        userData = data;
        return true;
    }
}