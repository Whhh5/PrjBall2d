using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class GameBattleWaveSystem : IBattleSystem
{
    private readonly Queue<int> _WaveInfo = new();
    private readonly Dictionary<int, GameBattleWaveData> _WaveId2Data = new();
    private int _CurWaveId;
    private int _MaxWaveId;
    private bool _IsPlaying = false;

    public void OnEnable()
    {
        var mapId = GameMapMgr.Instance.GetCurMapId();
        InitMonsterGrid(mapId);
        StartCreateMonster();
    }

    public void OnDisable()
    {
        ClearWaveData();
    }

    public void OnReset()
    {
        var mapId = GameMapMgr.Instance.GetCurMapId();
        ClearWaveData();
        InitMonsterGrid(mapId);
        StartCreateMonster();
    }

    private void InitMonsterGrid(int mapId)
    {
        var mapCfg = GameSchedule.Instance.GetMapCfg0(mapId);
        for (int waveId = mapCfg.nMinWaveId; waveId <= mapCfg.nMaxWaveId; waveId++)
        {
            RegisterWaveInfo(mapId, waveId);
            _WaveInfo.Enqueue(waveId);
        }
        _MaxWaveId = mapCfg.nMaxWaveId;
        RegisterWaveInfo(mapId, mapCfg.nBossWaveId);
    }

    public void RegisterWaveInfo(in int mapId, in int waveId)
    {
        var mapRowCol = GameMapUtility.GetMapGridRowCol(mapId);
        var waveCfg = GameSchedule.Instance.GetBattleWaveCfg0(waveId);
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
            createInfo.Init(j + 1, waveMonsterInfo.monsterId, mapRowCol.x - waveMonsterInfo.backRow, waveMonsterInfo.col, waveMonsterInfo.attValuesId, waveMonsterInfo.energyCount);
            waveInfo.AddMonsterCreateInfo(createInfo);
        }
        _WaveId2Data.Add(waveId, waveInfo);
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
        foreach (var (_, data) in _WaveId2Data)
        {
            ClassPoolMgr.Instance.Push(data);
        }
        _WaveInfo.Clear();
        _CurWaveId = -1;
        _IsPlaying = false;
        _WaveId2Data.Clear();
    }
    public int GetCurWaveId()
    {
        return _CurWaveId;
    }
    public int GetResidueWaveCount()
    {
        return _WaveInfo.Count;
    }
    public bool IsFinish()
    {
        return _WaveId2Data.Count == _CurWaveId;
    }
    public void OnUpdate()
    {
        if (!_IsPlaying)
            return;

        if (!_WaveInfo.TryPeek(out var waveId))
            return;
        var waveInfo = _WaveId2Data[waveId];

        if (!waveInfo.IsCreateWaveMonsters())
            return;
        _WaveInfo.Dequeue();

        var infos = waveInfo.GetMonsterCreateInfos();
        for (int i = 0; i < infos.Count; i++)
        {
            var createInfo = infos[i];

            // 创建怪物
            GameMonsterMgr.Instance.CreateMonsterEntity(createInfo.monsterId, createInfo.row, createInfo.col, createInfo.attValueCfgId, createInfo.energyCount);
            waveInfo.CreateFinish(createInfo.index);
        }
        var lastWave = _CurWaveId++;
        // 下一波
        var evUserData = ClassPoolMgr.Instance.Pull<GameBattleNextWaveEventData>();
        evUserData.lastWaveId = lastWave;
        evUserData.curWaveId = _CurWaveId;
        EventMgr.Instance.FireExecute(EnABBEvent.EVENT_BATTLE_NEXT_WAVE, 0, 0, evUserData);
        ClassPoolMgr.Instance.Push(evUserData);

        var mapId = GameMapMgr.Instance.GetCurMapId();
        var mapCfg = GameSchedule.Instance.GetMapCfg0(mapId);
        if (waveId == mapCfg.nMaxWaveId)
        {
            EventMgr.Instance.FireExecute(EnABBEvent.EVENT_BATTLE_WAVE_FINISH);
        }
    }
    public void CreateWaveMonsterEntitys(in int waveId)
    {
        _WaveInfo.Enqueue(waveId);
    }
}

