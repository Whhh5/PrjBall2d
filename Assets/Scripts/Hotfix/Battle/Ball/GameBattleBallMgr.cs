using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public enum EnBallId
{
    FenLie = 1002,
}
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
public class GameBattleSkillBallDestroyEventData : IClassPoolDestroy
{
    public int unvalidEntityId;
    public Vector2 pos;
    public Vector2 moveDir;
    public void OnPoolDestroy()
    {
        unvalidEntityId = -1;
        pos
            = moveDir
            = Vector2.zero;
    }
}
public class GameBattleSkillBallRuntimeData : IClassPoolDestroy
{
    private int _Star;
    private readonly List<int> _EntityIds = new();

    public void OnPoolDestroy()
    {
        _EntityIds.Clear();
        _Star = 0;
    }
    public int GetStar()
    {
        return _Star;
    }

    public void SetStar(in int star)
    {
        _Star = star;
    }

    public int GetEntityCount()
    {
        return _EntityIds.Count;
    }
    public int GetEntity(int index)
    {
        return _EntityIds[index];
    }
    public void AddEntityId(in int entityId)
    {
        _EntityIds.Add(entityId);
    }
    public void RemoveEntityId(in int entityId)
    {
        _EntityIds.Remove(entityId);
    }
}

public class GameBattleBallMgr : Singleton<GameBattleBallMgr>, IGameMapLifecycle
{
    public override EnManagerFuncType FuncType => base.FuncType | EnManagerFuncType.Update;

    private readonly Dictionary<int, GameBattleBallMoveData> _BallMoveDatas = new();
    private readonly Dictionary<int, GameBattleSkillBallData> _BallDatas = new();
    private readonly Dictionary<int, GameBattleSkillBallRuntimeData> _BallRuntimeData = new();
    private float _LastFireTime = 0;

    private readonly List<GameBallFireData> _DelayFireBalls = new();

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

    public EnGameMapLifecycleOrder GetMapLifecycleOrder()
    {
        return EnGameMapLifecycleOrder.GameBattleBallMgr;
    }
    public void EnterMap(int mapId)
    {
        InitBallConfig(mapId);
    }
    private void InitBallConfig(int mapId)
    {
        if (true)
        {
            var testBallId = 1012;
            SetBallStar(testBallId, 3);
            AddDelayFireBall(testBallId, float.MinValue);
            AddDelayFireBall(testBallId, float.MinValue);
            AddDelayFireBall(testBallId, float.MinValue);
            AddDelayFireBall(testBallId, float.MinValue);
            AddDelayFireBall(testBallId, float.MinValue);
            AddDelayFireBall(testBallId, float.MinValue);
            AddDelayFireBall(testBallId, float.MinValue);
            return;
        }
        var mapCfg = GameSchedule.Instance.GetMapCfg0(mapId);
        for (int i = 0; i < mapCfg.arrDefaultBalls.Length; i++)
        {
            var ballId = mapCfg.arrDefaultBalls[i];
            SetBallStar(ballId, 1);

            AddDelayFireBall(ballId, float.MinValue);
        }

    }

    public void ExitMap(int mapId)
    {
        ClearBalls();
    }

    public void ReenterMap(int mapId)
    {
        ClearBalls();
        InitBallConfig(mapId);
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
            ABBEventMgr.Instance.FireExecute(EnABBEvent.EVENT_SKILL_BALL_ADD, ballId, 0, eventUserData);
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
            ABBEventMgr.Instance.FireExecute(EnABBEvent.EVENT_SKILL_BALL_UPSTAR, ballId, 0, eventUserData);
            ClassPoolMgr.Instance.Push(eventUserData);
        }

    }
    public void RemoveBall(int ballId)
    {
        if (!_BallRuntimeData.TryGetValue(ballId, out var rtData))
            return;
        var entityCount = rtData.GetEntityCount();
        for (int i = 0; i < entityCount; i++)
        {
            var entityId = rtData.GetEntity(i);
            var moveData = _BallMoveDatas[entityId];
            ClassPoolMgr.Instance.Push(moveData);
            _BallMoveDatas.Remove(entityId);
        }

        RemoveDelayFireBall(ballId);
        var eventUserData = ClassPoolMgr.Instance.Pull<GameSkillBallRemoveEventData>();
        eventUserData.ballId = ballId;
        ABBEventMgr.Instance.FireExecute(EnABBEvent.EVENT_SKILL_BALL_REMOVE, ballId, 0, eventUserData);
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
    public int GetBallStar(int ballId)
    {
        if (!_BallRuntimeData.TryGetValue(ballId, out var rtData))
            return 0;
        var star = rtData.GetStar();
        return star;
    }

    public override bool IsUpdate()
    {
        return base.IsUpdate() && GameMapMgr.Instance.IsValid();
    }
    public override void Update()
    {
        base.Update();


        if (GameBattleInputMgr.Instance.IsValid())
        {
            var playerEntityId = GamePlayerMgr.Instance.GetPlayerEntityId();
            var curTime = GameUtility.GetGameTime();
            var interval = GamePlayerMgr.Instance.GetBallFireInterval(playerEntityId);
            if (interval + _LastFireTime < curTime)
            {
                if (TryGetNextFireBallId(out var ballId))
                {
                    _LastFireTime = curTime;
                    var firePoint = GameBattleInputMgr.Instance.GetFirePos();
                    var dir = GameBattleInputMgr.Instance.GetFireDirection();

                    CreateBallEntity(playerEntityId, ballId, firePoint, dir);
                    var cfg = GameSchedule.Instance.GetSkillBallCfg0(ballId);
                    GameEffectMgr.Instance.PlayEffectOnce(cfg.nFireEffectId, firePoint);
                }
            }
        }
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
        var proCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(entityId, EnSceneEntityComType.Attribute);
        proCom.SetBaseValue(EnEntityAttributeId.MoveSpeed, ballSpeed);

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

            var eventUserData = ClassPoolMgr.Instance.Pull<GameSkillBallRemoveEventData>();
            eventUserData.ballId = ballId;
            ABBEventMgr.Instance.FireExecute(EnABBEvent.EVENT_SKILL_BALL_REMOVE, ballId, 0, eventUserData);
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
    private bool TryGetNextFireBallId(out int ballId)
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

    public T GetBallStarData<T>(int ballId)
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
}
