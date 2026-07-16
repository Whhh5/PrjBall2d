using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class GameBattleBallMoveMgr : Singleton<GameBattleBallMoveMgr>
{
    public override EnManagerFuncType FuncType => base.FuncType | EnManagerFuncType.Update;

    private List<int> _EntityIds = new(50);
    private GamePhysicsHit _Hit;
    public override void Update()
    {
        base.Update();

        _EntityIds.Clear();
        GameBattleBallMgr.Instance.GetBallEntityIds(ref _EntityIds);

        if (_EntityIds.Count > 0)
        {
            var mapId = GameMapMgr.Instance.GetCurMapId();
            var mapLbPos = GameMapUtility.GetMapStartPosition(mapId);
            var gridLbPos = GameMapUtility.GetGridLbPosition(mapId);
            for (int i = 0; i < _EntityIds.Count; i++)
            {
                var entityId = _EntityIds[i];
                var ballData = GameBattleBallMgr.Instance.GetBallMoveData(entityId);
                MoveBall(entityId, ballData.moveDir, mapLbPos.y, gridLbPos.y);
            }
        }
    }
    private void MoveBall(int entityId, Vector2 dir, float mapMinY, float gridMinY)
    {
        var gameBallEntityData = EntityMgr.Instance.GetEntityData<GameBattleSkillBallEntityData>(entityId);
        var radius = gameBallEntityData.GetSkillBallRadius();
        var curPos = (Vector2)EntityMgr.Instance.GetEntityWorldPos(entityId);
        var proCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(entityId, EnSceneEntityComType.Attribute);

        var ballData = GameBattleBallMgr.Instance.GetBallMoveData(entityId);

        if (curPos.y >= gridMinY)
        {
            // 检测怪物碰撞
            if (GamePhysicsUtility.TrySphereCast(curPos, radius, dir, ref _Hit))
            {
                var angle = Vector2.Dot(_Hit.normal, dir);
                if (angle < 0)
                {
                    if (gameBallEntityData.IsCollision())
                    {
                        var reflect = Vector2.Reflect(dir, _Hit.normal);
                        dir = reflect;
                        ballData.moveDir = dir;
                    }
                    gameBallEntityData.Collision(in _Hit);
                }
            }
        }
        if (EntityMgr.Instance.IsValid(entityId))
        {
            var speed = proCom.GetValue(EnEntityAttributeId.MoveSpeed);
            var nextPos = curPos + speed * GameUtility.GetGameTimeDelta() * dir;

            if (nextPos.y < mapMinY) // 是否超出下边界
            {
                GameBattleBallMgr.Instance.DestroyBallEntity(entityId);
            }
            else
            {
                if (CheckGridEage(ref nextPos, ref dir))
                {
                    ballData.moveDir = dir;
                }
                EntityMgr.Instance.SetEntityWorldPos(entityId, nextPos);
            }
        }
    }

    private bool CheckGridEage(ref Vector2 pos, ref Vector2 dir)
    {
        var mapId = GameMapMgr.Instance.GetCurMapId();
        var lbPos = GameMapUtility.GetGridLbPosition(mapId);
        var ruPos = GameMapUtility.GetGridRuPosition(mapId);

        var result = false;
        if ((dir.y == 0 || dir.x < 0) && pos.x < lbPos.x)
        {
            pos.x = lbPos.x;
            dir.x *= -1;
            result = true;
        }
        else if ((dir.y == 0 || dir.x > 0) && pos.x > ruPos.x)
        {
            pos.x = ruPos.x;
            dir.x *= -1;
            result = true;
        }
        if ((dir.x == 0 || dir.y > 0) && pos.y > ruPos.y)
        {
            pos.y = ruPos.y;
            dir.y *= -1;
            result = true;
        }
        //else if ((dir.x == 0 || dir.y < 0) && pos.y < lbPos.y)
        //{
        //    pos.y = lbPos.y;
        //    dir.y *= -1;
        //    result = true;
        //}
        return result;
    }
}
