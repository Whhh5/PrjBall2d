using UnityEngine;

public static class GameMonsterUtility
{

    public static bool EntityIsValid(in int entityId)
    {
        if (!MonsterUtility.EntityIsValid(entityId))
            return false;
        return true;
    }
    public static bool EntityIsCanMove(int entityId)
    {
        if (MonsterUtility.EntityIsValid(entityId,
            EnSceneEntityStatus.Idle
                | EnSceneEntityStatus.Move
                | EnSceneEntityStatus.Hit, 
            EnSceneEntityStatus.None))
            return true;
        return false;
    }
    public static bool EntityIsAttack(int entityId)
    {
        if (MonsterUtility.EntityIsValid(entityId,
            EnSceneEntityStatus.Idle
                | EnSceneEntityStatus.Move
                | EnSceneEntityStatus.Hit,
            EnSceneEntityStatus.None))
            return true;
        return false;
    }

    public static void GetMonsterMainGridData(ref GameMonsterGridIndexData data, int entityId)
    {
        var worldPos = EntityMgr.Instance.GetEntityWorldPos2D(entityId);

        GetGridDataByWorldPos(ref data, in worldPos);
    }
    public static void GetGridDataByWorldPos(ref GameMonsterGridIndexData data, in Vector2 worldPos)
    {
        var mapId = GameMapMgr.Instance.GetCurMapId();
        var gridSize = GameMapUtility.GetMapGridSize(mapId);
        var maxRowCol = GameMapUtility.GetMapGridRowCol(mapId);
        var maxIndex = maxRowCol.x * maxRowCol.y - 1;

        var top = GameMapUtility.WorldPosToGridIndex(mapId, worldPos + new Vector2(0, gridSize.y * 0.5f));
        var center = GameMapUtility.WorldPosToGridIndex(mapId, worldPos);
        var bottom = GameMapUtility.WorldPosToGridIndex(mapId, worldPos - new Vector2(0, gridSize.y * 0.5f));
        data.top = Mathf.Clamp(top, 0, maxIndex);
        data.center = Mathf.Clamp(center, 0, maxIndex);
        data.bottom = Mathf.Clamp(bottom, 0, maxIndex);
    }


    #region 碰撞
    public static void OnCollision(int entityId, EnDirection dir)
    {
        //var userData = ClassPoolMgr.Instance.Pull<GameMonsterHitStatusUserData>();
        //userData.dir = dir;

        var mosterEntity = EntityMgr.Instance.GetEntityData<GameBattleMonsterEntityData>(entityId);
        var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(mosterEntity.GetMonsterCfgId());

        var cmdId = dir switch
        {
            EnDirection.Left => (EnEntityCmd)monsterCfg.nCmdHitLeft,
            EnDirection.Right => (EnEntityCmd)monsterCfg.nCmdHitRight,
            EnDirection.Top => (EnEntityCmd)monsterCfg.nCmdHitUp,
            EnDirection.Bottom => (EnEntityCmd)monsterCfg.nCmdHitDown,
            _ => EnEntityCmd.Boss1HitDown,
        };
        SceneEntityMgr.Instance.AddCmd(entityId, cmdId);

        //EnterStatus(entityId, EnSceneEntityStatus.Hit, userData);
        //ClassPoolMgr.Instance.Push(userData);
    }
    #endregion
}

