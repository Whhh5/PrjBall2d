using Spine.Unity;
using UnityEngine;

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

    #region 方法重写
    public override void SetPosition(in Vector3 worldPos)
    {
        var mapId = GameMapMgr.Instance.GetCurMapId();
        var lbPos = GameMapUtility.GetGridLbPosition(mapId);
        var ruPos = GameMapUtility.GetGridRuPosition(mapId);
        var testValue = Mathf.Clamp((worldPos.y - lbPos.y) / (ruPos.y - lbPos.y), 0, 0.9f);
        var depth = GameMapUtility.GetMapDepth(mapId);
        var posOffset = depth + (int)EnGameEntityOrderLayer.Monster + testValue;
        base.SetPosition(new Vector3(worldPos.x, worldPos.y, posOffset));
    }
    #endregion

    public override void Destroy()
    {
        base.Destroy();
        _HpSliderValue = -1;
    }
    protected override void OnPoolInit(GameBattleMonsterEntityUserData userData)
    {
        SetGridIndex(userData.gridPos);
        SetHpSliderValue(0);
    }

    public override void OnGOCreate()
    {
        base.OnGOCreate();

        SceneMonsterEntity.SetHpSliderValue(_HpSliderValue);
    }
    #region entity 方法
    public void SetHpSliderValue(float value)
    {
        _HpSliderValue = value;
        if (IsLoadSuccess)
            SceneMonsterEntity.SetHpSliderValue(value);
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
}

public abstract class GameBattleMonsterEntity : SceneMonsterEntity<GameBattleMonsterEntityData>, IEntitySpineCom
{
    [SerializeField]
    private SkeletonAnimation _SkeletonAnimation = null;
    [SerializeField]
    private SpriteRenderer _HpSlider;

    [SerializeField]
    private float _HpMaxSliderValue;
    public SkeletonAnimation GetSkeletonAnimation()
    {
        return _SkeletonAnimation;
    }


    public void SetHpSliderValue(float value)
    {
        _HpSlider.size = new Vector2(_HpMaxSliderValue * value, _HpSlider.size.y);
    }
}


public abstract class GameBattleMonsterEntity<T> : GameBattleMonsterEntity
    where T : GameBattleMonsterEntityData
{

}
