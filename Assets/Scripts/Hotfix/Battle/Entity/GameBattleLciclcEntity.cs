using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameBattleLciclcEntityData : SceneEntityData
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleLciclcEntity;


    public override void SetPosition(in Vector3 worldPos)
    {
        var posZ = GameMapMgr.Instance.GetLayerDepth(EnGameEntityOrderLayer.Effect);
        base.SetPosition(new Vector3(worldPos.x, worldPos.y, posZ));
    }
}
public class GameBattleLciclcEntity : SceneEntity
{

}
