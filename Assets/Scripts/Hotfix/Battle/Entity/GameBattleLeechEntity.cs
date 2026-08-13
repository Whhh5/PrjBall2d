using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleLeechEntityData : SceneEntityData
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleLeechEntity;

    public override void SetPosition(in Vector3 worldPos)
    {
        var posZ = GameMapMgr.Instance.GetLayerDepth(EnGameEntityOrderLayer.Effect);
        base.SetPosition(new Vector3(worldPos.x, worldPos.y, posZ));
    }
}
public class GameBattleLeechEntity : SceneEntity, IEntityAnimCom
{
    private Animator _Animator = null;
    public Animator GetAnimator()
    {
        if (_Animator == null)
        {
            if (!gameObject.TryGetComponent<Animator>(out _Animator))
            {
                _Animator = gameObject.AddComponent<Animator>();
            }
        }
        return _Animator;
    }
}
