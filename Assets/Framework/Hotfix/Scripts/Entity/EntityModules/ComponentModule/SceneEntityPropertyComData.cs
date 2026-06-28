using UnityEngine;

public class SceneEntityPropertyComData : Entity3DComData
{
    public override EnSceneEntityComType GetComType() => EnSceneEntityComType.Property;

    // 移动参数
    private int _DisableMoveCount = 0;
    private float _BaseMoveSpeed;
    private float _MoveSeedIncrement;
    private float _MoveSpeed;
    // 旋转参数
    private int _DisableRotationCount = 0;
    private float _BaseRotateSpeed;
    private float _RotateSpeedIncrement;
    private float _RotateSpeed;

    #region 移动参数

    public void SetIsCanMove(in bool isCanMove)
    {
        _DisableMoveCount += isCanMove ? -1 : 1;
    }

    public bool GetIsCanMove()
    {
        return _DisableMoveCount == 0;
    }
    private float CalculateMoveSpeed()
    {
        var speed = Mathf.Clamp(1 + _MoveSeedIncrement, 0, 5) * _BaseMoveSpeed;
        return speed;
    }
    public void SetBaseMoveSpeed(in float moveSpeed)
    {
        _BaseMoveSpeed = moveSpeed;
        _MoveSpeed = CalculateMoveSpeed();
    }
    public ref float GetMoveSpeed()
    {
        return ref _MoveSpeed;
    }
    public void AddMoveSeedIncrement(in float seedIncrement)
    {
        _MoveSeedIncrement += seedIncrement;
        _MoveSpeed = CalculateMoveSpeed();
    }
    public void RemoveMoveSeedIncrement(in float seedIncrement)
    {
        _MoveSeedIncrement -= seedIncrement;
        _MoveSpeed = CalculateMoveSpeed();
    }
    #endregion

    #region 旋转参数

    public void SetIsCanRotation(in bool isCanRotate)
    {
        _DisableRotationCount += isCanRotate ? -1 : 1;
    }

    public bool GetIsCanRotate()
    {
        return _DisableRotationCount == 0;
    }
    public void SetBaseRotationSpeed(in float rotationSpeed)
    {
        _BaseRotateSpeed = rotationSpeed;
        _RotateSpeed = CalculateRotateSpeed();
    }

    public ref readonly float GetRotationSpeed()
    {
        return ref _RotateSpeed;
    }

    private void AddRotateSpeedIncrement(in float speedIncrement)
    {
        _RotateSpeedIncrement += speedIncrement;
        _RotateSpeed = CalculateRotateSpeed();
    }

    private void RemoveRotateSpeedIncrement(in float speedIncrement)
    {
        _RotateSpeedIncrement -= speedIncrement;
        _RotateSpeed = CalculateRotateSpeed();
    }
    private float CalculateRotateSpeed()
    {
        var speed = Mathf.Clamp(1 + _RotateSpeedIncrement, 0, 5) * _BaseRotateSpeed;
        return speed;
    }

    #endregion
}