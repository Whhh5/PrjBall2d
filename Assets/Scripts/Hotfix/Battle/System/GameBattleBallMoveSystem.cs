using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class GameBattleBallMoveSystem : IBattleSystem
{
    private List<int> _EntityIds = new(50);
    private GamePhysicsHit _Hit;

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
                var proCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(entityId, EnSceneEntityComType.Attribute);
                var speed = proCom.GetValue(EnEntityAttributeId.MoveSpeed);
                if (speed > 0)
                {
                    MoveBall(entityId, ballData.moveDir, speed, mapLbPos.y, gridLbPos.y);
                }
            }
        }
    }
    private void MoveBall(int entityId, Vector2 dir, float speed, float mapMinY, float gridMinY)
    {
        var gameBallEntityData = EntityMgr.Instance.GetEntityData<GameBattleSkillBallEntityData>(entityId);
        var radius = gameBallEntityData.GetSkillBallRadius();
        var curPos = (Vector2)EntityMgr.Instance.GetEntityWorldPos(entityId);

        var ballData = GameBattleBallMgr.Instance.GetBallMoveData(entityId);

        if (curPos.y >= gridMinY)
        {
            // 检测怪物碰撞
            if (GamePhysicsUtility.TrySphereCast(curPos, radius, dir, ref _Hit))
            {
                var angle = Vector2.Dot(_Hit.normal, dir);
                if (angle < 0)
                {
                    var ballCfg = GameSchedule.Instance.GetSkillBallCfg0(gameBallEntityData.GetSkillBallId());
                    if (ballCfg.bIsCollision > 0)
                    {
                        var reflect = Vector2.Reflect(dir, _Hit.normal);
                        dir = reflect;
                        ballData.moveDir = dir;

                        var evUd = ClassPoolMgr.Instance.Pull<GameSkillBallReflectEventData>();
                        evUd.ballEntityId = entityId;
                        EventMgr.Instance.FireExecute(EnABBEvent.EVENT_BATTLE_BALL_REFLECT, entityId, 0, evUd);
                        ClassPoolMgr.Instance.Push(evUd);
                    }
                    gameBallEntityData.Collision(in _Hit);
                }
            }
        }
        if (EntityMgr.Instance.IsValid(entityId))
        {
            var nextPos = curPos + speed * GameUtility.GetGameTimeDelta() * dir;

            if (nextPos.y < mapMinY) // 是否超出下边界
            {
                GameBattleBallMgr.Instance.DestroyBallEntity(entityId);
            }
            else
            {
                // 检测是否碰到了墙体
                if (CheckGridEage(ref nextPos, ref dir))
                {
                    ballData.moveDir = dir;

                    AudioMgr.Instance.Play(EnAudioId.Ball_Hit_Eage);
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
