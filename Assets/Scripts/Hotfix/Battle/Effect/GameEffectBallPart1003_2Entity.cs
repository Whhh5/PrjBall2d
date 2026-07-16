using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameEffectBallPart1003_2Entity : GameEffectEntity
{
    [SerializeField]
    private Transform _TopEffect;


    public override void SetPosition()
    {
        base.SetPosition();

        var curPos = _TopEffect.position;
        curPos.z = GameMapUtility.GetCurMapDepth() + (int)EnGameEntityOrderLayer.Effect;
        _TopEffect.position = curPos;
    }
}
