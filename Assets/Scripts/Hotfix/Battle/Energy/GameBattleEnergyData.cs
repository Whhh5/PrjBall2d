using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleEnergyData : IClassPoolDestroy
{
    public Vector2 startPos;
    public Vector2 targetPos;
    public float createTime;
    public float delayTime;
    public float moveTime;
    public int energyValue;
    public void OnPoolDestroy()
    {
        startPos = default;
        targetPos = default;
        createTime
            = moveTime
            = delayTime
            = energyValue
            = -1;
    }
}
    