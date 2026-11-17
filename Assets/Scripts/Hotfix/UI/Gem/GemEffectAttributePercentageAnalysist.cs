
using UnityEngine;

public class GemEffectAttributePercentageAnalysist : GemEffectAttributeAnalysist
{
    public new void ApplyEntity(int entityId, int gemId)
    {
        var effectValue = UIGemUtility.GetGemEffectValue(gemId);
        var attId = (EnEntityAttributeId)UIGemUtility.GetGemQualityParam(gemId, 0);

        GameAttributeUtility.AddAddtionalPercentage(entityId, attId, effectValue / 100f);
    }

    public override float CalculateEffectValue(int goodsCfgId, EnGemQuality quality)
    {
        var goodsCfg = GameSchedule.Instance.GetGoodsCfg0(goodsCfgId);
        var gemCfgId = goodsCfg.nLinkCfgId;
        var (min, max) = GetEffectValueRange(gemCfgId, quality);
        var value = Mathf.Round(Random.Range(min, max) * 100f) / 100f;
        return value;
    }
}