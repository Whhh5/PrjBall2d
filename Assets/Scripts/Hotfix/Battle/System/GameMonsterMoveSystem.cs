using System.Collections.Generic;
using UnityEngine;

public class GameMonsterMoveSystem : IBattleSystem
{
    private List<int> _EntityIds = new(3);

    public void OnEnable()
    {

    }

    public void OnDisable()
    {

    }

    public void OnReset()
    {

    }
    public void OnUpdate()
    {
        var monsterList = GameMonsterMgr.Instance.GetMonsterList();
        for (int i = 0; i < monsterList.Count; i++)
        {
            var entityId = monsterList[i];
            MoveEntity(entityId);
            CheckUpdate(entityId);
            CheckAttack(entityId);
        }
    }
    private void MoveEntity(int entityId)
    {
        if (!GameMonsterUtility.EntityIsValid(entityId))
            return;
        // 判断是否有碰撞, 以限制移动位置
        ref readonly var gridSize = ref GameMapMgr.Instance.GetMapGridSize();
        var pointInfo = EntityMgr.Instance.GetEntityData<IEntityGridPointInfo>(entityId);
        var mainIndex = pointInfo.GetMainGridIndex();
        var mainPos = pointInfo.GetMainGridPos();
        var posList = pointInfo.GetGridIndexs();
        _EntityIds.Clear();

        var maxLocalRow = posList[^2];
        var maxLocalCol = 0;
        var barkMoveY = 0f;
        for (int i = 0; i < posList.Length; i += 2)
        {
            var localRow = posList[i];
            if (maxLocalRow != localRow)
                continue;
            var localCol = posList[i + 1];
            maxLocalCol = Mathf.Max(maxLocalCol, Mathf.Abs(localCol));

            var curIndexBtmY = mainPos.y + (localRow - 0.5f) * gridSize.y;
            var testRow = GameMapUtility.WorldPosY2Row(in curIndexBtmY);
            var testCol = mainIndex.col + localCol;
            var index = GameMapUtility.RowColToIndex(in testRow, in testCol);
            _EntityIds.Clear();
            GameBattleGridMgr.Instance.GetGridEntityIds(index, ref _EntityIds);

            for (int j = 0; j < _EntityIds.Count; j++)
            {
                var testEntityId = _EntityIds[j];
                if (testEntityId == entityId)
                    continue;
                var testPointInfo = EntityMgr.Instance.GetEntityData<IEntityGridPointInfo>(testEntityId);
                var testMainPos = testPointInfo.GetMainGridPos();
                if (testMainPos.y > mainPos.y)
                    continue;
                var testTopPosY = testMainPos.y + gridSize.y * 0.5f;
                if (testTopPosY < curIndexBtmY)
                    continue;
                barkMoveY = Mathf.Max(testTopPosY - curIndexBtmY, barkMoveY);
            }
        }


        // 移动
        var lastPos = EntityMgr.Instance.GetEntityWorldPos(entityId);
        var sizeY = (Mathf.Abs(maxLocalRow) + 1) * gridSize.y;
        var halfGridSizeY = gridSize.y / 2;
        var nextPosY = 0f;
        if (Mathf.Approximately(0, barkMoveY))
        {
            // 判断是否可以移动
            if (GameMonsterUtility.EntityIsCanMove(entityId))
            {
                var proCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(entityId, EnSceneEntityComType.Attribute);
                var moveSpeed = proCom.GetValue(EnEntityAttributeId.MoveSpeed);
                nextPosY = lastPos.y - moveSpeed * GameUtility.GetGameTimeDelta();
            }
            else
                return;
        }
        else
        {
            nextPosY = lastPos.y + barkMoveY;
        }
        var newPos = new Vector2(lastPos.x, nextPosY);
        GameMapUtility.CheckMoveBoundsY(ref newPos.y, -halfGridSizeY, sizeY - halfGridSizeY);

        GameEntityUtility.SetEntityPos(entityId, newPos);
        GameMonsterMgr.Instance.CheckUpdateEntityGrid(entityId);
    }
    public void CheckUpdate(int entityId)
    {
        var monsterEntityData = EntityMgr.Instance.GetEntityData<GameBattleMonsterEntityData>(entityId);
        monsterEntityData.MonsterUpdate();
    }
    public void CheckAttack(int entityId)
    {
        var monsterEntityData = EntityMgr.Instance.GetEntityData<GameBattleMonsterEntityData>(entityId);
        ref readonly var battleAreaLb = ref GameMapMgr.Instance.GetGridLbPosition();
        ref readonly var gridSize = ref GameMapMgr.Instance.GetMapGridSize();
        ref readonly var gridPosArr = ref monsterEntityData.GetGridIndexs();
        var minRowOffset = gridPosArr[^2];
        var entityWorldPos = EntityMgr.Instance.GetEntityWorldPos2D(entityId);
        var minPosY = entityWorldPos.y + (minRowOffset - 0.5f) * gridSize.y;
        var isDown = minPosY <= battleAreaLb.y;

        var lastIsDown = monsterEntityData.GetIsDown();
        if (isDown != lastIsDown)
        {
            if (isDown)
            {
                monsterEntityData.EnterMapDown();
            }
            else
            {
                monsterEntityData.ExitMapDown();
            }
            monsterEntityData.SetIsDown(isDown);
        }
        if (isDown)
        {
            monsterEntityData.UpdateMapDown();
        }
    }
}
