using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameBattleBallMoveData : IClassPoolDestroy
{
    public Vector2 moveDir;
    public void OnPoolDestroy()
    {
        moveDir = Vector2.zero;
    }
}
public class GameBallFireData : IClassPoolDestroy
{
    public int ballId;
    public float nextFireTime;

    public void OnPoolDestroy()
    {
        nextFireTime
            = ballId
            = -1;
    }
}
public class GameBattleBallMgr : Singleton<GameBattleBallMgr>, IGameMapLifecycle
{
    private readonly Dictionary<int, GameBattleBallMoveData> _BallMoveDatas = new();
    private readonly Dictionary<int, GameBattleSkillBallData> _BallDatas = new();
    private readonly Dictionary<int, GameBattleSkillBallRuntimeData> _BallRuntimeData = new();

    private readonly List<GameBallFireData> _DelayFireBalls = new();

    private readonly Dictionary<int, HashSet<int>> _BallId2FussionId = new();
    private readonly Dictionary<int, List<int>> _FussionIdBallId2SourceBallIds = new();

    public override void Destroy()
    {
        GameMapMgr.Instance.UnregisterLifecycle(this);
        base.Destroy();
    }
    public override async UniTask AwakeAsync()
    {
        await base.AwakeAsync();

        GameMapMgr.Instance.RegisterLifecycle(this);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        _BallId2FussionId.Clear();
        _FussionIdBallId2SourceBallIds.Clear();
    }
    public override async UniTask OnEnableAsync()
    {
        await base.OnEnableAsync();

        var fussionCfgCount = GameSchedule.Instance.GetSkillBallFussionCfgCount();
        for (int i = 0; i < fussionCfgCount; i++)
        {
            var cfg = GameSchedule.Instance.GetSkillBallFussionCfg(i);
            _FussionIdBallId2SourceBallIds.Add(cfg.nFinalBallId, new(cfg.arrSourceBallIds));
            for (int j = 0; j < cfg.arrSourceBallIds.Length; j++)
            {
                var ballId = cfg.arrSourceBallIds[j];
                if (!_BallId2FussionId.TryGetValue(ballId, out var list))
                {
                    list = new();
                    _BallId2FussionId.Add(ballId, list);
                }
                list.Add(cfg.nFussionId);
            }
        }
    }

    public EnGameMapLifecycleOrder GetMapLifecycleOrder()
    {
        return EnGameMapLifecycleOrder.GameBattleBallMgr;
    }
    public void EnterMap(int mapId, int chapterId)
    {
        InitBallConfig(mapId, chapterId);
    }
    private void InitBallConfig(int mapId, int chapterId)
    {
        var chapterCfg = GameSchedule.Instance.GetChapterCfg0(chapterId);
        for (int i = 0; i < chapterCfg.arrDefaultBalls.Length; i++)
        {
            var ballId = chapterCfg.arrDefaultBalls[i];
            SetBallStar(ballId, 1);

            AddDelayFireBall(ballId, float.MinValue);
        }
    }

    public void ExitMap()
    {
        ClearBalls();
    }

    public void ReenterMap()
    {
        var curMapId = GameMapMgr.Instance.GetCurMapId();
        var curChapterId = GameMapMgr.Instance.GetCurChapterId();
        ClearBalls();
        InitBallConfig(curMapId, curChapterId);
    }

    private GameBattleSkillBallData GetBallData(in int ballId)
    {
        if (!_BallDatas.TryGetValue(ballId, out var ballData))
        {
            var cfg = GameSchedule.Instance.GetSkillBallCfg0(ballId);
            var userData = ClassPoolMgr.Instance.Pull<GameBattleSkillBallUserData>();
            userData.ballId = ballId;
            ballData = TypeDefine.CreateInstance<GameBattleSkillBallData>(in cfg.nClassTypeId, userData);
            ClassPoolMgr.Instance.Push(userData);
            _BallDatas.Add(ballId, ballData);
        }
        return ballData;
    }

