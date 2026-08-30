



using System.Collections.Generic;

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
    public static void SetEntityAttributes(in int entityId, in int attValueCfgId)
    {
        var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(in entityId, EnSceneEntityComType.Attribute);

        var dic = GetAttributeValueList(attValueCfgId);

        foreach (var info in dic)
        {
            SetAttribute(attCom, info.attId, info.value);
            if (info.attId == EnEntityAttributeId.LimitHp)
            {
                SetAttribute(attCom, EnEntityAttributeId.Hp, info.value);
            }
        }
    }

    private static void SetAttribute(SceneEntityAttributeComData attCom, EnEntityAttributeId attId, float value)
    {
        attCom.SetBaseValue(attId, value);
    }

    public static void AddAddtionalValue(in int entityId, in EnEntityAttributeId attId, in float addValue)
    {
        var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(entityId, EnSceneEntityComType.Attribute);
        attCom.AddAddtionalValue(attId, addValue);
    }
    public static void RemoveAddtionalValue(in int entityId, in EnEntityAttributeId attId, in float addValue)
    {
        var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(entityId, EnSceneEntityComType.Attribute);
        attCom.RemoveAddtionalValue(attId, addValue);
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
        }
    }
    public static void AddResultValue(in int entityId, in EnEntityAttributeId attId, in float addValue)
    {
        var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(entityId, EnSceneEntityComType.Attribute);
        attCom.AddResultValue(attId, addValue);
    }
    public static void RemoveResultValue(in int entityId, in EnEntityAttributeId attId, in float addValue)
    {
        var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(entityId, EnSceneEntityComType.Attribute);
        attCom.RemoveResultValue(attId, addValue);
    }
    public static void SetBaseValue(in int entityId, in EnEntityAttributeId attId, in float addValue)
    {
        var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(entityId, EnSceneEntityComType.Attribute);
        attCom.SetBaseValue(attId, addValue);
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
}
