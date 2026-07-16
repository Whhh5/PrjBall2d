using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameMonsterMgr : Singleton<GameMonsterMgr>, IGameMapLifecycle
{
    private readonly HashSet<int> _MonsterEntityIds = new(100);
    private readonly Dictionary<int, GameMonsterGridIndexData> _MonsterGridDatas = new(100);

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
    public void EnterMap(int mapId)
    {
        EventMgr.Instance.Register<GameSkillMonsterDieEventData>(EnABBEvent.EVENT_SKILL_EVENT_SCHEDULE, 0, (int)EnSkillEventScheduleId.Die, OnEvent_SkillMonsterDie);
    }

    public void ExitMap(int mapId)
    {
        EventMgr.Instance.Unregister<GameSkillMonsterDieEventData>(EnABBEvent.EVENT_SKILL_EVENT_SCHEDULE, 0, (int)EnSkillEventScheduleId.Die, OnEvent_SkillMonsterDie);

        ClearMonster();
    }

    public void ReenterMap(int mapId)
    {
        ClearMonster();
    }

    private void OnEvent_SkillMonsterDie(in int nEntityId, in int nSkillEventId, in GameSkillMonsterDieEventData data)
    {
        GameMonsterMgr.Instance.DestroyMonsterEntity(nEntityId);
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
    }
    public List<int> GetMonsterList()
    {
        var list = _MonsterEntityIds.ToList();
        return list;
    }
    public int GetMonsterCount()
    {
        return _MonsterEntityIds.Count;
    }

    public void MonsterDie(int entityId)
    {
        var buffCom = SceneEntityMgr.Instance.GetEntityCom<EntityBuffComData>(entityId, EnSceneEntityComType.Buff);
        buffCom.ClearBuffs();
        var entityData = EntityMgr.Instance.GetEntityData<GameBattleMonsterEntityData>(entityId);
        var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(entityData.GetMonsterCfgId());
        entityData.SetSceneEntityStatus(EnSceneEntityStatus.Die);
        SceneEntityMgr.Instance.AddCmd(entityId, monsterCfg.nDieCmdID);
        GameBattleGridMgr.Instance.RemoveGridEntity(entityId);
    }
    public void DestroyMonsterEntity(int entityId)
    {
        if (!_MonsterEntityIds.Remove(entityId))
            return;
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
    public int CreateMonsterEntity(in int monsterId, in int row, in int col, in int attValueCfgId)
    {
        var gridIndex = GameMapUtility.CreateCurMapGridIndex(row, col); ;
        var userData = ClassPoolMgr.Instance.Pull<GameBattleMonsterEntityUserData>();
        userData.monsterId = monsterId;
        userData.gridPos = gridIndex;
        var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(monsterId);
        var entityId = SceneEntityMgr.Instance.CreateEntityData(monsterCfg.nEntityTypeId, userData);
        ClassPoolMgr.Instance.Push(userData);
        _MonsterEntityIds.Add(entityId);

        var pos = GameBattleGridMgr.Instance.GetGridWorldPos(userData.gridPos.index);
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
        return entityId;
    }

    private void SetEntityGrid(int entityId, Vector3 pos)
    {
        var mapId = GameMapMgr.Instance.GetCurMapId();
        var gridSize = GameMapUtility.GetMapGridSize(mapId);

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
        var mapId = GameMapMgr.Instance.GetCurMapId();
        var lastGridData = _MonsterGridDatas[entityId];


        var entityData = EntityMgr.Instance.GetEntityData<GameBattleMonsterEntityData>(entityId);
        ref readonly var lastGridIndex = ref entityData.GetMainGridIndex();
        var entityWorldPos = EntityMgr.Instance.GetEntityWorldPos2D(entityId);

        var gridData = ClassPoolMgr.Instance.Pull<GameMonsterGridIndexData>();
        GameMonsterUtility.GetGridDataByWorldPos(ref gridData, entityWorldPos);

        if (gridData == lastGridData)
        {
            ClassPoolMgr.Instance.Push(gridData);
            return;
        }
        var lastPos = lastGridData.lastPos;
        lastGridData.lastPos = entityWorldPos;
        var curGridIndex = GameMapUtility.WorldPosToGridIndex(mapId, entityWorldPos);
        var gridIndexData = GameMapUtility.CreateGridIndex(mapId, curGridIndex);
        entityData.SetGridIndex(gridIndexData);
        if (gridData.top != lastGridData.top)
            lastGridData.top = gridData.top;
        if (gridData.center != lastGridData.center)
            lastGridData.center = gridData.center;
        if (gridData.bottom != lastGridData.bottom)
            lastGridData.bottom = gridData.bottom;
        ClassPoolMgr.Instance.Push(gridData);


        var monsterCfg = MonsterUtility.GetMonsterCfg(entityId);
        var gridSize = GameMapUtility.GetMapGridSize(mapId);
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
}

