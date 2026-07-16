



public static class GameAttributeUtility
{
    public static void SetEntityAttributes(in int entityId, in int attValueCfgId)
    {
        var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(in entityId, EnSceneEntityComType.Attribute);
        var cfg = GameSchedule.Instance.GetAttributeValueCfg0(attValueCfgId);

        attCom.SetBaseValue(EnEntityAttributeId.Hp, cfg.nHp);
        attCom.SetBaseValue(EnEntityAttributeId.LimitHp, cfg.nLimitHp);
        attCom.SetBaseValue(EnEntityAttributeId.Attack, cfg.nAttack);
        attCom.SetBaseValue(EnEntityAttributeId.PhysicsArmor, cfg.nPhysicsArmor);
        attCom.SetBaseValue(EnEntityAttributeId.MagicArmor, cfg.nMagicArmor);
        attCom.SetBaseValue(EnEntityAttributeId.Critical, cfg.nCritical);
        attCom.SetBaseValue(EnEntityAttributeId.CriticalDamage, cfg.nCriticalDamage);
        attCom.SetBaseValue(EnEntityAttributeId.MoveSpeed, cfg.nMoveSpeed);

    }
}
