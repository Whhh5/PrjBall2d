using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;


public enum EnGameMapLifecycleOrder
{
    None,
    GameBattleMonitorMgr,
    GameBattleGridMgr,
    GamePlayerMgr,
    GameBattleBallPartsMgr,
    GameSkillPassivityMgr,

    GameMonsterMgr,
    GameBattleBallMgr,
    GameBattleMgr,
}
public interface IGameMapLifecycle
{
    public EnGameMapLifecycleOrder GetMapLifecycleOrder();
    public void EnterMap(int mapId, int chapterId);
    public void ExitMap();
    public void ReenterMap();
}
public class GameMapMgr : Singleton<GameMapMgr>
{
    public override EnSingletonOrder SingletonOrder => EnSingletonOrder.GameMapMgr;
    private readonly List<IGameMapLifecycle> _MapLifecycles = new();
    private int _CurMapId = -1;
    private int _CurChapterId = -1;
    private int _BgEntityId = -1;
    private int _FireHitEntityId = -1;
    private int _MapBossEntityId = -1;
    private int _BattleMainWindowId = -1;
    private readonly List<int> _MapBossCmdArray = new(5);
    private CancellationTokenSource _BossDebutToken = null;
    private float _MapDepth;
    public override void Destroy()
    {
        base.Destroy();
        _MapDepth
            = _CurMapId
            = _CurChapterId
            = _BgEntityId
            = _FireHitEntityId
            = _MapBossEntityId
            = _BattleMainWindowId
            = -1;
        _MapBossCmdArray.Clear();
    }
    public override void OnDisable()
    {
        if (_CurMapId > 0)
        {
            ExitMap();
        }
        base.OnDisable();
        _CurMapId = -1;
    }

    public void RegisterLifecycle(IGameMapLifecycle callback)
    {
        _MapLifecycles.Add(callback);
        _MapLifecycles.Sort((item1, item2) =>
        {
            var order1 = item1.GetMapLifecycleOrder();
            var order2 = item2.GetMapLifecycleOrder();
            if (order1 > order2)
                return 1;
            if (order1 < order2)
                return -1;
            return 0;
        });
    }
    public void UnregisterLifecycle(IGameMapLifecycle callback)
    {
        _MapLifecycles.Remove(callback);
    }

    public bool IsValid()
    {
        return _CurMapId > 0;
    }
    public int GetCurMapId()
    {
        return _CurMapId;
    }
    public int GetCurChapterId()
    {
        return _CurChapterId;
    }
    public bool EnterMap(int chapterId)
    {
        if (_CurMapId > 0)
            return false;
        var chapterCfg = GameSchedule.Instance.GetChapterCfg0(chapterId);
        var mapCfg = GameSchedule.Instance.GetMapCfg0(chapterCfg.nMapId);
        if (mapCfg == null)
            return false;
        _CurChapterId = chapterId;
        _CurMapId = chapterCfg.nMapId;
        UpdateMapCfgCache(_CurMapId);
        _MapDepth = mapCfg.nDepth;

        GamePhysicsUtility.InitParams(_CurMapId);
        CreateMap(_CurMapId);
        CreateMapBackground(_CurMapId);
        CreateMapBoss(_CurMapId, _CurChapterId);

        CameraMgr.Instance.SetCameraPos(new Vector3(mapCfg.arrCameraPos[0], mapCfg.arrCameraPos[1], mapCfg.arrCameraPos[2]));

        foreach (var item in _MapLifecycles)
            item.EnterMap(_CurMapId, chapterId);

        EventMgr.Instance.Register(EnABBEvent.EVENT_BATTLE_WAVE_FINISH, 0, 0, OnEvent_WaveFinish);
        EventMgr.Instance.Register<GameBattleNextWaveEventData>(EnABBEvent.EVENT_BATTLE_NEXT_WAVE, 0, 0, OnEvent_NextWave);
        EventMgr.Instance.Register<GameSkillBossDebutEventData>(EnABBEvent.EVENT_SKILL_EVENT_SCHEDULE, 0, (int)EnSkillEventScheduleId.Debut, OnEvent_BossDebut);

        AudioMgr.Instance.Play(mapCfg.nBgmAudioId);

        _BattleMainWindowId = UIMgr.Instance.ShowWindow<UIBattleMainWindowData>();

        _FireHitEntityId = EntityMgr.Instance.CreateEntityData<GameEffectFireLineEntityData>();
        EntityMgr.Instance.LoadEntity(_FireHitEntityId);
        return true;
    }

