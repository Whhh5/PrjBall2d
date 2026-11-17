using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class BattleMapBg_1Data : GameEntityData<BattleMapBg_1>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_BattleMapBg_1;

    public override void SetPosition(in Vector3 worldPos)
    {
        var depth = GameMapMgr.Instance.GetLayerDepth(EnGameEntityOrderLayer.Background);
        base.SetPosition(new Vector3(worldPos.x, worldPos.y, depth));
    }

}

public class BattleMapBg_1 : GameEntity
{
}
