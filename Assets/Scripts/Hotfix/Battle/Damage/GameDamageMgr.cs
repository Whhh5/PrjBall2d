using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameDamageItemInfo : IClassPoolDestroy
{
    public float desTime;
    public int entityId;

    public void OnPoolDestroy()
    {
        desTime
            = entityId
            = -1;
    }
}

public class GameDamageMgr : Singleton<GameDamageMgr>
{
    public override EnManagerFuncType FuncType => base.FuncType | EnManagerFuncType.Update;
    private readonly List<GameDamageItemInfo> _DamageItemInfos = new(200);


    public void EntityReduceHealth(
        in int value,
        in Vector2 worldPos,
        in EnDamageType damageType,
        in EnDamageValueType valueType)
    {
        // 表现
        var item = ClassPoolMgr.Instance.Pull<GameDamageItemInfo>();
        item.desTime = GameUtility.GetGameTime() + 0.5f;
        var userData = ClassPoolMgr.Instance.Pull<GameBattleDamageValueUserData>();
        userData.Init(damageType, valueType, value);
        item.entityId = EntityMgr.Instance.CreateEntityData<GameBattleDamageValueData>(userData);
        ClassPoolMgr.Instance.Push(userData);
        _DamageItemInfos.Add(item);

        var posx = Random.Range(-0.1f, 0.1f);
        EntityMgr.Instance.SetEntityWorldPos(item.entityId, worldPos + new Vector2(posx, 0));
        EntityMgr.Instance.LoadEntity(item.entityId);
    }


    public override void Update()
    {
        base.Update();

        if (_DamageItemInfos.Count > 0)
        {
            var itemInfo = _DamageItemInfos[0];
            if (itemInfo.desTime < GameUtility.GetGameTime())
            {
                EntityMgr.Instance.RecycleEntityData(itemInfo.entityId);
                ClassPoolMgr.Instance.Push(itemInfo);
                _DamageItemInfos.RemoveAt(0);
            }
        }
    }
}
