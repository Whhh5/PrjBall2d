using UnityEngine;

public enum EnEntityStatus
{
    None,
    Normal,
    Die,
}
public sealed class EntityLifeComData : Entity3DComData
{
    public override EnSceneEntityComType GetComType() => EnSceneEntityComType.Life;
    private int _CurHealthValue = -1;
    private int _MaxHealthValue = -1;

    private int _DefenseValue = -1;
    private int _MaxDefenseValue = 100;

    private EnEntityStatus _EntityStatus = EnEntityStatus.None;

    public void SetEntityStatus(EnEntityStatus status)
    {
        _EntityStatus = status;
    }
    public EnEntityStatus GetEntityStatus()
    {
        return _EntityStatus;
    }

    public int GetCurHealthValue()
    {
        return _CurHealthValue;
    }
    public void SetCurHealthValue(int value)
    {
        _CurHealthValue = Mathf.Clamp(value, 0, _MaxHealthValue);
    }
    public void AddHealthPoint(int value)
    {
        SetCurHealthValue(_CurHealthValue + value);
    }
    public void RemoveHealthPoint(int value)
    {
        SetCurHealthValue(_CurHealthValue + value);
    }
    public void SetMaxHealthValue(int value)
    {
        _MaxHealthValue = value;
    }
    public int GetMaxHealthValue()
    {
        return _MaxHealthValue;
    }
    public void SetDefenseValue(int value)
    {
        _DefenseValue = value;
    }
    public int GetDefenseValue()
    {
        return _DefenseValue;
    }
    public void SetMaxDefenseValue(int value)
    {
        _MaxDefenseValue = value;
    }
    public int GetMaxDefenseValue()
    {
        return _MaxDefenseValue;
    }

    public float GetHealthPointSchedule()
    {
        var schedule = (float)_CurHealthValue / _MaxHealthValue;
        return schedule;
    }
    public bool IsDie()
    {
        return GetEntityStatus() == EnEntityStatus.Die;
    }
}