using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameBattleWaveSystem : IBattleSystem
{
    private readonly Queue<int> _WaveInfo = new();
    private readonly Dictionary<int, GameBattleWaveData> _WaveId2Data = new();
    private int _CurWaveId;
    private int _MaxWaveId;
    private bool _IsPlaying = false;
    private readonly Dictionary<int, int> _MonsterId2AttributeGroupIds = new();

    public void OnEnable()
    {
        var mapId = GameMapMgr.Instance.GetCurMapId();
        var chapterId = GameMapMgr.Instance.GetCurChapterId();
        var chapterCfg = GameSchedule.Instance.GetChapterCfg0(chapterId);
        for (int i = 0; i < chapterCfg.arrMonsterIds.Length; i += 2)
        {
            var monsterId = chapterCfg.arrMonsterIds[i];
            var attGroupId = chapterCfg.arrMonsterIds[i + 1];
            _MonsterId2AttributeGroupIds.Add(monsterId, attGroupId);
        }

        InitMonsterGrid(mapId);
        StartCreateMonster();
    }

    public void OnDisable()
    {
        ClearWaveData();
        _MonsterId2AttributeGroupIds.Clear();
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
        var chapterId = GameMapMgr.Instance.GetCurChapterId();
        var chapterCfg = GameSchedule.Instance.GetChapterCfg0(chapterId);
        for (int waveId = chapterCfg.nMinWaveId; waveId <= chapterCfg.nMaxWaveId; waveId++)
        {
            RegisterWaveInfo(waveId);
            _WaveInfo.Enqueue(waveId);
        }
        _MaxWaveId = chapterCfg.nMaxWaveId;
        if (chapterCfg.nBossWaveId > 0)
            RegisterWaveInfo(chapterCfg.nBossWaveId);
    }

    public void RegisterWaveInfo(in int waveId)
    {
        ref readonly var mapRowCol = ref GameMapMgr.Instance.GetMapGridRowCol();
        var waveCfg = GameSchedule.Instance.GetBattleWaveCfg0(waveId);
        var chapterId = GameMapMgr.Instance.GetCurChapterId();
        var chapterCfg = GameSchedule.Instance.GetChapterCfg0(chapterId);
        var index = 0;
        var userData = ClassPoolMgr.Instance.Pull<CommonSerializeToIntArrayUserData>();
        userData.arrParams = waveCfg.arrMonsterInfos;
        userData.startIndex = 0;
        userData.count = waveCfg.arrMonsterInfos.Length;
        ISerializeToIntArray.ParseToValue(userData.arrParams, userData.startIndex, userData.count, ref index, out List<GameBattleWaveMonsterInfo> waveMonsterInfos, userData);
        ClassPoolMgr.Instance.Push(userData);

        var waveInfo = ClassPoolMgr.Instance.Pull<GameBattleWaveData>();;
        for (int j = 0; j < waveMonsterInfos.Count; j++)
        {
            var waveMonsterInfo = waveMonsterInfos[j];
            var createInfo = ClassPoolMgr.Instance.Pull<GameMonsterCreateInfo>();
            createInfo.Init(
                j + 1,
                waveMonsterInfo.monsterId,
                mapRowCol.x - waveMonsterInfo.backRow,
                waveMonsterInfo.col,
                waveMonsterInfo.attributeCofficient * chapterCfg.fCofficient
                );
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
    public int GetWaveCount()
    {
        return _WaveId2Data.Count;
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
            waveInfo.CreateFinish(createInfo.index);
            var waveCofficient = Mathf.Floor(_CurWaveId / 10);
            CreateMonster(createInfo.monsterId, createInfo.row, createInfo.col, createInfo.attCofficient + waveCofficient);
        }
        var lastWave = _CurWaveId++;
        // 下一波
        var evUserData = ClassPoolMgr.Instance.Pull<GameBattleNextWaveEventData>();
        evUserData.lastWaveId = lastWave;
        evUserData.curWaveId = _CurWaveId;
        EventMgr.Instance.FireExecute(EnABBEvent.EVENT_BATTLE_NEXT_WAVE, 0, 0, evUserData);
        ClassPoolMgr.Instance.Push(evUserData);

        var chapterId = GameMapMgr.Instance.GetCurChapterId();
        var chapterCfg = GameSchedule.Instance.GetChapterCfg0(chapterId);
        if (waveId == chapterCfg.nMaxWaveId)
        {
            EventMgr.Instance.FireExecute(EnABBEvent.EVENT_BATTLE_WAVE_FINISH);
        }
    }
    public void CreateWaveMonsterEntitys(in int waveId)
    {
        _WaveInfo.Enqueue(waveId);
    }
    public int CreateMonster(int monsterCfgId, int row, int col, float attCofficient)
    {
        if (_MonsterId2AttributeGroupIds.TryGetValue(monsterCfgId, out var attGroupId))
        {
            // 创建怪物
            var monsterId = GameMonsterMgr.Instance.CreateMonsterEntity(monsterCfgId, row, col, attGroupId, attCofficient);
            return monsterId;
        }
        else
        {
            AotDebug.LogError($"意外的怪物id，请在 {nameof(ChapterCfg)}.{nameof(ChapterCfg.arrMonsterIds)} 中配置");
            return -1;
        }
    }
}

