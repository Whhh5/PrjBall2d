



using System.Runtime.InteropServices.ComTypes;

public static class GameAttributeUtility
{
    public static void SetEntityAttributes(in int entityId, in int attValueCfgId)
    {
        var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(in entityId, EnSceneEntityComType.Attribute);
        var cfg = GameSchedule.Instance.GetAttributeValueCfg0(attValueCfgId);

        SetAttribute(attCom, EnEntityAttributeId.Hp, cfg.nHp);
        SetAttribute(attCom, EnEntityAttributeId.LimitHp, cfg.nLimitHp);
        SetAttribute(attCom, EnEntityAttributeId.Attack, cfg.nAttack);
        SetAttribute(attCom, EnEntityAttributeId.PhysicsArmor, cfg.nPhysicsArmor);
        SetAttribute(attCom, EnEntityAttributeId.MagicArmor, cfg.nMagicArmor);
        SetAttribute(attCom, EnEntityAttributeId.Critical, cfg.nCritical);
        SetAttribute(attCom, EnEntityAttributeId.CriticalDamage, cfg.nCriticalDamage);
        SetAttribute(attCom, EnEntityAttributeId.MoveSpeed, cfg.nMoveSpeed);

    }

    private static void SetAttribute(SceneEntityAttributeComData attCom, EnEntityAttributeId attId, float value)
    {
        var attCfg = GameSchedule.Instance.GetAttributeCfg0((int)attId);
        if (attCfg.nValueType == (int)EnValueType.Percentage)
        {
            attCom.SetBaseValue(attId, 1);
            attCom.RemoveAddtionalPercentage(attId, 1);
            attCom.AddAddtionalPercentage(attId, value);
        }
        else
        {
            attCom.SetBaseValue(attId, value);
        }
    }

    public static void AddAddtionalValue(in int entityId, in EnEntityAttributeId attId, in float addValue)
    {
        var attCfg = GameSchedule.Instance.GetAttributeCfg0((int)attId);
        if (attCfg.nValueType == (int)EnValueType.Percentage)
        {
            var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(entityId, EnSceneEntityComType.Attribute);
            attCom.AddAddtionalPercentage(attId, addValue);
        }
        else
        {
            var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(entityId, EnSceneEntityComType.Attribute);
            attCom.AddAddtionalValue(attId, addValue);
        }
    }
    public static void RemoveAddtionalValue(in int entityId, in EnEntityAttributeId attId, in float addValue)
    {
        var attCfg = GameSchedule.Instance.GetAttributeCfg0((int)attId);
        if (attCfg.nValueType == (int)EnValueType.Percentage)
        {
            var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(entityId, EnSceneEntityComType.Attribute);
            attCom.RemoveAddtionalPercentage(attId, addValue);
        }
        else
        {
            var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(entityId, EnSceneEntityComType.Attribute);
            attCom.RemoveAddtionalValue(attId, addValue);
        }
    }
    public static void AddAddtionalPercentage(in int entityId, in EnEntityAttributeId attId, in float addValue)
    {
        var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(entityId, EnSceneEntityComType.Attribute);
        attCom.AddAddtionalPercentage(attId, addValue);
    }
    public static void RemoveAddtionalPercentage(in int entityId, in EnEntityAttributeId attId, in float addValue)
    {
        var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(entityId, EnSceneEntityComType.Attribute);
        attCom.RemoveAddtionalPercentage(attId, addValue);
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
}
