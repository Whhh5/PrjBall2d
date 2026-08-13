using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleMonster1000102EntityData : GameBattleMonsterNormalEntityData<GameBattleMonster1000102Entity>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleMonster1000102Entity;


    protected override Vector2 GetLocalCenterPointPos()
    {
        return new Vector2(0, 0);
    }


}
public class GameBattleMonster1000102Entity : GameBattleMonsterNormalEntity
{

}