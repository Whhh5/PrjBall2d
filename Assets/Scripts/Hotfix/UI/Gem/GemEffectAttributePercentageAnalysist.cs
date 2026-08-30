
public class GemEffectAttributePercentageAnalysist : GemEffectAttributeAnalysist
{
    public new void ApplyEntity(int entityId, int gemId)
    {
        var cfgId = UIGemUtility.GetGemGoodsCfgId(gemId);
        var cfg = GameSchedule.Instance.GetGemCfg0(cfgId);

        var effectValue = UIGemUtility.GetGemEffectValue(gemId);
        var attId = (EnEntityAttributeId)cfg.arrEffectParams[0];

        var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(entityId, EnSceneEntityComType.Attribute);
        attCom.AddAddtionalPercentage(attId, effectValue / 100f);
    }
}