    public void AddBall(int ballId)
    {
        AddBall(ballId, 1);
    }
    public void AddBall(int ballId, int star)
    {
        var cfg = GameSchedule.Instance.GetSkillBallCfg0(ballId);
        if (!_BallRuntimeData.TryGetValue(ballId, out var rtData))
        {
            if (star > cfg.nMaxStar)
                return;
            rtData = ClassPoolMgr.Instance.Pull<GameBattleSkillBallRuntimeData>();
            _BallRuntimeData.Add(ballId, rtData);
            rtData.SetStar(star);

            AddDelayFireBall(ballId, float.MinValue);

            var eventUserData = ClassPoolMgr.Instance.Pull<GameSkillBallAddEventData>();
            eventUserData.ballId = ballId;
            eventUserData.star = star;
            EventMgr.Instance.FireExecute(EnABBEvent.EVENT_SKILL_BALL_ADD, cfg.nBallType, ballId, eventUserData);
            ClassPoolMgr.Instance.Push(eventUserData);
        }
        else
        {
            var curStar = rtData.GetStar();
            var toStar = curStar + star;
            if (toStar > cfg.nMaxStar)
                return;
            rtData.SetStar(toStar);

            var eventUserData = ClassPoolMgr.Instance.Pull<GameSkillBallUpStarEventData>();
            eventUserData.ballId = ballId;
            eventUserData.star = toStar;
            eventUserData.addStar = star;
            EventMgr.Instance.FireExecute(EnABBEvent.EVENT_SKILL_BALL_UPSTAR, cfg.nBallType, ballId, eventUserData);
            ClassPoolMgr.Instance.Push(eventUserData);
        }

    }
    public void RemoveBall(int ballId)
    {
        if (!_BallRuntimeData.TryGetValue(ballId, out var rtData))
            return;
        var ballCfg = GameSchedule.Instance.GetSkillBallCfg0(ballId);
        var ballData = GetBallData(in ballId);
        var entityCount = rtData.GetEntityCount();
        for (int i = 0; i < entityCount; i++)
        {
            var entityId = rtData.GetEntity(i);
            var moveData = _BallMoveDatas[entityId];
            ClassPoolMgr.Instance.Push(moveData);
            _BallMoveDatas.Remove(entityId);

            ballData.DestroyBallEntity(entityId);
        }

        RemoveDelayFireBall(ballId);
        var eventUserData = ClassPoolMgr.Instance.Pull<GameSkillBallRemoveEventData>();
        eventUserData.ballId = ballId;
        eventUserData.star = rtData.GetStar();
        EventMgr.Instance.FireExecute(EnABBEvent.EVENT_SKILL_BALL_REMOVE, ballCfg.nBallType, ballId, eventUserData);
        ClassPoolMgr.Instance.Push(eventUserData);
        _BallRuntimeData.Remove(ballId);
        ClassPoolMgr.Instance.Push(rtData);
    }
    public void SetBallStar(int ballId, int star)
    {
        var curStar = GetBallStar(ballId);
        if (curStar == star)
            return;
        AddBall(ballId, star - curStar);
    }
    public int GetBallStar(EnBallId ballId)
    {
        var star = GetBallStar((int)ballId);
        return star;
    }
    public int GetBallStar(in int ballId)
    {
        if (!_BallRuntimeData.TryGetValue(ballId, out var rtData))
            return 0;
        var star = rtData.GetStar();
        return star;
    }

