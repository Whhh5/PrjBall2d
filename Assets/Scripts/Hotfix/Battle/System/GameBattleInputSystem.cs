using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameBattleInputSystem : IBattleSystem
{
    private bool _MouseIsValid = false;

    private Vector2 _LeftDirection;
    private Vector2 _RightDirection;
    private float _LeftAngle;
    private float _RightAngle;

    private Vector2 _MouseDownPos;
    private float _LastDownTime;

    public void OnEnable()
    {
        var mapId = GameMapMgr.Instance.GetCurMapId();
        var gridLbPos = GameMapUtility.GetGridLbPosition(mapId);
        var gridRuPos = GameMapUtility.GetGridRuPosition(mapId);

        var playerStart = GameMapUtility.GetMapPlayerStartPosition(mapId);
        _LeftDirection = (gridLbPos - playerStart).normalized;
        _RightDirection = (new Vector2(gridRuPos.x, gridLbPos.y) - playerStart).normalized;
        _LeftAngle = Vector2.Angle(Vector3.up, _LeftDirection);
        _RightAngle = Vector2.Angle(Vector3.up, _RightDirection);
    }
    public void OnReset()
    {

    }
    public void OnDisable()
    {
        
    }

    public void OnUpdate()
    {
        if (!_MouseIsValid)
        {
            if (Input.GetMouseButton(0))
            {
                if (EventSystem.current != null && !EventSystem.current.IsPointerOverGameObject())
                {
                    _MouseIsValid = true;

                    _MouseDownPos = UIUtil.ViewToWorld(GetMouseScreenPos());
                    _LastDownTime = GameUtility.GetGameTime();
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            _MouseIsValid = false;

            var curTime = GameUtility.GetGameTime();
            if (curTime - _LastDownTime < 0.1f)
            {
                var curWorldPos = UIUtil.ViewToWorld(GetMouseScreenPos());
                if (Vector3.SqrMagnitude(curWorldPos - _MouseDownPos) < 0.01f)
                {
                    var userData = ClassPoolMgr.Instance.Pull<GameBattleMouseClickEventData>();
                    userData.worldPos = curWorldPos;
                    EventMgr.Instance.FireExecute(EnABBEvent.EVENT_BATTLE_MOUSE_CLICK, 0, 0, userData);
                    ClassPoolMgr.Instance.Push(userData);
                }
            }
        }
    }
    public bool MouseIsValid()
    {
        return _MouseIsValid;
    }
    public Vector2 GetMouseScreenPos()
    {
        return Input.mousePosition;
    }

    public float GetFireRadius()
    {
        return 1.5f;
    }
    public Vector2 GetFireCenter()
    {
        var mapId = GameMapMgr.Instance.GetCurMapId();
        var playerStart = GameMapUtility.GetMapPlayerStartPosition(mapId);
        return playerStart;
    }
    public Vector2 GetFireDirection()
    {
        var mousePos = GameUtility.GetMousePos();
        var playerScreenPos = CameraMgr.Instance.WorldPosToScreenPos(GetFireCenter());
        var dir = (mousePos - playerScreenPos).normalized;

        var angle = Vector2.Angle(Vector3.up, dir);
        if (dir.x > 0)
        {
            if (angle > _RightAngle)
            {
                return _RightDirection;
            }
        }
        else
        {
            if (angle > _LeftAngle)
            {
                return _LeftDirection;
            }
        }

        return dir;
    }
    public Vector2 GetFirePos()
    {
        var dir = GetFireDirection();
        var firePoint = GetFireRadius() * dir + GetFireCenter();
        return firePoint;
    }

}