    public void ReenterMap()
    {
        _BossDebutToken?.Cancel();
        var reenterEventData = ClassPoolMgr.Instance.Pull<GameBattleMapReenterEventData>();
        reenterEventData.mapId = _CurMapId;
        EventMgr.Instance.FireExecute(EnABBEvent.EVENT_BATTLE_MAP_REENTER, 0, 0, reenterEventData);
        ClassPoolMgr.Instance.Push(reenterEventData);

        UIMgr.Instance.HideWindow(_BattleMainWindowId);

        _BossDebutToken = null;

        DestroyMapBoss();

        CreateMapBoss(_CurMapId, _CurChapterId);

        for (int i = 0; i < _MapLifecycles.Count; i++)
        {
            _MapLifecycles[^(i + 1)].ReenterMap();
        }
        _BattleMainWindowId = UIMgr.Instance.ShowWindow<UIBattleMainWindowData>();
    }
    public void ExitMap()
    {
        if (_CurMapId <= 0)
            return;
        _BossDebutToken?.Cancel();
        EventMgr.Instance.Unregister(EnABBEvent.EVENT_BATTLE_WAVE_FINISH, 0, 0, OnEvent_WaveFinish);
        EventMgr.Instance.Unregister<GameBattleNextWaveEventData>(EnABBEvent.EVENT_BATTLE_NEXT_WAVE, 0, 0, OnEvent_NextWave);
        EventMgr.Instance.Unregister<GameSkillBossDebutEventData>(EnABBEvent.EVENT_SKILL_EVENT_SCHEDULE, 0, (int)EnSkillEventScheduleId.Debut, OnEvent_BossDebut);

        for (var i = 0; i < _MapLifecycles.Count; i++)
            _MapLifecycles[^(i + 1)].ExitMap();
        DestroyMapBoss();
        EntityMgr.Instance.RecycleEntityData(_BgEntityId);
        EntityMgr.Instance.RecycleEntityData(_FireHitEntityId);
        UIMgr.Instance.HideWindow(_BattleMainWindowId);
        _CurMapId = -1;
        _BgEntityId = -1;
        _FireHitEntityId = -1;
        _BattleMainWindowId = -1;
        _BossDebutToken = null;
        _MapDepth = -1;
    }
    public int GetMapBossEntityId()
    {
        return _MapBossEntityId;
    }
    private void CreateMapBackground(int mapId)
    {
        // 创建背景
        ref readonly var mapSize = ref GameMapMgr.Instance.GetMapSize();
        ref readonly var mapLbPos = ref GameMapMgr.Instance.GetMapStartPosition();
        _BgEntityId = EntityMgr.Instance.CreateEntityData<BattleMapBg_1Data>();
        GameEntityUtility.SetEntityPos(_BgEntityId, mapLbPos + mapSize * 0.5f);
        EntityMgr.Instance.LoadEntity(_BgEntityId);
    }
    private void CreateMap(int mapId)
    {

        // 创建网格
        ref readonly var rowCol = ref GameMapMgr.Instance.GetMapGridRowCol();
        ref readonly var startPos = ref GameMapMgr.Instance.GetGridLbPosition();
        ref readonly var gridSize = ref GameMapMgr.Instance.GetMapGridSize();
        for (int i = 0; i < rowCol.x; i++)
        {
            var posY = (i + 0.5f) * gridSize.y;
            for (int j = 0; j < rowCol.y; j++)
            {
                var index = i * rowCol.y + j;
                var gridLocalPos = new Vector2((j + 0.5f) * gridSize.x, posY);
                var pos = gridLocalPos + startPos;
                GameBattleGridMgr.Instance.CreateGridEntity(index, pos);
            }
        }
    }
    public bool IsExistBossWave()
    {
        var chapterCfg = GameSchedule.Instance.GetChapterCfg0(_CurChapterId);
        return chapterCfg.nBossWaveId > 0;
    }

