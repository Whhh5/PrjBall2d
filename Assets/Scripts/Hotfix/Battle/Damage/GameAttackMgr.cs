using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EnValueType
{
    None,
    Value,
    Percentage,
    Count,
}
public interface IGameEnityCallback
{

}
public interface IGameDamageCallback : IGameEnityCallback
{
    public void DamageCallback(int sourceEntityId, int targetEntityId, int damageValue, EnDamageType damageType, EnDamageValueType valueType, Vector2 worldPos);
}
public interface IGameRestoreHealthCallback : IGameEnityCallback
{
    public void RestoreHealthCallback(int sourceEntityId, int targetEntityId, float restoreHealthValue, Vector2 worldPos);
}
public struct AttackDamageResult
{
    public int value;
    public EnDamageValueType valueType;
    public EnDamageType damageType;

    public AttackDamageResult(int value, EnDamageValueType valueType, EnDamageType damageType)
    {
        this.value = value;
        this.valueType = valueType;
        this.damageType = damageType;
    }

    public override bool Equals(object obj)
    {
        return obj is AttackDamageResult result &&
               value == result.value &&
               valueType == result.valueType &&
               damageType == result.damageType;
    }

    public override int GetHashCode()
    {
        return System.HashCode.Combine(value, valueType, damageType);
    }

    public static bool operator ==(AttackDamageResult r1, AttackDamageResult r2)
    {
        return r1.value == r2.value && r1.valueType == r2.valueType && r1.damageType == r2.damageType;
    }
    public static bool operator !=(AttackDamageResult r1, AttackDamageResult r2)
    {
        return r1.value != r2.value && r1.valueType != r2.valueType && r1.damageType != r2.damageType;
    }
}
public class GameAttackMgr : Singleton<GameAttackMgr>
{

    public Dictionary<EnDamageType, Dictionary<int, HashSet<IGameEnityCallback>>> _DamageSourceCallback = new();
    public Dictionary<EnDamageType, Dictionary<int, HashSet<IGameEnityCallback>>> _DamageTargetCallback = new();
    public Dictionary<int, HashSet<IGameEnityCallback>> _RestoreHealthSourceCallback = new();
    public Dictionary<int, HashSet<IGameEnityCallback>> _RestoreHealthTargetCallback = new();

    #region 广播消息
    public void RegisterDamageSourceCallback(int entityId, EnDamageType damageType, IGameDamageCallback callback)
    {
        RegisterDamageCallback(ref _DamageSourceCallback, entityId, damageType, callback);
    }
    public void UnregisterDamageSourceCallback(int entityId, EnDamageType damageType, IGameDamageCallback callback)
    {
        UnregisterDamageCallback(ref _DamageSourceCallback, entityId, damageType, callback);
    }
    public void RegisterDamageTargetCallback(int entityId, EnDamageType damageType, IGameDamageCallback callback)
    {
        RegisterDamageCallback(ref _DamageTargetCallback, entityId, damageType, callback);
    }
    public void UnregisterDamageTargetCallback(int entityId, EnDamageType damageType, IGameDamageCallback callback)
    {
        UnregisterDamageCallback(ref _DamageTargetCallback, entityId, damageType, callback);
    }
    public static void RegisterDamageCallback(ref Dictionary<EnDamageType, Dictionary<int, HashSet<IGameEnityCallback>>> datas, int entityId, EnDamageType damageType, IGameEnityCallback callback)
    {
        if (!datas.TryGetValue(damageType, out var entityList))
        {
            entityList = new();
            datas.Add(damageType, entityList);
        }
        RegisterEntityCallback(ref entityList, entityId, callback);
    }
    public static void UnregisterDamageCallback(ref Dictionary<EnDamageType, Dictionary<int, HashSet<IGameEnityCallback>>> datas, int entityId, EnDamageType damageType, IGameEnityCallback callback)
    {
        if (!datas.TryGetValue(damageType, out var entityList))
            return;
        UnregisterEntityCallback(ref entityList, entityId, callback);
        if (entityList.Count == 0)
        {
            datas.Remove(damageType);
        }
    }
    public static void RegisterEntityCallback(ref Dictionary<int, HashSet<IGameEnityCallback>> datas, int entityId, IGameEnityCallback callback)
    {
        if (!datas.TryGetValue(entityId, out var list))
        {
            list = new();
            datas.Add(entityId, list);
        }
        list.Add(callback);
    }
    public static void UnregisterEntityCallback(ref Dictionary<int, HashSet<IGameEnityCallback>> datas, int entityId, IGameEnityCallback callback)
    {
        if (!datas.TryGetValue(entityId, out var list))
            return;
        if (!list.Remove(callback))
            return;
        if (list.Count > 0)
            return;
        datas.Remove(entityId);
    }

