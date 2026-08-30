using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;

public enum EnGameMonsterType
{
    None,
    Normal,
    Elite,
    Boss,
}
public class GameMonsterMgr : Singleton<GameMonsterMgr>, IGameMapLifecycle
{
    private readonly HashSet<int> _MonsterEntityIds = new(100);
    private readonly Dictionary<int, GameMonsterGridIndexData> _MonsterGridDatas = new(100);
    private bool _DirtyMonster = false;
    private readonly List<int> _TempMonsterIds = new(100);

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
        return EnGameMapLifecycleOrder.GameMonsterMgr;
    }
    public void EnterMap(int mapId, int chapterId)
    {
        EventMgr.Instance.Register<GameBattleMouseClickEventData>(EnABBEvent.EVENT_BATTLE_MOUSE_CLICK, 0, 0, OnEvent_MouseClick);
        EventMgr.Instance.Register<GameSkillMonsterDieEventData>(EnABBEvent.EVENT_SKILL_EVENT_SCHEDULE, 0, (int)EnSkillEventScheduleId.Die, OnEvent_SkillMonsterDie);
        EventMgr.Instance.Register<GameSkillMonsterAttack1EventData>(EnABBEvent.EVENT_SKILL_EVENT_SCHEDULE, 0, (int)EnSkillEventScheduleId.Attack, OnEvent_SkillMonsterAttack);
    }

    public void ExitMap()
    {
        EventMgr.Instance.Unregister<GameBattleMouseClickEventData>(EnABBEvent.EVENT_BATTLE_MOUSE_CLICK, 0, 0, OnEvent_MouseClick);
        EventMgr.Instance.Unregister<GameSkillMonsterDieEventData>(EnABBEvent.EVENT_SKILL_EVENT_SCHEDULE, 0, (int)EnSkillEventScheduleId.Die, OnEvent_SkillMonsterDie);
        EventMgr.Instance.Unregister<GameSkillMonsterAttack1EventData>(EnABBEvent.EVENT_SKILL_EVENT_SCHEDULE, 0, (int)EnSkillEventScheduleId.Attack, OnEvent_SkillMonsterAttack);

        ClearMonster();
    }

    public void ReenterMap()
    {
        ClearMonster();
    }

    private void OnEvent_SkillMonsterDie(in int nEntityId, in int nSkillEventId, in GameSkillMonsterDieEventData data)
    {
        GameMonsterMgr.Instance.DestroyMonsterEntity(nEntityId);
    }

    private void OnEvent_SkillMonsterAttack(in int nEntityId, in int nSkillEventId, in GameSkillMonsterAttack1EventData data)
    {

    }

    private void ClearMonster()
    {
        foreach (var entityId in _MonsterEntityIds)
        {
            SceneEntityMgr.Instance.RemoveCom(entityId, EnSceneEntityComType.Buff);
            //SceneEntityMgr.Instance.RemoveCom(entityId, EnSceneEntityComType.Spine);
            SceneEntityMgr.Instance.RemoveCom(entityId, EnSceneEntityComType.Animation);
            SceneEntityMgr.Instance.RemoveCom(entityId, EnSceneEntityComType.Cmd);
            SceneEntityMgr.Instance.RemoveCom(entityId, EnSceneEntityComType.Attribute);
            SceneEntityMgr.Instance.DestroyEntityData(entityId);
        }
        foreach (var (_, data) in _MonsterGridDatas)
        {
            ClassPoolMgr.Instance.Push(data);
        }
        _MonsterEntityIds.Clear();
        _MonsterGridDatas.Clear();
        _DirtyMonster = true;
    }
    public List<int> GetMonsterList()
    {
        if (_DirtyMonster)
        {
            _DirtyMonster = false;
            _TempMonsterIds.Clear();
            _TempMonsterIds.AddRange(_MonsterEntityIds);
        }
        return _TempMonsterIds;
    }
    public int GetMonsterCount()
    {
        return _MonsterEntityIds.Count;
    }
    public void MonsterDie(int entityId)
    {
        var entityData = EntityMgr.Instance.GetEntityData<GameBattleMonsterEntityData>(entityId);
        entityData.SetSceneEntityStatus(EnSceneEntityStatus.Die);
        var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(entityData.GetMonsterCfgId());
        SceneEntityMgr.Instance.AddCmd(entityId, monsterCfg.nDieCmdID);

        var data = ClassPoolMgr.Instance.Pull<GameBattleMonsterDieEventData>();
        data.entityId = entityId;
        EventMgr.Instance.FireExecute(EnABBEvent.EVENT_BATTLE_MONSTER_DIE, entityId, 0, data);
        ClassPoolMgr.Instance.Push(data);

        BuffMgr.Instance.RemoveEntityBuffByEntityID(in entityId);
        GameBattleGridMgr.Instance.RemoveGridEntity(entityId);

        if (monsterCfg.nDieAudioId > 0)
            AudioMgr.Instance.Play(monsterCfg.nDieAudioId);
    }
    public void DestroyMonsterEntity(int entityId)
    {
        if (!_MonsterEntityIds.Remove(entityId))
            return;
        _DirtyMonster = true;
        var gridData = _MonsterGridDatas[entityId];
        ClassPoolMgr.Instance.Push(gridData);
        _MonsterGridDatas.Remove(entityId);
        GameBattleGridMgr.Instance.RemoveGridEntity(entityId);

        SceneEntityMgr.Instance.RemoveCom(entityId, EnSceneEntityComType.Buff);
        //SceneEntityMgr.Instance.RemoveCom(entityId, EnSceneEntityComType.Spine);
        SceneEntityMgr.Instance.RemoveCom(entityId, EnSceneEntityComType.Animation);
        SceneEntityMgr.Instance.RemoveCom(entityId, EnSceneEntityComType.Cmd);
        SceneEntityMgr.Instance.RemoveCom(entityId, EnSceneEntityComType.Attribute);
        SceneEntityMgr.Instance.DestroyEntityData(entityId);
    }
    public int CreateMonsterEntity(in int monsterId, in int row, in int col, in int attValueCfgId, in int energyCount)
    {
        var gridIndex = GameMapUtility.CreateGridIndex(row, col);
        var userData = ClassPoolMgr.Instance.Pull<GameBattleMonsterEntityUserData>();
        userData.monsterId = monsterId;
        userData.gridPos = gridIndex;
        userData.energyCount = energyCount;
        var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(monsterId);
        var entityId = SceneEntityMgr.Instance.CreateEntityData(monsterCfg.nEntityTypeId, userData);
        ClassPoolMgr.Instance.Push(userData);
        _MonsterEntityIds.Add(entityId);
        _DirtyMonster = true;

        var pos = GameBattleGridMgr.Instance.GetGridWorldPos(gridIndex.index);
        EntityMgr.Instance.SetEntityWorldPos(entityId, pos);
        //SceneEntityMgr.Instance.AddCom(entityId, EnSceneEntityComType.Spine);
        SceneEntityMgr.Instance.AddCom(entityId, EnSceneEntityComType.Animation);
        SceneEntityMgr.Instance.AddCom(entityId, EnSceneEntityComType.Cmd);

        SceneEntityMgr.Instance.AddCom(entityId, EnSceneEntityComType.Attribute);
        SceneEntityMgr.Instance.AddCom(entityId, EnSceneEntityComType.Buff);

        GameAttributeUtility.SetEntityAttributes(in entityId, in attValueCfgId);

        var gridData = ClassPoolMgr.Instance.Pull<GameMonsterGridIndexData>();
        GameMonsterUtility.GetGridDataByWorldPos(ref gridData, pos);
        gridData.lastPos = pos;
        _MonsterGridDatas.Add(entityId, gridData);

        // 设置网格
        SetEntityGrid(entityId, pos);

        EntityMgr.Instance.LoadEntity(entityId);

        SceneEntityMgr.Instance.AddCmd(in entityId, monsterCfg.nIdleCmdID);
        SceneEntityMgr.Instance.AddCmd(in entityId, monsterCfg.nFlyDown);

        CheckGridBalls(row, col, monsterId);

        var monsterEntity = EntityMgr.Instance.GetEntityData<GameBattleMonsterEntityData>(entityId);
        monsterEntity.ApplyData();
        return entityId;
    }

    private List<int> _BallEntityIds = new(50);
    private void CheckGridBalls(int row, int col, int monsterCfgId)
    {
        _BallEntityIds.Clear();
        GameBattleBallMgr.Instance.GetBallEntityIds(ref _BallEntityIds);

        var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(monsterCfgId);
        for (int i = 0; i < _BallEntityIds.Count; i++)
        {
            var ballEntityId = _BallEntityIds[i];
            var ballEntityData = EntityMgr.Instance.GetEntityData<GameBattleSkillBallEntityData>(ballEntityId);
            var ballCfg = GameSchedule.Instance.GetSkillBallCfg0(ballEntityData.GetSkillBallId());
            if (ballCfg.bMonsterDownClear <= 0)
                continue;

            var ballWorldPos = EntityMgr.Instance.GetEntityWorldPos2D(ballEntityId);
            if (!GameMapUtility.WorldPosToGridRowCol(in ballWorldPos, out var ballRow, out var ballCol))
                continue;
            for (int j = 0; j < monsterCfg.arrGridLocalPos.Length; j += 2)
            {
                var testRow = row + monsterCfg.arrGridLocalPos[j];
                var testCol = col + monsterCfg.arrGridLocalPos[j + 1];

                if (testRow != ballRow || testCol != ballCol)
                    continue;
                // 销毁这个球
                GameBattleBallMgr.Instance.DestroyBallEntity(ballEntityId);
                break;
            }
        }
    }

    private void SetEntityGrid(int entityId, Vector3 pos)
    {
        ref readonly var gridSize = ref GameMapMgr.Instance.GetMapGridSize();

        var monsterCfg = MonsterUtility.GetMonsterCfg(entityId);

        var gridData = ClassPoolMgr.Instance.Pull<GameMonsterGridIndexData>();
        for (int i = 0; i < monsterCfg.arrGridLocalPos.Length; i += 2)
        {
            var localGridOffsetRow = monsterCfg.arrGridLocalPos[i];
            var localGridOffsetCol = monsterCfg.arrGridLocalPos[i + 1];
            var localWorldPos = pos + new Vector3(localGridOffsetCol * gridSize.x, localGridOffsetRow * gridSize.y);
            GameMonsterUtility.GetGridDataByWorldPos(ref gridData, localWorldPos);

            GameBattleGridMgr.Instance.AddGridEntity(gridData.top, entityId);
            GameBattleGridMgr.Instance.AddGridEntity(gridData.center, entityId);
            GameBattleGridMgr.Instance.AddGridEntity(gridData.bottom, entityId);
        }
        ClassPoolMgr.Instance.Push(gridData);
    }

    public void CheckUpdateEntityGrid(int entityId)
    {
        var lastGridData = _MonsterGridDatas[entityId];


        var entityData = EntityMgr.Instance.GetEntityData<GameBattleMonsterEntityData>(entityId);
        ref readonly var lastGridIndex = ref entityData.GetMainGridIndex();
        var entityWorldPos = EntityMgr.Instance.GetEntityWorldPos2D(entityId);

        var gridData = ClassPoolMgr.Instance.Pull<GameMonsterGridIndexData>();
        GameMonsterUtility.GetGridDataByWorldPos(ref gridData, entityWorldPos);

        if (GameMonsterGridIndexData.Equals(gridData, lastGridData))
        {
            ClassPoolMgr.Instance.Push(gridData);
            return;
        }
        var lastPos = lastGridData.lastPos;
        lastGridData.lastPos = entityWorldPos;
        var curGridIndex = GameMapUtility.WorldPosToGridIndex(entityWorldPos);
        var gridIndexData = GameMapUtility.CreateGridIndex(curGridIndex);
        entityData.SetGridIndex(gridIndexData);
        if (gridData.top != lastGridData.top)
            lastGridData.top = gridData.top;
        if (gridData.center != lastGridData.center)
            lastGridData.center = gridData.center;
        if (gridData.bottom != lastGridData.bottom)
            lastGridData.bottom = gridData.bottom;
        ClassPoolMgr.Instance.Push(gridData);


        var monsterCfg = MonsterUtility.GetMonsterCfg(entityId);
        ref readonly var gridSize = ref GameMapMgr.Instance.GetMapGridSize();
        var lastGridIndexData = ClassPoolMgr.Instance.Pull<GameMonsterGridIndexData>();
        var curGridIndexData = ClassPoolMgr.Instance.Pull<GameMonsterGridIndexData>();
        for (int i = 0; i < monsterCfg.arrGridLocalPos.Length; i += 2)
        {
            var localGridOffsetRow = monsterCfg.arrGridLocalPos[i];
            var localGridOffsetCol = monsterCfg.arrGridLocalPos[i + 1];
            var offsetWorldPos = new Vector2(localGridOffsetCol * gridSize.x, localGridOffsetRow * gridSize.y);
            var localWorldPos = lastPos + offsetWorldPos;
            GameMonsterUtility.GetGridDataByWorldPos(ref lastGridIndexData, in localWorldPos);
            var curWorldPos = entityWorldPos + offsetWorldPos;
            GameMonsterUtility.GetGridDataByWorldPos(ref curGridIndexData, in curWorldPos);

            if (curGridIndexData.top != lastGridIndexData.top)
            {
                GameBattleGridMgr.Instance.RemoveGridEntity(lastGridIndexData.top, entityId);
                GameBattleGridMgr.Instance.AddGridEntity(curGridIndexData.top, entityId);
            }
            if (curGridIndexData.center != lastGridIndexData.center)
            {
                GameBattleGridMgr.Instance.RemoveGridEntity(lastGridIndexData.center, entityId);
                GameBattleGridMgr.Instance.AddGridEntity(curGridIndexData.center, entityId);
            }
            if (curGridIndexData.bottom != lastGridIndexData.bottom)
            {
                GameBattleGridMgr.Instance.RemoveGridEntity(lastGridIndexData.bottom, entityId);
                GameBattleGridMgr.Instance.AddGridEntity(curGridIndexData.bottom, entityId);
            }
        }
        ClassPoolMgr.Instance.Push(curGridIndexData);
        ClassPoolMgr.Instance.Push(lastGridIndexData);

    }
    private List<int> _MouseEntityIds = new();
    private void OnEvent_MouseClick(in int _, in int __, in GameBattleMouseClickEventData data)
    {
        var index = GameMapUtility.WorldPosToGridIndex(data.worldPos);
        _MouseEntityIds.Clear();
        GameBattleGridMgr.Instance.GetGridEntityIds(index, ref _MouseEntityIds);
        for (int i = 0; i < _MouseEntityIds.Count; i++)
        {
            var entityId = _MouseEntityIds[i];
            if (!IsMouseDownMoster(in entityId, in data.worldPos))
                continue;

            var monsterData = EntityMgr.Instance.GetEntityData<GameBattleMonsterEntityData>(entityId);
            var monsterId = monsterData.GetMonsterCfgId();
            var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(monsterId);
            var monsterName = LanguageUtility.GetLanguage(monsterCfg.nNameLangId);
            var tipsUserData = ClassPoolMgr.Instance.Pull<UIBattleEntityAttributeTipsUserData>();
            tipsUserData.title = monsterName;
            tipsUserData.targetEntityId = entityId;
            UIMgr.Instance.ShowWindow<UIBattleEntityAttributeTipsData>(tipsUserData);

            break;
        }

    }
    private bool IsMouseDownMoster(in int entityId, in Vector2 mousePos)
    {
        ref readonly var gridSize = ref GameMapMgr.Instance.GetMapGridSize();
        var testDisSqr = gridSize.x * gridSize.y / 2 / 2;
        var monsterData = EntityMgr.Instance.GetEntityData<GameBattleMonsterEntityData>(entityId);
        var monsterId = monsterData.GetMonsterCfgId();
        var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(monsterId);
        var mainWorldPos = monsterData.GetMainGridPos();
        var localMousePos = mousePos - mainWorldPos;
        for (int j = 0; j < monsterCfg.arrGridLocalPos.Length; j += 2)
        {
            var checkWorldPos = new Vector2()
            {
                y = monsterCfg.arrGridLocalPos[j] * gridSize.y,
                x = monsterCfg.arrGridLocalPos[j + 1] * gridSize.x,
            };

            var disSqr = Vector2.SqrMagnitude(localMousePos - checkWorldPos);
            if (disSqr < testDisSqr)
            {
                return true;
            }
        }
        return false;
    }
}

