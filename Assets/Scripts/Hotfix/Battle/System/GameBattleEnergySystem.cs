using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameBattleEnergySystem : IBattleSystem
{
    private readonly Dictionary<int, GameBattleEnergyData> _EnergyEntityId2Data = new(10);
    private readonly Queue<int> _EnergyDelayMoveIds = new(10);
    private readonly List<int> _EnergyMoveIds = new(10);
    private float _LastPlayEffectTime;
    private Vector2 _EnergyTargetPos;
    private int _EnergyLevel;
    private int _EnergyValue;
    private int _MaxEnergyValue;

    public void OnEnable()
    {
        var mapId = GameMapMgr.Instance.GetCurMapId();
        var mapCfg = GameSchedule.Instance.GetMapCfg0(mapId);
        var chapterId = GameMapMgr.Instance.GetCurChapterId();
        var chapterCfg = GameSchedule.Instance.GetChapterCfg0(chapterId);
        _EnergyTargetPos = new Vector2(mapCfg.arrEnergyCollisionPoint[0], mapCfg.arrEnergyCollisionPoint[1]);
        EventMgr.Instance.Register<GameBattleMonsterDieEventData>(EnABBEvent.EVENT_BATTLE_MONSTER_DIE, 0, 0, OnEvent_MonsterDie);

        _MaxEnergyValue = chapterCfg.arrEnergyStageValue[_EnergyLevel];
    }
    public void OnDisable()
    {
        EventMgr.Instance.Unregister<GameBattleMonsterDieEventData>(EnABBEvent.EVENT_BATTLE_MONSTER_DIE, 0, 0, OnEvent_MonsterDie);
        ClearEnergyEntity();
    }

    public void OnReset()
    {
        ClearEnergyEntity();

        var chapterId = GameMapMgr.Instance.GetCurChapterId();
        var chapterCfg = GameSchedule.Instance.GetChapterCfg0(chapterId);
        _MaxEnergyValue = chapterCfg.arrEnergyStageValue[_EnergyLevel];
    }
    private void ClearEnergyEntity()
    {
        foreach (var (entityId, data) in _EnergyEntityId2Data)
        {
            ClassPoolMgr.Instance.Push(data);
            SceneEntityMgr.Instance.DestroyEntityData(entityId);
        }
        _EnergyEntityId2Data.Clear();
        _EnergyDelayMoveIds.Clear();
        _EnergyMoveIds.Clear();
        _LastPlayEffectTime
            = _EnergyLevel
            = _EnergyValue
            = _MaxEnergyValue
            = 0;
    }
    private void OnEvent_MonsterDie(in int nEnttiyId, in int _, in GameBattleMonsterDieEventData data)
    {
        var monsterEntityData = EntityMgr.Instance.GetEntityData<GameBattleMonsterEntityData>(data.entityId);
        var energyCount = monsterEntityData.GetEnergyCount();

        if (energyCount <= 0)
            return;
        ref readonly var gridSize = ref GameMapMgr.Instance.GetMapGridSize();
        var targetPos = GetFlyTargetPosition();
        for (int i = 0; i < energyCount; i++)
        {
            var pos = monsterEntityData.GetCenterPointWorldPos() +
                new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f)) * gridSize * monsterEntityData.GetVolume();
            var energyEntityId = SceneEntityMgr.Instance.CreateEntityData<GameBattleEnergyEntityData>();
            EntityMgr.Instance.LoadEntity(energyEntityId);
            EntityMgr.Instance.SetEntityWorldPos(energyEntityId, pos);

            var energyData = ClassPoolMgr.Instance.Pull<GameBattleEnergyData>();
            energyData.targetPos = targetPos;
            energyData.createTime = GameUtility.GetGameTime();
            energyData.startPos = pos;
            energyData.moveTime = Vector2.Distance(targetPos, pos) / 10f;
            energyData.delayTime = Random.Range(1f, 2f);
            energyData.energyValue = 1;
            _EnergyEntityId2Data.Add(energyEntityId, energyData);
            _EnergyDelayMoveIds.Enqueue(energyEntityId);
        }
    }

    public void OnUpdate()
    {
        var curTime = GameUtility.GetGameTime();
        if (_EnergyDelayMoveIds.TryPeek(out var delayEntityId))
        {
            var data = _EnergyEntityId2Data[delayEntityId];
            if (data.createTime + data.delayTime < curTime)
            {
                _EnergyMoveIds.Add(delayEntityId);
                _EnergyDelayMoveIds.Dequeue();
            }
        }

        for (int i = 0; i < _EnergyMoveIds.Count; i++)
        {
            var entityEntityId = _EnergyMoveIds[i];
            var data = _EnergyEntityId2Data[entityEntityId];
            var t = (curTime - (data.createTime + data.delayTime)) / data.moveTime;
            var pos = Vector2.Lerp(data.startPos, data.targetPos, t);
            EntityMgr.Instance.SetEntityWorldPos(entityEntityId, pos);
            if (Vector2.SqrMagnitude(pos - data.targetPos) < 0.05f)
            {
                if (curTime - _LastPlayEffectTime > 0.2f)
                {
                    _LastPlayEffectTime = curTime;
                    GameEffectMgr.Instance.PlayEffectOnce(EnGameEffectId.Eff_battle_energy_collistion, data.targetPos);
                    AudioMgr.Instance.Play(EnAudioId.Reward3);
                }

                var lastEnergy = _EnergyValue;
                var curEnergyValue = _EnergyValue + data.energyValue;

                while (curEnergyValue >= _MaxEnergyValue)
                {
                    var chapterId = GameMapMgr.Instance.GetCurChapterId();
                    var chapterCfg = GameSchedule.Instance.GetChapterCfg0(chapterId);
                    if (chapterCfg.nMaxEnergyStage > 0)
                    {
                        if (_EnergyLevel == chapterCfg.nMaxEnergyStage)
                            break;
                        _EnergyLevel = Mathf.Min(_EnergyLevel + 1, chapterCfg.nMaxEnergyStage);
                    }
                    else
                    {
                        _EnergyLevel++;
                    }
                    if (_EnergyLevel != chapterCfg.nMaxEnergyStage)
                    {
                        curEnergyValue -= _MaxEnergyValue;
                        _MaxEnergyValue = chapterCfg.arrEnergyStageValue[Mathf.Min(chapterCfg.arrEnergyStageValue.Length - 1, _EnergyLevel)];
                    }

                    var levelEvUd = ClassPoolMgr.Instance.Pull<GameBattleEnergyUplevelEventData>();
                    levelEvUd.currentLevel = _EnergyLevel;
                    levelEvUd.maxEnergyValue = _MaxEnergyValue;
                    EventMgr.Instance.FireExecute(EnABBEvent.EVENT_BATTLE_ENERGY_UPLEVEL, 0, 0, levelEvUd);
                    ClassPoolMgr.Instance.Push(levelEvUd);
                }
                _EnergyValue = curEnergyValue;

                var evUd = ClassPoolMgr.Instance.Pull<GameBattleEnergyChangeEventData>();
                evUd.currentValue = curEnergyValue;
                evUd.lastValue = lastEnergy;
                evUd.maxValue = _MaxEnergyValue;
                EventMgr.Instance.FireExecute(EnABBEvent.EVENT_BATTLE_ENERGY_CHANGE, 0, 0, evUd);
                ClassPoolMgr.Instance.Push(evUd);

                // 清理实体和数据
                SceneEntityMgr.Instance.DestroyEntityData(entityEntityId);
                ClassPoolMgr.Instance.Push(data);
                _EnergyEntityId2Data.Remove(entityEntityId);
                _EnergyMoveIds.RemoveAt(i--);
            }
        }
    }

    public ref readonly Vector2 GetFlyTargetPosition()
    {
        return ref _EnergyTargetPos;
    }
}