    public void RegisterRestoreHealthSourceCallback(int entityId, IGameRestoreHealthCallback callback)
    {
        RegisterEntityCallback(ref _RestoreHealthSourceCallback, entityId, callback);
    }
    public void UnregisterRestoreHealthSourceCallback(int entityId, IGameRestoreHealthCallback callback)
    {
        UnregisterEntityCallback(ref _RestoreHealthSourceCallback, entityId, callback);
    }
    public void RegisterRestoreHealthTargetCallback(int entityId, IGameRestoreHealthCallback callback)
    {
        RegisterEntityCallback(ref _RestoreHealthTargetCallback, entityId, callback);
    }
    public void UnregisterRestoreHealthTargetCallback(int entityId, IGameRestoreHealthCallback callback)
    {
        UnregisterEntityCallback(ref _RestoreHealthTargetCallback, entityId, callback);
    }
    #endregion

    public AttackDamageResult AttackMonster(
        in int sourceEntityId,
        in int targetEntityId,
        in int value,
        in EnDamageType damageType)
    {
        var pointInfo = EntityMgr.Instance.GetEntityData<GameBattleMonsterEntityData>(targetEntityId);
        var center = pointInfo.GetCenterPointWorldPos();
        var offset = new Vector2()
        {
            x = Random.Range(-0.3f, 0.3f),
            y = Random.Range(-0.3f, 0.3f),
        } * pointInfo.GetVolume();
        var result = AttackMonster(in sourceEntityId, in targetEntityId, in value, center + offset, in damageType);
        return result;
    }
    public AttackDamageResult AttackMonster(
        in int sourceEntityId,
        in int targetEntityId,
        in int value,
        in Vector2 worldPos,
        in EnDamageType damageType)
    {
        if (!GameMonsterUtility.EntityIsValid(in targetEntityId))
            return default;

        var targetProCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(targetEntityId, EnSceneEntityComType.Attribute);
        var damageResult = Attack(in sourceEntityId, in targetEntityId, in value, in worldPos, in damageType);

        if (damageResult == default)
            return default;

        var curHp = targetProCom.GetValueInt(EnEntityAttributeId.Hp);

        // 死亡
        if (curHp <= 0)
        {
            GameMonsterMgr.Instance.MonsterDie(targetEntityId);
        }
        return damageResult;
    }
    public AttackDamageResult AttackPlayer(
        in int sourceEntityId,
        in int targetEntityId,
        in int value,
        in Vector2 worldPos,
        in EnDamageType damageType)
    {
        if (!GamePlayerMgr.Instance.IsValid())
            return default;

        var targetProCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(targetEntityId, EnSceneEntityComType.Attribute);
        var damageResult = Attack(in sourceEntityId, in targetEntityId, in value, in worldPos, in damageType);
        GameEffectMgr.Instance.PlayEffectOnce(EnGameEffectId.Eff_Hero_Hit, worldPos);

        if (damageResult == default)
            return default;
        var curHp = targetProCom.GetValueInt(EnEntityAttributeId.Hp);

        // 死亡
        if (curHp <= 0)
        {
            //SceneEntityMgr.Instance.AddCmd(sourceEntityId, EnEntityCmd.PlayerDie);
            GamePlayerMgr.Instance.SetValid(false);

            EventMgr.Instance.FireExecute(EnABBEvent.EVENT_BATTLE_PLAYER_DIE, targetEntityId, 0);
        }
        return damageResult;
    }
    private AttackDamageResult Attack(
        in int sourceEntityId,
        in int targetEntityId,
        in int value,
        in Vector2 worldPos,
        in EnDamageType damageType)
    {
        var targetProCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(targetEntityId, EnSceneEntityComType.Attribute);
        var sourceProCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(sourceEntityId, EnSceneEntityComType.Attribute);
        // 计算伤害数值
        var finalDamage = value;
        var valueType = EnDamageValueType.Normal;
        switch (damageType)
        {
            case EnDamageType.Physic:
                {
                    var criticalValue = sourceProCom.GetValue(EnEntityAttributeId.Critical);
                    var testCriticalValue = Random.Range(0, 100);
                    if (testCriticalValue < criticalValue) // 暴击
                    {
                        var criticalDamageValue = sourceProCom.GetValue(EnEntityAttributeId.CriticalDamage);
                        finalDamage += Mathf.CeilToInt(finalDamage * criticalDamageValue / 100f);
                        valueType = EnDamageValueType.Critical;
                    }
                    var armorValue = targetProCom.GetValue(EnEntityAttributeId.PhysicsArmor);
                    var reduceValue = Mathf.Clamp01(armorValue / 100f);
                    finalDamage = Mathf.CeilToInt(finalDamage * (1 - reduceValue));
                }
                break;
            case EnDamageType.Magic:
                {
                    var magicCritical = sourceProCom.GetValue(EnEntityAttributeId.MagicCritical);
                    if (magicCritical > 0)
                    {
                        var criticalValue = sourceProCom.GetValue(EnEntityAttributeId.Critical);
                        var testCriticalValue = Random.Range(0, 100);
                        if (testCriticalValue < criticalValue) // 暴击
                        {
                            var criticalDamageValue = sourceProCom.GetValue(EnEntityAttributeId.CriticalDamage);
                            finalDamage += Mathf.CeilToInt(finalDamage * criticalDamageValue / 100f);
                            valueType = EnDamageValueType.Critical;
                        }
                    }
                    var armorValue = targetProCom.GetValue(EnEntityAttributeId.MagicArmor);
                    var reduceValue = Mathf.Clamp01(armorValue / 100f);
                    finalDamage = Mathf.CeilToInt(finalDamage * (1 - reduceValue));
                }
                break;
            case EnDamageType.True:
                break;
            default:
                break;
        }

        // 减少血量
        var maxHp = targetProCom.GetValue(EnEntityAttributeId.LimitHp);
        var curHp = targetProCom.GetValue(EnEntityAttributeId.Hp);
        var slider = Mathf.Clamp01((curHp - finalDamage) / maxHp);
        var monsterEntityData = EntityMgr.Instance.GetEntityData<IGameEntityHpChange>(targetEntityId);
        monsterEntityData.SetHpSliderValue(slider);
        // 数值表现
        GameDamageMgr.Instance.EntityReduceHealth(in finalDamage, in worldPos, in damageType, in valueType);
        // 数据变化
        GameAttributeUtility.RemoveResultValue(targetEntityId, EnEntityAttributeId.Hp, finalDamage);

        // 广播订阅
        if (_DamageSourceCallback.TryGetValue(damageType, out var entityList))
            if (entityList.TryGetValue(sourceEntityId, out var list))
            {
                var listCopy = list.ToList();
                for (int i = 0; i < listCopy.Count; i++)
                {
                    var callback = listCopy[i];
                    if (!list.Contains(callback))
                        continue;
                    (callback as IGameDamageCallback).DamageCallback(sourceEntityId, targetEntityId, finalDamage, damageType, valueType, worldPos);

                }
            }
        if (_DamageTargetCallback.TryGetValue(damageType, out var entityList2))
            if (entityList2.TryGetValue(targetEntityId, out var list))
            {
                var listCopy = list.ToList();
                for (int i = 0; i < listCopy.Count; i++)
                {
                    var callback = listCopy[i];
                    if (!list.Contains(callback))
                        continue;
                    (callback as IGameDamageCallback).DamageCallback(sourceEntityId, targetEntityId, finalDamage, damageType, valueType, worldPos);

                }
            }
        return new(finalDamage, valueType, damageType);
    }
    public void RestoreHealth(
        in int sourceEntityId,
        in int targetEntityId,
        in float value,
        in Vector2 worldPos)
    {
        // 设置数值
        var curValue = GameAttributeUtility.GetValue(targetEntityId, EnEntityAttributeId.Hp);
        var maxValue = GameAttributeUtility.GetValue(targetEntityId, EnEntityAttributeId.LimitHp);
        var addValue = Mathf.Min(value, maxValue - curValue);
        GameAttributeUtility.AddResultValue(targetEntityId, EnEntityAttributeId.Hp, addValue);
        // 更新显示
        var sliderValue = (curValue + addValue) / maxValue;
        var monsterEntityData = EntityMgr.Instance.GetEntityData<IGameEntityHpChange>(targetEntityId);
        monsterEntityData.SetHpSliderValue(Mathf.Clamp01(sliderValue));
        // 数值飘字

        // 广播消息
        if (_RestoreHealthSourceCallback.TryGetValue(sourceEntityId, out var callbacks))
            foreach (IGameRestoreHealthCallback callback in callbacks)
                callback.RestoreHealthCallback(sourceEntityId, targetEntityId, addValue, worldPos);
        if (_RestoreHealthTargetCallback.TryGetValue(sourceEntityId, out var callbacks2))
            foreach (IGameRestoreHealthCallback callback in callbacks2)
                callback.RestoreHealthCallback(sourceEntityId, targetEntityId, addValue, worldPos);

    }
}
