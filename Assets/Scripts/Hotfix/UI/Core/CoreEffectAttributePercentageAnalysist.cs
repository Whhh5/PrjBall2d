

using UnityEngine;

public class CoreEffectAttributePercentageAnalysist : CoreEffectAttributeAnalysist
{
    public override void ApplyEntity(int entityId, int coreId)
    {
        var value = UICoreMgr.Instance.GetCoreEffectValue(coreId);
        var attId = (EnEntityAttributeId)UICoreUtility.GetCoreQualityParam(coreId, 0);
        GameAttributeUtility.AddAddtionalPercentage(entityId, attId, value / 100f);
    }
    public override float CalculateRandomEffectValue(int coreCfgId, EnCoreQuality quality)
    {
        var coreCfg = GameSchedule.Instance.GetCoreCfg0(coreCfgId);
        var (minValue, maxValue) = GetEffectValueRandom(coreCfg.nCoreId, quality);
        var value = Mathf.Round(Random.Range(minValue, maxValue) * 100f) / 100f;
        return value;
    }
}