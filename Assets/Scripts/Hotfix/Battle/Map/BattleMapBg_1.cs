using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleMapBg_1Data : GameEntityData
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_BattleMapBg_1;

    public override void SetPosition(in Vector3 worldPos)
    {
        var depth = GameMapUtility.GetCurMapDepth();
        base.SetPosition(new Vector3(worldPos.x, worldPos.y, depth + (int)EnGameEntityOrderLayer.Background));
    }
}

public class BattleMapBg_1 : GameEntity<BattleMapBg_1Data>
{
    
}