    public int CreateBallEntity(int ownerEntityId, int ballId, in Vector2 pos, in Vector2 dir)
    {
        var userData = ClassPoolMgr.Instance.Pull<GameBattleSkillBallEntityUserData>();
        var entityId = CreateBallEntity(in ownerEntityId, in ballId, in pos, in dir, userData);
        ClassPoolMgr.Instance.Push(userData);
        return entityId;
    }
    public int CreateBallEntity(in int ownerEntityId, in int ballId, in Vector2 pos, in Vector2 dir, GameBattleSkillBallEntityUserData userData)
    {
        var ballData = GetBallData(in ballId);

        var entityId = ballData.CreateBallEntity(in ownerEntityId, in ballId, in pos, userData);

        var ballSpeed = GamePlayerMgr.Instance.GetBallSpeed(ownerEntityId);
        GameAttributeUtility.SetBaseValue(entityId, EnEntityAttributeId.MoveSpeed, ballSpeed);

        var data = ClassPoolMgr.Instance.Pull<GameBattleBallMoveData>();
        data.moveDir = dir;
        _BallMoveDatas.Add(entityId, data);

        if (!_BallRuntimeData.TryGetValue(ballId, out var rtData))
        {
            rtData = ClassPoolMgr.Instance.Pull<GameBattleSkillBallRuntimeData>();
            _BallRuntimeData.Add(ballId, rtData);
        }
        rtData.AddEntityId(entityId);

        return entityId;
    }
    public void DestroyBallEntity(int entityId)
    {
        var entityData = EntityMgr.Instance.GetEntityData<GameBattleSkillBallEntityData>(entityId);

        var moveData = _BallMoveDatas[entityId];
        var ballId = entityData.GetSkillBallId();
        var pos = entityData.GetWorldPos();
        var dir = moveData.moveDir;
        ClassPoolMgr.Instance.Push(moveData);
        _BallMoveDatas.Remove(entityId);

        var ballData = GetBallData(in ballId);
        ballData.DestroyBallEntity(entityId);

        var rtData = _BallRuntimeData[ballId];
        rtData.RemoveEntityId(entityId);

        var cfg = GameSchedule.Instance.GetSkillBallCfg0(ballId);
        if (cfg.bIsRecycle > 0)
        {
            AddDelayFireBall(ballId);
        }

        var evUd = ClassPoolMgr.Instance.Pull<GameBattleSkillBallDestroyEventData>();
        evUd.unvalidEntityId = entityId;
        evUd.pos = pos;
        evUd.moveDir = dir;
        EventMgr.Instance.FireExecute(EnABBEvent.EVENT_BATTLE_BALL_DESTROY, ballId, 0, evUd);
        ClassPoolMgr.Instance.Push(evUd);
    }
    public int GetBallPos(in int ballId, ref Vector2[] pos)
    {
        if (!_BallRuntimeData.TryGetValue(ballId, out var value))
            return 0;
        var entityCount = value.GetEntityCount();
        for (int i = 0; i < entityCount; i++)
        {
            if (pos == null || pos.Length == 0 || i >= pos.Length)
            {
                var tempArr = pos;
                pos = new Vector2[pos.Length * 2];
                Array.Copy(tempArr, pos, tempArr.Length);
            }
            var entityId = value.GetEntity(i);
            var worldPos = EntityMgr.Instance.GetEntityWorldPos2D(entityId);
            pos[i] = worldPos;
        }
        return entityCount;
    }

    public void GetBallEntityIds(ref List<int> entityIds)
    {
        entityIds.AddRange(_BallMoveDatas.Keys);
    }

    public int GetBallCountByType(EnBallType ballType)
    {
        var count = 0;
        foreach (var (ballId, _) in _BallRuntimeData)
        {
            var ballCfg = GameSchedule.Instance.GetSkillBallCfg0(ballId);
            if (ballCfg.nBallType != (int)ballType)
                continue;
            count++;
        }
        return count;
    }
    public List<int> GetBallIdsByType(EnBallType ballType)
    {
        var list = new List<int>(_BallRuntimeData.Count);
        foreach (var (ballId, _) in _BallRuntimeData)
        {
            var ballCfg = GameSchedule.Instance.GetSkillBallCfg0(ballId);
            if (ballCfg.nBallType != (int)ballType)
                continue;
            list.Add(ballId);
        }
        return list;
    }
    public GameBattleBallMoveData GetBallMoveData(int entityId)
    {
        var data = _BallMoveDatas[entityId];
        return data;
    }
    public void OnCollision(int ballEntityId, int monsterEntityId, Vector2 hitPoint)
    {
        var ballEntityData = EntityMgr.Instance.GetEntityData<GameBattleSkillBallEntityData>(ballEntityId);
        var ballId = ballEntityData.GetSkillBallId();
        var ballCfg = GameSchedule.Instance.GetSkillBallCfg0(ballId);
        if (ballCfg.nCollisionEffectId > 0)
        {
            GameEffectMgr.Instance.PlayEffectOnce(ballCfg.nCollisionEffectId, hitPoint);
        }
    }

