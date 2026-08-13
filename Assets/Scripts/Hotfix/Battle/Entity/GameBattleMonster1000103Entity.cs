using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleMonster1000103EntityData : GameBattleMonsterNormalEntityData<GameBattleMonster1000103Entity>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleMonster1000103Entity;



    protected override Vector2 GetLocalCenterPointPos()
    {
        return new Vector2(0.5f, 0);
    }


}
public class GameBattleMonster1000103Entity : GameBattleMonsterNormalEntity
{

}