using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameBalletBallPartPosEntityData : SceneEntityData<GameBalletBallPartPosEntity>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBalletBallPartPosEntity;

    public override void SetPosition(in Vector3 worldPos)
    {
        var posZ = GameMapMgr.Instance.GetLayerDepth(EnGameEntityOrderLayer.Grid);
        base.SetPosition(new Vector3(worldPos.x, worldPos.y, posZ));
    }
}
public class GameBalletBallPartPosEntity : SceneEntity
{
    
}
