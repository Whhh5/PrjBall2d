using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameBattleEnergyEntityData : SceneEntityData
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleEnergyEntity;

    public override void SetPosition(in Vector3 worldPos)
    {
        var posZ = GameMapMgr.Instance.GetLayerDepth(EnGameEntityOrderLayer.EffectGround);
        base.SetPosition(new Vector3(worldPos.x, worldPos.y, posZ));
    }
}
public class GameBattleEnergyEntity : SceneEntity
{

}
