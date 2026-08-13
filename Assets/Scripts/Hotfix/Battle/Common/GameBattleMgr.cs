using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public interface IBattleSystem
{
    public void OnEnable();
    public void OnDisable();
    public void OnUpdate();
    public void OnReset();
}
public enum EnGameBattleStatus
{
    None,
    Idle,
    Battle,
    Finish,
    Faild,
    Success,
}
public class GameBattleMgr : Singleton<GameBattleMgr>, IGameMapLifecycle
{
    public override EnManagerFuncType FuncType => base.FuncType | EnManagerFuncType.Update;
    private readonly Dictionary<string, IBattleSystem> _BattleSystems = new();

    private int _BossEntityId;
    private EnGameBattleStatus _BattleStatus = EnGameBattleStatus.None;
    public override async UniTask OnEnableAsync()
    {
        await base.OnEnableAsync();

        GameMapMgr.Instance.RegisterLifecycle(this);

        var types = typeof(GameBattleMgr).Assembly.GetTypes();
        foreach (var type in types)
        {
            if (type.IsInterface)
                continue;
            if (!typeof(IBattleSystem).IsAssignableFrom(type))
                continue;
            var ins = (IBattleSystem)System.Activator.CreateInstance(type);
            _BattleSystems.Add(type.FullName, ins);
        }
    }
    public T GetSystem<T>()
        where T : IBattleSystem
    {
        var key = typeof(T).FullName;
        if (!_BattleSystems.TryGetValue(key, out var system))
            return default;
        return (T)system;
    }
    public override void OnDisable()
    {
        GameMapMgr.Instance.UnregisterLifecycle(this);
        base.OnDisable();
    }

    public EnGameMapLifecycleOrder GetMapLifecycleOrder() => EnGameMapLifecycleOrder.GameBattleMgr;

    public void EnterMap(int mapId)
    {
        EventMgr.Instance.Register<GameSkillBossDebutEventData>(EnABBEvent.EVENT_SKILL_EVENT_SCHEDULE, 0, (int)EnSkillEventScheduleId.Debut, OnEvent_BossDebut);
        foreach (var item in _BattleSystems)
        {
            item.Value.OnEnable();
        }
        SetBattleStatus(EnGameBattleStatus.Battle);
    }

    public void ExitMap(int mapId)
    {
        EventMgr.Instance.Unregister<GameSkillBossDebutEventData>(EnABBEvent.EVENT_SKILL_EVENT_SCHEDULE, 0, (int)EnSkillEventScheduleId.Debut, OnEvent_BossDebut);
        foreach (var item in _BattleSystems)
        {
            item.Value.OnDisable();
        }
        if (_BossEntityId > 0)
        {
            EventMgr.Instance.Register<GameBattleMonsterDieEventData>(EnABBEvent.EVENT_BATTLE_MONSTER_DIE, _BossEntityId, 0, OnEvent_BossDie);
        }
        _BossEntityId = -1;
        SetBattleStatus(EnGameBattleStatus.Finish);
    }

    public void ReenterMap(int mapId)
    {
        foreach (var item in _BattleSystems)
        {
            item.Value.OnReset();
        }
        if (_BossEntityId > 0)
        {
            EventMgr.Instance.Register<GameBattleMonsterDieEventData>(EnABBEvent.EVENT_BATTLE_MONSTER_DIE, _BossEntityId, 0, OnEvent_BossDie);
        }
        _BossEntityId = -1;
        SetBattleStatus(EnGameBattleStatus.Battle);
    }
    public override bool IsUpdate()
    {
        return base.IsUpdate() && GameMapMgr.Instance.IsValid();
    }
    public override void Update()
    {
        base.Update();

        if (IsBattleStatus(EnGameBattleStatus.Battle))
        {
            foreach (var item in _BattleSystems)
            {
                item.Value.OnUpdate();
            }

            var waveSys = GetSystem<GameBattleWaveSystem>();
            var waveIsFinish = waveSys.IsFinish();
            var monsterCount = GameMonsterMgr.Instance.GetMonsterCount();
            if (waveIsFinish && monsterCount == 0)
            {
                SetBattleStatus(EnGameBattleStatus.Success);
            }
        }
    }
    public bool IsBattleStatus(EnGameBattleStatus gameStatus)
    {
        return _BattleStatus == gameStatus;
    }
    public void SetBattleStatus(EnGameBattleStatus gameStatus)
    {
        if (gameStatus == _BattleStatus)
            return;
        _BattleStatus = gameStatus; 

        EventMgr.Instance.FireExecute(EnABBEvent.EVENT_BATTLE_STATUS_CHANGLE, (int)gameStatus);
    }

    public int GetBossEntityId()
    {
        return _BossEntityId;
    }
    public void SetBossEntityId(int bossEntityId)
    {
        _BossEntityId = bossEntityId;
        EventMgr.Instance.Register<GameBattleMonsterDieEventData>(EnABBEvent.EVENT_BATTLE_MONSTER_DIE, bossEntityId, 0, OnEvent_BossDie);
    }

    private void OnEvent_BossDebut(in int mapBossEntityId, in int __, in GameSkillBossDebutEventData data)
    {
        var mapId = GameMapMgr.Instance.GetCurMapId();
        var mapCfg = GameSchedule.Instance.GetMapCfg0(mapId);
        var waveSystem = GameBattleMgr.Instance.GetSystem<GameBattleWaveSystem>();
        waveSystem.CreateWaveMonsterEntitys(mapCfg.nBossWaveId);
    }
    private void OnEvent_BossDie(in int entityId, in int _, in GameBattleMonsterDieEventData data)
    {
        var monsterIds = GameMonsterMgr.Instance.GetMonsterList();
        foreach (var monsterEntityId in monsterIds)
        {
            if (!GameMonsterUtility.EntityIsValid(monsterEntityId))
                continue;
            GameMonsterMgr.Instance.MonsterDie(monsterEntityId);
        }

        var eventUserData = ClassPoolMgr.Instance.Pull<GameBattleBossDieEventData>();
        eventUserData.entityId = entityId;
        EventMgr.Instance.FireExecute(EnABBEvent.EVENT_BATTLE_BOSS_DIE, 0, 0, eventUserData);
        ClassPoolMgr.Instance.Push(eventUserData);

    }
}
