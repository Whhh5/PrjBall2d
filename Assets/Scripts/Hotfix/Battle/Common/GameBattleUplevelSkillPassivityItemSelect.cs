using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleUplevelSkillPassivityItemSelect : IBattleEnergyUplevelItemSelect
{
    private readonly List<int> _SkillPassivityPool = new();

    private readonly HashSet<int> _IgnoreHistory = new();
    private int _MaxCount = 0;
    public void Destroy()
    {
        _SkillPassivityPool.Clear();
        _IgnoreHistory.Clear();
        _MaxCount = 0;
    }

    public void Init(int mapId)
    {
    }


    public int GetTestValue()
    {
        var curSkillPassivityIds = GameSkillPassivityMgr.Instance.GetSkillPassivityIds();
        var newSkillPassivityPool = _SkillPassivityPool.FindAll(testBallId => !_IgnoreHistory.Contains(testBallId));
        if (curSkillPassivityIds.Count >= GameGlobal.SkillPassivityCount)
        {
            newSkillPassivityPool = newSkillPassivityPool.FindAll(testBallId => curSkillPassivityIds.Contains(testBallId));
        }
        return Mathf.Min(GameGlobal.SkillPassivityCount, newSkillPassivityPool.Count);
    }

    public void Reset()
    {
        _IgnoreHistory.Clear();
        _SkillPassivityPool.Clear();
        var mapId = GameMapMgr.Instance.GetCurMapId();
        var mapCfg = GameSchedule.Instance.GetMapCfg0(mapId);
        _SkillPassivityPool.AddRange(mapCfg.arrSkillPassivityIds);
        var curSkillList = GameSkillPassivityMgr.Instance.GetSkillPassivityIds();
        for (int i = 0; i < curSkillList.Count; i++)
        {
            var skillPassivityId = curSkillList[i];
            var skillPassivityStar = GameSkillPassivityMgr.Instance.GetStar(skillPassivityId);
            var skillPassivityCfg = GameSchedule.Instance.GetSkillPassivityCfg0(skillPassivityId);
            if (skillPassivityStar == skillPassivityCfg.nMaxStar)
                _IgnoreHistory.Add(skillPassivityId);
            if (!_SkillPassivityPool.Contains(skillPassivityId))
            {
                _SkillPassivityPool.Add(skillPassivityId);
            }
        }
        _MaxCount = _IgnoreHistory.Count;
    }
    public bool IsValid()
    {
        return _MaxCount < GameGlobal.SkillPassivityCount && _IgnoreHistory.Count < _SkillPassivityPool.Count;
    }

    public bool TrySelect(out IUIBattleUplevelWindow_ItemUserData userData, bool isIgnoreHistory, HashSet<int> extraIgnoree)
    {
        userData = default;
        var newSkillPassivityPool = extraIgnoree?.Count > 0
            ? _SkillPassivityPool.FindAll(skillPassivityId => !extraIgnoree.Contains(skillPassivityId))
            : _SkillPassivityPool;
        newSkillPassivityPool = isIgnoreHistory
            ? newSkillPassivityPool.FindAll(skillPassivityId => !_IgnoreHistory.Contains(skillPassivityId))
            : newSkillPassivityPool;
        var curSkillPassivityIds = GameSkillPassivityMgr.Instance.GetSkillPassivityIds();
        if (curSkillPassivityIds.Count >= GameGlobal.SkillPassivityCount)
        {
            newSkillPassivityPool = newSkillPassivityPool.FindAll(testSkillPassivityId => curSkillPassivityIds.Contains(testSkillPassivityId));
        }
        if (newSkillPassivityPool.Count == 0)
            return false;
        var selectIndex = Random.Range(0, newSkillPassivityPool.Count);
        var skillPassivityId = newSkillPassivityPool[selectIndex];
        _IgnoreHistory.Add(skillPassivityId);

        var star = GameSkillPassivityMgr.Instance.GetStar(skillPassivityId);
        var data = ClassPoolMgr.Instance.Pull<UIBattleUplevelWindow_SkillPassivityItemUserData>();
        data.skillPassivityId = skillPassivityId;
        data.star = star + 1;
        userData = data;

        var skillPassivityCfg = GameSchedule.Instance.GetSkillPassivityCfg0(skillPassivityId);
        if (data.star == skillPassivityCfg.nMaxStar)
        {
            _MaxCount++;
        }
        return true;
    }
}
