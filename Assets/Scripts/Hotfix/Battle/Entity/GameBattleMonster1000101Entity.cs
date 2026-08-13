using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameBattleMonster1000101EntityData : GameBattleMonsterNormalEntityData<GameBattleMonster1000101Entity>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameBattleMonster1000101Entity;

    public override void Destroy()
    {
        base.Destroy();
    }
    protected override void OnPoolInit(GameBattleMonsterEntityUserData userData)
    {
        base.OnPoolInit(userData);
    }

    public override void OnGOCreate(GameEntity entity)
    {
        base.OnGOCreate(entity);

    }
    protected override Vector2 GetLocalCenterPointPos()
    {
        return new Vector2(0, 0);
    }
}
public class GameBattleMonster1000101Entity : GameBattleMonsterNormalEntity
{
}
