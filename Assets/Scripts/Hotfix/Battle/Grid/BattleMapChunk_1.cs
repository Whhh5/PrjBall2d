using UnityEngine;

public class BattleMapChunk_1Data : GameEntityData
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_BattleMapChunk_1;

    public override void SetPosition(in Vector3 worldPos)
    {
        var depth = GameMapUtility.GetCurMapDepth();
        base.SetPosition(new Vector3(worldPos.x, worldPos.y, depth + (int)EnGameEntityOrderLayer.Grid));
    }
}
public class BattleMapChunk_1 : GameEntity<BattleMapChunk_1Data>
{
    
}
