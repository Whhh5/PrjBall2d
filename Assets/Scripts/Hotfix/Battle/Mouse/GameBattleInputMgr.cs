using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameBattleInputMgr : Singleton<GameBattleInputMgr>, IGameMapLifecycle
{
    public override EnManagerFuncType FuncType => base.FuncType | EnManagerFuncType.Update;

    private bool _MouseIsValid = false;

    private Vector2 _LeftDirection;
    private Vector2 _RightDirection;
    private float _LeftAngle;
    private float _RightAngle;

    public override async UniTask OnEnableAsync()
    {
        await base.OnEnableAsync();
        GameMapMgr.Instance.RegisterLifecycle(this);
    }
    public override void OnDisable()
    {
        GameMapMgr.Instance.UnregisterLifecycle(this);
        base.OnDisable();
    }
    public EnGameMapLifecycleOrder GetMapLifecycleOrder() => EnGameMapLifecycleOrder.GameBattleInputMgr;

    public void EnterMap(int mapId)
    {
        var maId = GameMapMgr.Instance.GetCurMapId();
        var gridLbPos = GameMapUtility.GetGridLbPosition(maId);
        var gridRuPos = GameMapUtility.GetGridRuPosition(maId);

        var playerStart = GameMapUtility.GetMapPlayerStartPosition(mapId);
        _LeftDirection = (gridLbPos - playerStart).normalized;
        _RightDirection = (new Vector2(gridRuPos.x, gridLbPos.y) - playerStart).normalized;
        _LeftAngle = Vector2.Angle(Vector3.up, _LeftDirection);
        _RightAngle = Vector2.Angle(Vector3.up, _RightDirection);
    }

    public void ExitMap(int mapId)
    {
    }

    public void ReenterMap(int mapId)
    {
    }

    public bool IsValid()
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
    public override void Update()
    {
        base.Update();

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && !EventSystem.current.IsPointerOverGameObject())
            {
                _MouseIsValid = true;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _MouseIsValid = false;
        }
    }
}
