using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleMonster1000201EntityData : GameBattleMonsterEliteEntityData<GameBattleMonster1000201Entity>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleMonster1000201Entity;



    protected override Vector2 GetLocalCenterPointPos()
    {
        return new Vector2(0.5f, -0.5f);
    }

}
public class GameBattleMonster1000201Entity : GameBattleMonsterEliteEntity
{

}