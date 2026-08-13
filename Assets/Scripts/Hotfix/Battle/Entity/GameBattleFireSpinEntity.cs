using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleFireSpinEntityData : SceneEntityData<GameBattleFireSpinEntity>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleFireSpinEntity;

    public override void SetPosition(in Vector3 worldPos)
    {
        var posZ = GameMapMgr.Instance.GetLayerDepth(EnGameEntityOrderLayer.Effect);
        base.SetPosition(new Vector3(worldPos.x, worldPos.y, posZ));
    }

    public override void OnGOCreate(GameEntity entity)
    {
        base.OnGOCreate(entity);

        _SceneEntity.UpdateBottomEffectDepth();
    }
}

public class GameBattleFireSpinEntity : SceneEntity
{
    [SerializeField, AutoReference]
    private GameObject _BottomEffectGo = null;

    public void UpdateBottomEffectDepth()
    {
        var curPos = _BottomEffectGo.transform.position;
        curPos.z = GameMapMgr.Instance.GetLayerDepth(EnGameEntityOrderLayer.EffectGround);
        _BottomEffectGo.transform.position = curPos;
    }
}
