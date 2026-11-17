



using System.Collections.Generic;
using UnityEngine;

public struct AttributeInfo
{
    public EnEntityAttributeId attId;
    public float value;
    public AttributeInfo(EnEntityAttributeId attId, float value)
    {
        this.attId = attId;
        this.value = value;
    }
}
public static class GameAttributeUtility
{
    private static readonly List<AttributeInfo> _TempAttributeDatas = new(10);
    public static List<AttributeInfo> GetAttributeValueList(in int attValueCfgId)
    {
        _TempAttributeDatas.Clear();
        var cfg = GameSchedule.Instance.GetAttributeValueCfg0(attValueCfgId);
        //_TempAttributeDatas.Add(new(EnEntityAttributeId.Hp, cfg.nHp));
        _TempAttributeDatas.Add(new(EnEntityAttributeId.LimitHp, cfg.nLimitHp));
        _TempAttributeDatas.Add(new(EnEntityAttributeId.Attack, cfg.nAttack));
        _TempAttributeDatas.Add(new(EnEntityAttributeId.PhysicsArmor, cfg.nPhysicsArmor));
        _TempAttributeDatas.Add(new(EnEntityAttributeId.MagicArmor, cfg.nMagicArmor));
        _TempAttributeDatas.Add(new(EnEntityAttributeId.Critical, cfg.nCritical));
        _TempAttributeDatas.Add(new(EnEntityAttributeId.CriticalDamage, cfg.nCriticalDamage));
        _TempAttributeDatas.Add(new(EnEntityAttributeId.MoveSpeed, cfg.nMoveSpeed));
        return _TempAttributeDatas;
    }
    public static float AttributeChange_Cofficient(in EnValueType valueType, in float value, in float attCofficient)
    {
        var result = valueType switch
        {
            EnValueType.Value => Mathf.RoundToInt(value * attCofficient),
            _ => value * attCofficient,
        };
        return result;
    }
    public static void SetEntityAttributes(in int entityId, in int attValueCfgId, float attCofficient)
    {
        var dic = GetAttributeValueList(attValueCfgId);

        foreach (var info in dic)
        {
            var attCfg = GameSchedule.Instance.GetAttributeCfg0((int)info.attId);
            var value = attCfg.bCofficient > 0
                ? AttributeChange_Cofficient((EnValueType)attCfg.nValueType, info.value, attCofficient)
                : info.value;
            SetBaseValue(entityId, info.attId, value);

            if (info.attId == EnEntityAttributeId.LimitHp)
            {
                SetBaseValue(entityId, EnEntityAttributeId.Hp, value);
            }
        }
    }
    public static void SetEntityAttributes(in int entityId, in int attValueCfgId)
    {
        SetEntityAttributes(entityId, attValueCfgId, 1f);
    }
    public static void AddAddtionalValue(in int entityId, in EnEntityAttributeId attId, in float addValue)
    {
        var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(entityId, EnSceneEntityComType.Attribute);
        attCom.AddAddtionalValue(attId, addValue);

        FireChangeEvent(entityId, attId);
    }
    public static void RemoveAddtionalValue(in int entityId, in EnEntityAttributeId attId, in float addValue)
    {
        var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(entityId, EnSceneEntityComType.Attribute);
        attCom.RemoveAddtionalValue(attId, addValue);

        FireChangeEvent(entityId, attId);
    }
    public static void AddAddtionalPercentage(in int entityId, in EnEntityAttributeId attId, in float addValue)
    {
        var attCfg = GameSchedule.Instance.GetAttributeCfg0((int)attId);
        if (attCfg.nValueType == (int)EnValueType.Percentage)
        {
            AddAddtionalValue(in entityId, in attId, addValue * 100);
        }
        else
        {
            var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(entityId, EnSceneEntityComType.Attribute);
            attCom.AddAddtionalPercentage(attId, addValue);

            FireChangeEvent(entityId, attId);
        }
    }
    public static void RemoveAddtionalPercentage(in int entityId, in EnEntityAttributeId attId, in float addValue)
    {
        var attCfg = GameSchedule.Instance.GetAttributeCfg0((int)attId);
        if (attCfg.nValueType == (int)EnValueType.Percentage)
        {
            RemoveAddtionalValue(in entityId, in attId, addValue * 100);
        }
        else
        {
            var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(entityId, EnSceneEntityComType.Attribute);
            attCom.RemoveAddtionalPercentage(attId, addValue);

            FireChangeEvent(entityId, attId);
        }
    }
    public static void AddResultValue(in int entityId, in EnEntityAttributeId attId, in float addValue)
    {
        var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(entityId, EnSceneEntityComType.Attribute);
        attCom.AddResultValue(attId, addValue);

        FireChangeEvent(entityId, attId);
    }
    public static void RemoveResultValue(in int entityId, in EnEntityAttributeId attId, in float addValue)
    {
        var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(entityId, EnSceneEntityComType.Attribute);
        attCom.RemoveResultValue(attId, addValue);

        FireChangeEvent(entityId, attId);
    }
    public static void SetBaseValue(in int entityId, in EnEntityAttributeId attId, in float addValue)
    {
        var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(entityId, EnSceneEntityComType.Attribute);
        attCom.SetBaseValue(attId, addValue);

        FireChangeEvent(entityId, attId);
    }
    public static float GetValue(in int entityId, in EnEntityAttributeId attId)
    {
        var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(entityId, EnSceneEntityComType.Attribute);
        var value = attCom.GetValue(attId);
        return value;
    }
    public static int GetValueInt(in int entityId, in EnEntityAttributeId attId)
    {
        var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(entityId, EnSceneEntityComType.Attribute);
        var value = attCom.GetValueInt(attId);
        return value;
    }
    private static void FireChangeEvent(in int entityId, EnEntityAttributeId proId)
    {
        var evData = ClassPoolMgr.Instance.Pull<GameBattleEntityAttributeChangeEventData>();
        evData.currentValue = GetValue(entityId, proId);
        EventMgr.Instance.FireExecute(EnABBEvent.EVENT_BATTLE_ENTITY_ATTRIBUTE_CHANGE, entityId, (int)proId, evData);
        ClassPoolMgr.Instance.Push(evData);
    }


    public static int GetMiscIntValue(in int entityId, in EnMiscId id)
    {
        var attCom = SceneEntityMgr.Instance.GetEntityCom<EntityMiscComData>(entityId, EnSceneEntityComType.Misc);
        var value = attCom.GetIntValue(id);
        return value;
    }
    public static float GetMiscValue(in int entityId, in EnMiscId id)
    {
        var attCom = SceneEntityMgr.Instance.GetEntityCom<EntityMiscComData>(entityId, EnSceneEntityComType.Misc);
        var value = attCom.GetValue(id);
        return value;
    }
    public static void SetMiscValue(in int entityId, in EnMiscId id, float value)
    {
        var attCom = SceneEntityMgr.Instance.GetEntityCom<EntityMiscComData>(entityId, EnSceneEntityComType.Misc);
        attCom.SetValue(id, value);
    }
}