    private void ClearBalls()
    {
        foreach (var (_, data) in _BallMoveDatas)
        {
            ClassPoolMgr.Instance.Push(data);
        }
        foreach (var data in _DelayFireBalls)
        {
            ClassPoolMgr.Instance.Push(data);
        }
        foreach (var (ballId, rtData) in _BallRuntimeData)
        {
            var ballData = GetBallData(in ballId);
            var entityCount = rtData.GetEntityCount();
            for (int i = 0; i < entityCount; i++)
            {
                var entityId = rtData.GetEntity(i);
                ballData.DestroyBallEntity(in entityId);
            }
            var ballCfg = GameSchedule.Instance.GetSkillBallCfg0(ballId);
            var eventUserData = ClassPoolMgr.Instance.Pull<GameSkillBallRemoveEventData>();
            eventUserData.ballId = ballId;
            eventUserData.star = rtData.GetStar();
            EventMgr.Instance.FireExecute(EnABBEvent.EVENT_SKILL_BALL_REMOVE, ballCfg.nBallType, ballId, eventUserData);
            ClassPoolMgr.Instance.Push(eventUserData);
            ClassPoolMgr.Instance.Push(rtData);
        }
        _BallMoveDatas.Clear();
        _DelayFireBalls.Clear();
        _BallRuntimeData.Clear();
    }
    private void AddDelayFireBall(int ballId)
    {
        var cfg = GameSchedule.Instance.GetSkillBallCfg0(ballId);
        var nextFireTime = GameUtility.GetGameTime() + cfg.fDelayFireTime;
        AddDelayFireBall(ballId, nextFireTime);
    }
    private void AddDelayFireBall(int ballId, float fireTime)
    {
        var data = ClassPoolMgr.Instance.Pull<GameBallFireData>();
        data.ballId = ballId;
        data.nextFireTime = fireTime;
        _DelayFireBalls.Add(data);
    }
    private void RemoveDelayFireBall(in int ballId)
    {
        for (int i = 0; i < _DelayFireBalls.Count; i++)
        {
            var data = _DelayFireBalls[i];
            if (data.ballId != ballId)
                continue;
            ClassPoolMgr.Instance.Push(data);
            _DelayFireBalls.RemoveAt(i--);
        }
    }
    public bool TryGetNextFireBallId(out int ballId)
    {
        var curTime = GameUtility.GetGameTime();
        for (int i = 0; i < _DelayFireBalls.Count; i++)
        {
            var data = _DelayFireBalls[i];
            if (curTime < data.nextFireTime)
                continue;
            ballId = data.ballId;
            ClassPoolMgr.Instance.Push(data);
            _DelayFireBalls.RemoveAt(i);
            return true;
        }
        ballId = -1;
        return false;
    }

