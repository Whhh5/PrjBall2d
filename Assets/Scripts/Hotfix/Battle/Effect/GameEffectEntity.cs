using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnGameEffectPos
{
    None,
    Top,
    Bottom,
}
public class GameEffectEntityData : EffectEntityData
{
    public override void SetPosition(in Vector3 worldPos)
    {
        var effectCfg = GameSchedule.Instance.GetEffectCfg0(_EffectID);
        var layer = (EnGameEffectPos)effectCfg.nEffectPos switch
        {
            EnGameEffectPos.Top => EnGameEntityOrderLayer.Effect,
            EnGameEffectPos.Bottom => EnGameEntityOrderLayer.EffectGround,
            _ => EnGameEntityOrderLayer.None
        };
        var posZ = GameMapUtility.GetCurMapDepth() + (int)layer;
        base.SetPosition(new Vector3(worldPos.x, worldPos.y, posZ));
    }
}
public class GameEffectEntity : EffectEntity
{

}
