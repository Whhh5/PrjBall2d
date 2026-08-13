using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameEntityHpChange
{
    public void SetHpSliderValue(float sliderValue);
}
public class GameBattleCharacter1EntityData : SceneEntityData<GameBattleCharacter1Entity>, IGameEntityHpChange, IEntityGridPointInfo
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleCharacter1Entity;

    private float _SliderValue;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        _SliderValue = 0;
    }
    public override void SetPosition(in Vector3 worldPos)
    {
        var posZ = GameMapMgr.Instance.GetLayerDepth(EnGameEntityOrderLayer.Monster);
        base.SetPosition(new Vector3(worldPos.x, worldPos.y, posZ));
    }

    public override void OnGOCreate(GameEntity entity)
    {
        base.OnGOCreate(entity);

        _SceneEntity.SetHpSliderValue(_SliderValue);
    }
    public void SetHpSliderValue(float sliderValue)
    {
        _SliderValue = sliderValue;
        if (IsLoadSuccess)
            _SceneEntity.SetHpSliderValue(sliderValue);
    }

    public Vector2 GetCenterPointWorldPos()
    {
        return WorldPos;
    }

    public ref readonly GameBattleGridIndex GetMainGridIndex()
    {
        throw new System.NotImplementedException();
    }

    public ref readonly int[] GetGridIndexs()
    {
        throw new System.NotImplementedException();
    }

    public ref readonly int[] GetGridValidEdges()
    {
        throw new System.NotImplementedException();
    }

    public Vector2 GetMainGridPos()
    {
        throw new System.NotImplementedException();
    }
}
public class GameBattleCharacter1Entity : SceneEntity
{
    [SerializeField]
    private SpriteRenderer _HpSlider;

    [SerializeField]
    private float _HpMaxSliderValue;


    public void SetHpSliderValue(float value)
    {
        _HpSlider.size = new Vector2(_HpMaxSliderValue * value, _HpSlider.size.y);
    }
}