    public T GetBallStarData<T>(in int ballId)
    {
        var star = GameBattleBallMgr.Instance.GetBallStar(ballId);
        var value = GetBallStarData<T>(ballId, star);
        return value;
    }
    public T GetBallStarData<T>(int ballId, int star)
    {
        var ballData = GetBallData(in ballId);
        var data = ballData.GetStarData(star);
        if (data is not T value)
            return default;
        return value;
    }
    public GameBattleSkillBallStarData GetBallStarData(in int ballId)
    {
        var star = GameBattleBallMgr.Instance.GetBallStar(ballId);
        var data = GetBallStarData(in ballId, in star);
        return data;
    }
    public GameBattleSkillBallStarData GetBallStarData(in int ballId, in int star)
    {
        var ballData = GetBallData(in ballId);
        var data = ballData.GetStarData(star);
        return data;
    }
    public T CreateStarData<T>(int ballId, int starLevel, ISerializeToIntArrayUserData userData)
        where T : GameBattleSkillBallStarData, new()
    {
        var ballCfg = GameSchedule.Instance.GetSkillBallCfg0(ballId);
        var startIndex = 0;
        for (int i = 0; i < starLevel - 1; i++)
        {
            var itemCount = ballCfg.arrStarParams[startIndex++];
            startIndex += itemCount;
        }
        var count = ballCfg.arrStarParams[startIndex++];
        userData.arrParams = ballCfg.arrStarParams;
        userData.startIndex = startIndex;
        userData.count = count;
        var starUserData = ClassPoolMgr.Instance.Pull<T>(userData);
        return starUserData;
    }


    #region 融合相关
    public List<int> GetSourceIds(int ballId)
    {
        if (!_FussionIdBallId2SourceBallIds.TryGetValue(ballId, out var sourceIds))
            return new() { ballId };
        return sourceIds;
    }
    public void Fussion(int fussionCfgId)
    {
        var fussionCfg = GameSchedule.Instance.GetSkillBallFussionCfg0(fussionCfgId);

        for (int i = 0; i < fussionCfg.arrSourceBallIds.Length; i++)
        {
            var ballId = fussionCfg.arrSourceBallIds[i];
            GameBattleBallMgr.Instance.RemoveBall(ballId);
        }
        GameBattleBallMgr.Instance.AddBall(fussionCfg.nFinalBallId, fussionCfg.nFinalBallStar);
    }
    public bool IsFussion(params int[] ballIds)
    {
        var ballId = ballIds[0]; // 1007
        if (!_BallId2FussionId.TryGetValue(ballId, out var fussionIds)) // 4, 5
            return false;
        foreach (var fussionId in fussionIds)
        {
            var fussionCfg = GameSchedule.Instance.GetSkillBallFussionCfg0(fussionId);
            var index = -1;
            for (int k = 1; k < ballIds.Length; k++)
            {
                index = Array.FindIndex(fussionCfg.arrSourceBallIds, id => id == ballIds[k]);
                if (index < 0)
                    break;
            }
            if (index >= 0)
                return true;
        }
        return false;
    }
    public bool TryFussion(EnGameBallFussionCondition condition, ref List<int> fussionInfos, params int[] ballIds)
    {
        var ballHash = new HashSet<int>(ballIds.Length);
        foreach (var item in ballIds)
            ballHash.Add(item);

        var testedFussionIds = new HashSet<int>(10);
        var count = ballIds.Length;
        for (int i = 0; i < count; i++)
        {
            var ballId = ballIds[i];
            if (!_BallId2FussionId.TryGetValue(ballId, out var fussionIds))
                continue;
            foreach (var fussionId in fussionIds)
            {
                if (testedFussionIds.Contains(fussionId))
                    continue;
                testedFussionIds.Add(fussionId);
                var fussionCfg = GameSchedule.Instance.GetSkillBallFussionCfg0(fussionId);
                for (int k = 0; k < fussionCfg.arrSourceBallIds.Length; k++)
                {
                    var testBallId = fussionCfg.arrSourceBallIds[k];
                    if (!ballHash.Contains(testBallId))
                        goto next;
                    if ((condition & EnGameBallFussionCondition.Star) != EnGameBallFussionCondition.None)
                    {
                        var ballStar = GetBallStar(testBallId);
                        var ballCfg = GameSchedule.Instance.GetSkillBallCfg0(testBallId);
                        if (ballStar < ballCfg.nMaxStar)
                            goto next;
                    }
                }
                fussionInfos.Add(fussionId);
            next:
                continue;
            }
        }
        return fussionInfos.Count > 0;
    }

    #endregion
}
