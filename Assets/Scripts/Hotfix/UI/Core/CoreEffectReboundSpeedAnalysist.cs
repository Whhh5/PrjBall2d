using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreEffectReboundSpeedAnalysist : ICoreEffectAnalysist
{
    public void ApplyEntity(int entityId, int coreId)
    {
        var effectValue = UICoreMgr.Instance.GetCoreEffectValue(coreId);

        var curBallSpeed = GameAttributeUtility.GetMiscValue(entityId, EnMiscId.BallSpeedPercentage);
        GameAttributeUtility.SetMiscValue(entityId, EnMiscId.BallSpeedPercentage, curBallSpeed + effectValue);
    }

    public float CalculateRandomEffectValue(int coreCfgId, EnCoreQuality quality)
    {
        var (minValue, maxValue) = GetEffectValueRandom(coreCfgId, quality);
        var value = Mathf.Round(Random.Range(minValue, maxValue) * 100) / 100;
        return value;
    }

    public string GetCoreName(int coreCfgId)
    {
        var coreCfg = GameSchedule.Instance.GetCoreCfg0(coreCfgId);
        var coreEffectCfg = GameSchedule.Instance.GetCoreEffectCfg0(coreCfg.nEffectId);
        var nameStr = LanguageUtility.GetLanguage(coreEffectCfg.nNameLangId);
        return nameStr;
    }

    public string GetDescriptsByCoreCfgId(int coreCfgId, EnCoreQuality quality)
    {
        var (minValue, maxValue) = GetEffectValueRandom(coreCfgId, quality);
        var cfg = GameSchedule.Instance.GetCoreCfg0(coreCfgId);
        var effectCfg = GameSchedule.Instance.GetCoreEffectCfg0(cfg.nEffectId);
        var desc = LanguageUtility.GetLanguage(effectCfg.nEffectDescLangId2, minValue, maxValue);
        return desc;
    }

    public string GetDescriptsByCoreId(int coreId, EnCoreQuality quality)
    {
        var coreCfgId = UICoreUtility.GetCoreCfgId(coreId);
        var (minValue, maxValue) = GetEffectValueRandom(coreCfgId, quality);
        var curEffectValue = UICoreMgr.Instance.GetCoreEffectValue(coreId);
        var cfg = GameSchedule.Instance.GetCoreCfg0(coreCfgId);
        var effectCfg = GameSchedule.Instance.GetCoreEffectCfg0(cfg.nEffectId);
        var desc = LanguageUtility.GetLanguage(effectCfg.nEffectDescLangId, curEffectValue, minValue, maxValue);
        return desc;
    }

    public int GetIconAssetId(int coreCfgId)
    {
        return (int)EnLoadTarget.Sp_img_icon_attribute_extend1;
    }
    private (float minValue, float maxValue) GetEffectValueRandom(int coreCfgId, EnCoreQuality quality)
    {
        var minValue = UICoreUtility.GetCoreQualityParam(coreCfgId, quality, 0);
        var maxValue = UICoreUtility.GetCoreQualityParam(coreCfgId, quality, 1);
        return (minValue, maxValue);
    }
}
