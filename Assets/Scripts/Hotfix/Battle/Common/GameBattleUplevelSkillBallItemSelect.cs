
using System.Collections.Generic;
using UnityEngine;

public class GameBattleUplevelSkillBallItemSelect : IBattleEnergyUplevelItemSelect
{
    private readonly List<int> _SkillBallPool = new();

    private readonly HashSet<int> _IgnoreHistory = new();
    private int _MaxCount = 0;
    public void Destroy()
    {
        _SkillBallPool.Clear();
        _IgnoreHistory.Clear();
        _MaxCount = 0;
    }

    public void Init(int chapterId)
    {
    }

    public int GetTestValue()
    {
        var curBallIds = GameBattleBallMgr.Instance.GetBallIdsByType(EnBallType.Skill);
        var newBallPool = _SkillBallPool.FindAll(testBallId => !_IgnoreHistory.Contains(testBallId));

        var maxBallCount = GlobalCfgUtility.Get40001_Int32();
        if (curBallIds.Count >= maxBallCount)
        {
            newBallPool = newBallPool.FindAll(testBallId => curBallIds.Contains(testBallId));
        }
        return Mathf.Min(newBallPool.Count, maxBallCount) * 3;
    }

    public void Reset()
    {
        _IgnoreHistory.Clear();
        _SkillBallPool.Clear();
        var chapterId = GameMapMgr.Instance.GetCurChapterId();
        var chapterCfg = GameSchedule.Instance.GetChapterCfg0(chapterId);
        _SkillBallPool.AddRange(chapterCfg.arrSkillBallIds);
        var curSkillList = GameBattleBallMgr.Instance.GetBallIdsByType(EnBallType.Skill);
        for (int i = 0; i < curSkillList.Count; i++)
        {
            var ballId = curSkillList[i];
            var ballStar = GameBattleBallMgr.Instance.GetBallStar(ballId);
            var ballCfg = GameSchedule.Instance.GetSkillBallCfg0(ballId);
            if (ballStar == ballCfg.nMaxStar)
                _IgnoreHistory.Add(ballId);
            if (!_SkillBallPool.Contains(ballId))
            {
                _SkillBallPool.Add(ballId);
            }
        }
        // 移除融合技能球
        var curBallIds = GameBattleBallMgr.Instance.GetBallIdsByType(EnBallType.Skill);
        for (int i = 0; i < curBallIds.Count; i++)
        {
            var ballId = curBallIds[i];
            var ballCfg = GameSchedule.Instance.GetSkillBallCfg0(ballId);
            if (ballCfg.nBallUseType != (int)EnBallUseType.Fussion)
                continue;
            var sourceBallIds = GameBattleBallMgr.Instance.GetSourceIds(ballId);
            _SkillBallPool.RemoveAll(id => sourceBallIds.Contains(id));
        }

        _MaxCount = _IgnoreHistory.Count;
    }
    public bool IsValid()
    {
        var maxBallCount = GlobalCfgUtility.Get40001_Int32();
        return _MaxCount < maxBallCount && _IgnoreHistory.Count < _SkillBallPool.Count;
    }

    public bool TrySelect(out IUIBattleUplevelWindow_ItemUserData userData, bool isIgnoreHistory, HashSet<int> extraIgnoree)
    {
        userData = default;
        var newBallPool = extraIgnoree?.Count > 0
            ? _SkillBallPool.FindAll(ballId => !extraIgnoree.Contains(ballId))
            : _SkillBallPool;
        newBallPool = isIgnoreHistory
            ? newBallPool.FindAll(ballId => !_IgnoreHistory.Contains(ballId))
            : newBallPool;
        var curBallIds = GameBattleBallMgr.Instance.GetBallIdsByType(EnBallType.Skill);
        var maxBallCount = GlobalCfgUtility.Get40001_Int32();
        if (curBallIds.Count >= maxBallCount)
        {
            newBallPool = newBallPool.FindAll(testBallId => curBallIds.Contains(testBallId));
        }
        if (newBallPool.Count == 0)
            return false;
        var selectIndex = Random.Range(0, newBallPool.Count);
        var ballId = newBallPool[selectIndex];
        _IgnoreHistory.Add(ballId);

        var star = GameBattleBallMgr.Instance.GetBallStar(ballId);
        var data = ClassPoolMgr.Instance.Pull<UIBattleUplevelWindow_SkillBallItemUserData>();
        data.ballId = ballId;
        data.star = star + 1;
        userData = data;

        var ballCfg = GameSchedule.Instance.GetSkillBallCfg0(ballId);
        if (data.star == ballCfg.nMaxStar)
        {
            _MaxCount++;
        }
        return true;
    }
}