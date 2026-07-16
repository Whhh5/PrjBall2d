using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleMonster1000104EntityData : GameBattleMonsterEntityData
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleMonster1000104Entity;



    protected override Vector2 GetLocalCenterPointPos()
    {
        return new Vector2(0, -0.5f);
    }

}
public class GameBattleMonster1000104Entity : GameBattleMonsterEntity<GameBattleMonster1000104EntityData>
{

}