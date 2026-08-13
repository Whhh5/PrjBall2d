using UnityEngine;

public class BattleMapChunk_1Data : GameEntityData<BattleMapChunk_1>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_BattleMapChunk_1;

    public override void SetPosition(in Vector3 worldPos)
    {
        var depth = GameMapMgr.Instance.GetLayerDepth(EnGameEntityOrderLayer.Grid);
        base.SetPosition(new Vector3(worldPos.x, worldPos.y, depth));
    }
}
public class BattleMapChunk_1 : GameEntity
{
}
