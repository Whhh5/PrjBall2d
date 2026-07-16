using System.Collections.Generic;
using System.Threading;
using Codice.CM.Client.Differences.Graphic;
using Cysharp.Threading.Tasks;
using UnityEngine;


public enum EnGameMapLifecycleOrder
{
    None,
    GameBattleGridMgr,
    GamePlayerMgr,
    GameBattleBallPartsMgr,
    GameBattleWaveMgr,
    GameBattleInputMgr,

    GameMonsterMgr,
    GameBattleBallMgr,
}
public interface IGameMapLifecycle
{
    public EnGameMapLifecycleOrder GetMapLifecycleOrder();
    public void EnterMap(int mapId);
    public void ExitMap(int mapId);
    public void ReenterMap(int mapId);
}
public class GameMapMgr : Singleton<GameMapMgr>
{
    private readonly List<IGameMapLifecycle> _MapLifecycles = new();
    private int _CurMapId = -1;
    private int _BgEntityId = -1;
    private int _FireHitEntityId = -1;
    private int _MapBossEntityId = -1;
    private CancellationTokenSource _BossDebutToken = null;
    public override void Destroy()
    {
        if (_CurMapId > 0)
        {
            ExitMap();
        }
        base.Destroy();
        _CurMapId
            = _BgEntityId
            = _FireHitEntityId
            = _MapBossEntityId
            = -1;
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
    public bool EnterMap(int mapId)
    {
        if (_CurMapId > 0)
            return false;
        var mapCfg = GameSchedule.Instance.GetMapCfg0(mapId);
        if (mapCfg == null)
            return false;
        GamePhysicsUtility.InitParams(mapId);
        _CurMapId = mapId;
        CreateMap(mapId);
        CreateMapBoss(mapId);

        CameraMgr.Instance.SetCameraPos(new Vector3(mapCfg.arrCameraPos[0], mapCfg.arrCameraPos[1], mapCfg.arrCameraPos[2]));

        foreach (var item in _MapLifecycles)
            item.EnterMap(mapId);

        EventMgr.Instance.Register<GameBattleNextWaveEventData>(EnABBEvent.EVENT_BATTLE_NEXT_WAVE, 0, 0, OnEvent_NextWave);
        EventMgr.Instance.Register(EnABBEvent.EVENT_BATTLE_WAVE_FINISH, 0, 0, OnEvent_WaveFinish);
        EventMgr.Instance.Register<GameSkillBossDebutEventData>(EnABBEvent.EVENT_SKILL_EVENT_SCHEDULE, 0, (int)EnSkillEventScheduleId.Debut, OnEvent_BossDebut);

        return true;
    }

    public void ReenterMap()
    {
        _BossDebutToken?.Cancel();
        _BossDebutToken = null;
        foreach (var item in _MapLifecycles)
            item.ReenterMap(_CurMapId);
    }
    public void ExitMap()
    {
        if (_CurMapId <= 0)
            return;
        _BossDebutToken?.Cancel();
        EventMgr.Instance.Unregister<GameBattleNextWaveEventData>(EnABBEvent.EVENT_BATTLE_NEXT_WAVE, 0, 0, OnEvent_NextWave);
        EventMgr.Instance.Unregister(EnABBEvent.EVENT_BATTLE_WAVE_FINISH, 0, 0, OnEvent_WaveFinish);
        EventMgr.Instance.Unregister<GameSkillBossDebutEventData>(EnABBEvent.EVENT_SKILL_EVENT_SCHEDULE, 0, (int)EnSkillEventScheduleId.Debut, OnEvent_BossDebut);

        for (var i = 0; i < _MapLifecycles.Count; i++)
            _MapLifecycles[^(i + 1)].ExitMap(_CurMapId);
        DestroyMapBoss();
        EntityMgr.Instance.RecycleEntityData(_BgEntityId);
        EntityMgr.Instance.RecycleEntityData(_FireHitEntityId);
        _CurMapId = -1;
        _BgEntityId = -1;
        _FireHitEntityId = -1;
        _BossDebutToken = null;
    }
    private void CreateMap(int mapId)
    {
        // 创建背景
        var mapSize = GameMapUtility.GetMapSize(mapId);
        var mapLbPos = GameMapUtility.GetMapStartPosition(mapId);
        _BgEntityId = EntityMgr.Instance.CreateEntityData<BattleMapBg_1Data>();
        GameEntityUtility.SetEntityPos(_BgEntityId, mapLbPos + mapSize * 0.5f);
        EntityMgr.Instance.LoadEntity(_BgEntityId);


        _FireHitEntityId = EntityMgr.Instance.CreateEntityData<GameEffectFireLineEntityData>();
        EntityMgr.Instance.LoadEntity(_FireHitEntityId);

        // 创建网格
        var rowCol = GameMapUtility.GetMapGridRowCol(mapId);
        var startPos = GameMapUtility.GetGridLbPosition(mapId);
        var gridSize = GameMapUtility.GetMapGridSize(mapId);
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

    private void CreateMapBoss(int mapId)
    {
        var mapCfg = GameSchedule.Instance.GetMapCfg0(mapId);
        var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(mapCfg.nMapBossMonsterId);
        var userData = ClassPoolMgr.Instance.Pull<SceneMonsterEntityUserData>();
        userData.monsterId = mapCfg.nMapBossMonsterId;
        _MapBossEntityId = SceneEntityMgr.Instance.CreateEntityData(monsterCfg.nEntityTypeId, userData);
        ClassPoolMgr.Instance.Push(userData);
        EntityMgr.Instance.SetEntityWorldPos(in _MapBossEntityId, new Vector3(mapCfg.nMapBossPos[0], mapCfg.nMapBossPos[1]));
        EntityMgr.Instance.LoadEntity(_MapBossEntityId);

        SceneEntityMgr.Instance.AddCom(_MapBossEntityId, EnSceneEntityComType.Animation);
        SceneEntityMgr.Instance.AddCom(_MapBossEntityId, EnSceneEntityComType.Cmd);
        SceneEntityMgr.Instance.AddCmd(_MapBossEntityId, monsterCfg.nIdleCmdID);
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
    public int GetMapBossEntityId()
    {
        return _MapBossEntityId;
    }

    private void OnEvent_NextWave(in int _, in int __, in GameBattleNextWaveEventData data)
    {
        if (_MapBossEntityId > 0)
        {
            var entityData = EntityMgr.Instance.GetEntityData<SceneMonsterEntityData>(in _MapBossEntityId);
            var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(entityData.GetMonsterCfgId());
            var arrCmd = new int[] { monsterCfg.nCmdHitLeft, monsterCfg.nCmdHitRight, monsterCfg.nCmdHitUp };
            var cmdIndex = Random.Range(0, arrCmd.Length);
            SceneEntityMgr.Instance.AddCmd(in _MapBossEntityId, arrCmd[cmdIndex]);
        }

    }
    private void OnEvent_WaveFinish(in int _, in int __)
    {
        DelayMonsterZeroCreateBoss();
    }
    private async void DelayMonsterZeroCreateBoss()
    {
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
    private void OnEvent_BossDebut(in int _, in int __, in GameSkillBossDebutEventData data)
    {
        DestroyMapBoss();
        var mapCfg = GameSchedule.Instance.GetMapCfg0(_CurMapId);
        var userData = ClassPoolMgr.Instance.Pull<CommonSerializeToIntArrayUserData>();
        userData.arrParams = mapCfg.nBossMonsterInfo;
        userData.startIndex = 0;
        userData.count = mapCfg.nBossMonsterInfo.Length;
        var waveData = ClassPoolMgr.Instance.Pull<GameBattleWaveMonsterInfo>(userData);
        ClassPoolMgr.Instance.Push(userData);
        var maxRowCol = GameMapUtility.GetMapGridRowCol(_CurMapId);
        GameMonsterMgr.Instance.CreateMonsterEntity(waveData.monsterId, maxRowCol.x - waveData.backRow, waveData.col, waveData.attValuesId);
        ClassPoolMgr.Instance.Push(waveData);
    }

}
