using System;
using UnityEngine;
using static Codice.Client.Commands.WkTree.WorkspaceTreeNode;

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
        if (!MonsterUtility.EntityIsValid(entityId,
            EnSceneEntityStatus.Die
                | EnSceneEntityStatus.ForceMove
                | EnSceneEntityStatus.Freeze
                //| EnSceneEntityStatus.Fly
                | EnSceneEntityStatus.Atk
                | EnSceneEntityStatus.Skill1
                | EnSceneEntityStatus.Skill2
                | EnSceneEntityStatus.Skill3,
            EnSceneEntityStatus.None))
            return false;
        return true;
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


    #region 状态
    public static bool EnterStatus(int entityId, EnSceneEntityStatus status, IClassPoolDestroy userData)
    {
        return false;
        var entityData = EntityMgr.Instance.GetEntityData<GameBattleMonsterEntityData>(entityId);
        ref readonly var curStatus = ref entityData.GetSceneEntityStatus();
        if (curStatus > status)
            return false;
        entityData.AddMonsterStatus(status);
        PlayStatusAnim(entityId, status, userData);
        return true;
    }
    public static void ExitStatus(int entityId, EnSceneEntityStatus status, EnGameSpineCompleteStatus animStatus)
    {
        return;
        var entityData = EntityMgr.Instance.GetEntityData<GameBattleMonsterEntityData>(entityId);
        var curStatus = entityData.GetSceneEntityStatus();
        if ((curStatus & status) != status)
            return;
        switch (status)
        {
            case EnSceneEntityStatus.Die:
                if (animStatus == EnGameSpineCompleteStatus.Normal)
                {
                    GameMonsterMgr.Instance.DestroyMonsterEntity(entityId);
                }
                return;
            default:
                if (animStatus == EnGameSpineCompleteStatus.Normal)
                {
                    entityData.RemoveMonsterStatus(status);
                    EnterStatus(entityId, EnSceneEntityStatus.Idle, null);
                }
                break;
        }
    }
    public static void PlayStatusAnim(int entityId, EnSceneEntityStatus status, IClassPoolDestroy userData)
    {
        return;
        var animId = EnSpineAnimId.None;
        switch (status)
        {
            case EnSceneEntityStatus.Hit:
                {
                    //var data = userData as GameMonsterHitStatusUserData;
                    //switch (data.dir)
                    //{
                    //    case EnDirection.Top:
                    //    case EnDirection.Bottom:
                    //        animId = EnSpineAnimId.hit_3;
                    //        break;
                    //    case EnDirection.Left:
                    //        animId = EnSpineAnimId.hit_2;
                    //        break;
                    //    case EnDirection.Right:
                    //        animId = EnSpineAnimId.hit_3;
                    //        break;
                    //    default:
                    //        break;
                    //}
                }
                break;
            case EnSceneEntityStatus.Die: animId = EnSpineAnimId.die; break;
            case EnSceneEntityStatus.Idle: animId = EnSpineAnimId.stand; break;
            case EnSceneEntityStatus.Move: animId = EnSpineAnimId.stand; break;
            case EnSceneEntityStatus.Fly: animId = EnSpineAnimId.appear; break;
            case EnSceneEntityStatus.Atk: animId = EnSpineAnimId.attack; break;
            default:
                break;
        }
        var skeleCom = SceneEntityMgr.Instance.GetEntityCom<EntitySpineComData>(entityId, EnSceneEntityComType.Spine);
        skeleCom.PlayAnim(animId, status);
    }
    #endregion
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