    private void CreateMapBoss(int mapId, int chapterId)
    {
        var mapCfg = GameSchedule.Instance.GetMapCfg0(mapId);
        var chapterCfg = GameSchedule.Instance.GetChapterCfg0(chapterId);
        if (chapterCfg.nMapBossMonsterId <= 0)
            return;
        var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(chapterCfg.nMapBossMonsterId);
        var userData = ClassPoolMgr.Instance.Pull<SceneMonsterEntityUserData>();
        userData.monsterId = chapterCfg.nMapBossMonsterId;
        _MapBossEntityId = SceneEntityMgr.Instance.CreateEntityData(monsterCfg.nEntityTypeId, userData);
        ClassPoolMgr.Instance.Push(userData);
        EntityMgr.Instance.SetEntityWorldPos(in _MapBossEntityId, new Vector3(mapCfg.nMapBossPos[0], mapCfg.nMapBossPos[1]));
        EntityMgr.Instance.LoadEntity(_MapBossEntityId);

        SceneEntityMgr.Instance.AddCom(_MapBossEntityId, EnSceneEntityComType.Animation);
        SceneEntityMgr.Instance.AddCom(_MapBossEntityId, EnSceneEntityComType.Cmd);
        SceneEntityMgr.Instance.AddCmd(_MapBossEntityId, monsterCfg.nIdleCmdID);

        _MapBossCmdArray.Add(monsterCfg.nCmdHitLeft);
        _MapBossCmdArray.Add(monsterCfg.nCmdHitRight);
        _MapBossCmdArray.Add(monsterCfg.nCmdHitUp);
    }
    private void DestroyMapBoss()
    {
        if (_MapBossEntityId <= 0)
            return;

        SceneEntityMgr.Instance.RemoveCom(_MapBossEntityId, EnSceneEntityComType.Animation);
        SceneEntityMgr.Instance.RemoveCom(_MapBossEntityId, EnSceneEntityComType.Cmd);
        SceneEntityMgr.Instance.DestroyEntityData(_MapBossEntityId);

        _MapBossEntityId = -1;
    }
    private void OnEvent_WaveFinish(in int _, in int __)
    {
        DelayMonsterZeroCreateBoss();
    }
    private async void DelayMonsterZeroCreateBoss()
    {
        if (!IsExistBossWave())
            return;

        _BossDebutToken = new();
        var isCancel = await UniTask.WaitUntil(() => GameMonsterMgr.Instance.GetMonsterCount() == 0, cancelImmediately: true, cancellationToken: _BossDebutToken.Token)
            .SuppressCancellationThrow();
        if (!isCancel)
        {
            _BossDebutToken = null;
            var entityData = EntityMgr.Instance.GetEntityData<SceneMonsterEntityData>(in _MapBossEntityId);
            var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(entityData.GetMonsterCfgId());
            SceneEntityMgr.Instance.AddCmd(_MapBossEntityId, monsterCfg.nFlyDown);
        }
    }
    private void OnEvent_NextWave(in int _, in int __, in GameBattleNextWaveEventData data)
    {
        if (_MapBossEntityId > 0)
        {
            var cmdIndex = Random.Range(0, _MapBossCmdArray.Count);
            var cmd = _MapBossCmdArray[cmdIndex];
            SceneEntityMgr.Instance.AddCmd(in _MapBossEntityId, cmd);
        }

    }

    private void OnEvent_BossDebut(in int _, in int __, in GameSkillBossDebutEventData data)
    {
        DestroyMapBoss();

        var chapterCfg = GameSchedule.Instance.GetChapterCfg0(_CurChapterId);
        AudioMgr.Instance.Play(chapterCfg.nBossBgmAudioId);
    }

    public float GetLayerDepth(EnGameEntityOrderLayer layer, float offset = 0)
    {
        var depth = _MapDepth + (int)layer + offset;
        return depth;
    }

    #region 当前地图数据缓存

    private Vector2 _GridAreaLbPosition;
    private Vector2 _GridAreaRuPosition;
    private Vector2 _GridAreaSize;
    private Vector2 _MapStartPosition;
    private Vector2 _MapPlayerStartPosition;
    private Vector2 _MapSize;
    private Vector2 _MapGridSize;
    private Vector2Int _MapGridRowCol;
    private void UpdateMapCfgCache(int mapId)
    {
        var mapCfg = GameSchedule.Instance.GetMapCfg0(mapId);
        var size = new Vector3(mapCfg.fMapBound[3] - mapCfg.fMapBound[2], mapCfg.fMapBound[0] - mapCfg.fMapBound[1]);
        _GridAreaSize = size;

        _MapStartPosition = new Vector2(mapCfg.fMapLbPos[0], mapCfg.fMapLbPos[1]);

        var pos = new Vector2(mapCfg.fMapBound[2], mapCfg.fMapBound[1]);
        _GridAreaLbPosition = pos + _MapStartPosition;

        _GridAreaRuPosition = _GridAreaLbPosition + _GridAreaSize;

        var localPos = new Vector2(mapCfg.fPlayerLocalPos[0], mapCfg.fPlayerLocalPos[1]);
        _MapPlayerStartPosition = _MapStartPosition + localPos;

        _MapSize = new Vector2(mapCfg.fMapSize[0], mapCfg.fMapSize[1]);

        _MapGridSize = new Vector2(mapCfg.nGridSize[0], mapCfg.nGridSize[1]);

        _MapGridRowCol = new Vector2Int(mapCfg.nGridRowCol[0], mapCfg.nGridRowCol[1]);
    }
    public ref readonly Vector2 GetGridLbPosition() => ref _GridAreaLbPosition;
    public ref readonly Vector2 GetGridRuPosition() => ref _GridAreaRuPosition;
    public ref readonly Vector2 GetBattleAreaBoundSize() => ref _GridAreaSize;
    public ref readonly Vector2 GetMapStartPosition() => ref _MapStartPosition;
    public ref readonly Vector2 GetMapPlayerStartPosition() => ref _MapPlayerStartPosition;
    public ref readonly Vector2 GetMapSize() => ref _MapSize;
    public ref readonly Vector2Int GetMapGridRowCol() => ref _MapGridRowCol;
    public ref readonly Vector2 GetMapGridSize() => ref _MapGridSize;

    #endregion
}
