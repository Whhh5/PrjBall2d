using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class GameBattleWaveMgr : Singleton<GameBattleWaveMgr>, IGameMapLifecycle
{
    public override EnManagerFuncType FuncType => base.FuncType | EnManagerFuncType.Update;

    private readonly List<GameBattleWaveData> _WaveInfo = new();
    private int _CurWaveId;
    private bool _IsPlaying = false;

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
        return EnGameMapLifecycleOrder.GameBattleWaveMgr;
    }
    public void EnterMap(int mapId)
    {
        InitMonsterGrid(mapId);
        StartCreateMonster();
    }

    public void ExitMap(int mapId)
    {
        ClearWaveData();
    }

    public void ReenterMap(int mapId)
    {
        ClearWaveData();
        InitMonsterGrid(mapId);
        StartCreateMonster();
    }

    private void InitMonsterGrid(int mapId)
    {
        var mapCfg = GameSchedule.Instance.GetMapCfg0(mapId);
        var mapRowCol = GameMapUtility.GetMapGridRowCol(mapId);
        for (int i = mapCfg.minWaveId; i <= mapCfg.maxWaveId; i++)
        {
            var waveCfg = GameSchedule.Instance.GetBattleWaveCfg0(i);
            var index = 0;
            var userData = ClassPoolMgr.Instance.Pull<CommonSerializeToIntArrayUserData>();
            userData.arrParams = waveCfg.arrMonsterInfos;
            userData.startIndex = 0;
            userData.count = waveCfg.arrMonsterInfos.Length;
            ISerializeToIntArray.ParseToValue(userData.arrParams, userData.startIndex, userData.count, ref index, out List<GameBattleWaveMonsterInfo> waveMonsterInfos, userData);
            ClassPoolMgr.Instance.Push(userData);

            var waveInfo = ClassPoolMgr.Instance.Pull<GameBattleWaveData>();
            for (int j = 0; j < waveMonsterInfos.Count; j++)
            {
                var waveMonsterInfo = waveMonsterInfos[j];
                var createInfo = ClassPoolMgr.Instance.Pull<GameMonsterCreateInfo>();
                createInfo.Init(j + 1, waveMonsterInfo.monsterId, mapRowCol.x - waveMonsterInfo.backRow, waveMonsterInfo.col, waveMonsterInfo.attValuesId);
                waveInfo.AddMonsterCreateInfo(createInfo);
            }
            _WaveInfo.Add(waveInfo);
        }
    }

    private bool StartCreateMonster()
    {
        if (_WaveInfo.Count == 0)
            return false;
        _CurWaveId = 0;

        _IsPlaying = true;
        return true;
    }
    private void ClearWaveData()
    {
        foreach (var item in _WaveInfo)
        {
            ClassPoolMgr.Instance.Push(item);
        }
        _WaveInfo.Clear();
        _CurWaveId = -1;
        _IsPlaying = false;
    }
    public int GetCurWaveId()
    {
        return _CurWaveId;
    }
    public override bool IsUpdate()
    {
        return base.IsUpdate() && GameMapMgr.Instance.IsValid();
    }
    public override void Update()
    {
        base.Update();

        if (!_IsPlaying)
            return;

        var maxWave = GetMaxWave();
        if (_CurWaveId == maxWave)
            return;

        var waveInfo = _WaveInfo[_CurWaveId];
        if (!waveInfo.TryGetNextCreateIndex(out var createInfo))
        {
            var lastWave = _CurWaveId++;
            if (_CurWaveId == maxWave)
            {
                EventMgr.Instance.FireExecute(EnABBEvent.EVENT_BATTLE_WAVE_FINISH);
            }
            else
            {
                // 下一波
                var evUserData = ClassPoolMgr.Instance.Pull<GameBattleNextWaveEventData>();
                evUserData.lastWaveId = lastWave;
                evUserData.curWaveId = _CurWaveId;
                EventMgr.Instance.FireExecute(EnABBEvent.EVENT_BATTLE_NEXT_WAVE, 0, 0, evUserData);
                ClassPoolMgr.Instance.Push(evUserData);
            }
            return;
        }

        var posList = GameEntityUtility.GetEntityGridPosList(createInfo.monsterId, createInfo.row, createInfo.col);
        if (!GameMapUtility.GridsIsIdle(posList))
            return;

        // 创建怪物
        GameMonsterMgr.Instance.CreateMonsterEntity(createInfo.monsterId, createInfo.row, createInfo.col, createInfo.attValueCfgId);
        waveInfo.CreateFinish(createInfo.index);
    }

    private int GetMaxWave()
    {
        return _WaveInfo.Count;
    }
}

