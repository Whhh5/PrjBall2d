using UnityEngine;
using UnityEngine.Rendering;

public interface IEntityGridPointInfo
{
    public Vector2 GetCenterPointWorldPos();
    public ref readonly GameBattleGridIndex GetMainGridIndex();
    public ref readonly int[] GetGridIndexs();
    public ref readonly int[] GetGridValidEdges();
    public Vector2 GetMainGridPos();
}
public class GameBattleMonsterEntityUserData : SceneMonsterEntityUserData
{
    public GameBattleGridIndex gridPos;
    public int energyCount;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        gridPos = default;
        energyCount = -1;
    }
}
public abstract class GameBattleMonsterEntityData<T> : GameBattleMonsterEntityData
    where T : GameBattleMonsterEntity
{
    protected T GameBattleMonsterEntity => _Entity as T;

}
public abstract class GameBattleMonsterEntityData : SceneMonsterEntityData<GameBattleMonsterEntity, GameBattleMonsterEntityUserData>, IGameEntityHpChange, IEntityGridPointInfo
{
    public GameBattleGridIndex _MainGridIndex;
    private float _HpSliderValue;
    private bool _IsDown;
    private int _EnergyCount;
    private float _MonsterUpdateTime;

    public override void SetPosition(in Vector3 worldPos)
    {
        ref readonly var lbPos = ref GameMapMgr.Instance.GetGridLbPosition();
        ref readonly var ruPos = ref GameMapMgr.Instance.GetGridRuPosition();
        var testValue = Mathf.Lerp(0, 0.9f, (worldPos.y - lbPos.y) / (ruPos.y - lbPos.y));
        var depth = GameMapMgr.Instance.GetLayerDepth(EnGameEntityOrderLayer.Monster, testValue);
        base.SetPosition(new Vector3(worldPos.x, worldPos.y, depth));
    }

    public override void Destroy()
    {
        base.Destroy();
        _HpSliderValue = -1;
        _EnergyCount = -1;
        _IsDown = false;
    }
    protected override void OnPoolInit(GameBattleMonsterEntityUserData userData)
    {
        _EnergyCount = userData.energyCount;
        SetGridIndex(userData.gridPos);
        SetHpSliderValue(0);
    }

    public override void OnGOCreate(GameEntity entity)
    {
        base.OnGOCreate(entity);

        _SceneMonsterEntity.SetHpSliderValue(_HpSliderValue);
    }
    #region entity 方法
    public void SetHpSliderValue(float value)
    {
        _HpSliderValue = value;
        if (IsLoadSuccess)
            _SceneMonsterEntity.SetHpSliderValue(value);
    }
    #endregion
    #region 排序 和 层
    public ref readonly GameBattleGridIndex GetMainGridIndex()
    {
        return ref _MainGridIndex;
    }
    public Vector2 GetMainGridPos()
    {
        return (Vector2)WorldPos;
    }
    public void SetGridIndex(GameBattleGridIndex gridIndex)
    {
        _MainGridIndex = gridIndex;
    }
    public ref readonly int[] GetGridIndexs()
    {
        var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(GetMonsterCfgId());
        return ref monsterCfg.arrGridLocalPos;
    }
    public ref readonly int[] GetGridValidEdges()
    {
        var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(GetMonsterCfgId());
        return ref monsterCfg.arrGridValidEdge;
    }
    #endregion
    #region 参考点
    protected abstract Vector2 GetLocalCenterPointPos();
    public Vector2 GetCenterPointWorldPos()
    {
        var result = (Vector2)GetWorldPos() + GetLocalCenterPointPos();
        return result;
    }
    #endregion
    public int GetEnergyCount()
    {
        return _EnergyCount;
    }

    public void SetIsDown(bool isDown)
    {
        _IsDown = isDown;
    }
    public bool GetIsDown()
    {
        return _IsDown;
    }
    public abstract void EnterMapDown();
    public abstract void UpdateMapDown();
    public abstract void ExitMapDown();
    public virtual void MonsterUpdate()
    {
        _MonsterUpdateTime += GameUtility.GetGameTimeDelta();
    }
    protected float GetLocalTime()
    {
        return _MonsterUpdateTime;
    }
    public virtual void ApplyData()
    {

    }
}

[RequireComponent(typeof(SortingGroup))]
public abstract class GameBattleMonsterEntity : SceneMonsterEntity
{
    public abstract void SetHpSliderValue(float value);
}