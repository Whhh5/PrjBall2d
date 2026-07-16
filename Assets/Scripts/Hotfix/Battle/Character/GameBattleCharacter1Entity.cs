using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameEntityHpChange
{
    public void SetHpSliderValue(float sliderValue);
}
public class GameBattleCharacter1EntityData : SceneEntityData, IGameEntityHpChange, IEntityGridPointInfo
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleCharacter1Entity;

    public override void SetPosition(in Vector3 worldPos)
    {
        var posZ = GameMapUtility.GetCurMapDepth() + (int)EnGameEntityOrderLayer.Monster;
        base.SetPosition(new Vector3(worldPos.x, worldPos.y, posZ));
    }
    public void SetHpSliderValue(float sliderValue)
    {
        
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
    
}
