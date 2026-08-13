using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameEffectBallPart1003_2Entity : GameEffectEntity
{
    [SerializeField]
    private Transform _TopEffect;


    public override void SetPosition(in Vector3 worldPos)
    {
        base.SetPosition(in worldPos);

        var curPos = _TopEffect.position;
        curPos.z = GameMapMgr.Instance.GetLayerDepth(EnGameEntityOrderLayer.Effect);
        _TopEffect.position = curPos;
    }
}
