using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreEffectReboundSpeedAnalysist : ICoreEffectAnalysist
{
    public void ApplyEntity(int entityId, int coreId)
    {

    }

    public int CalculateRandomEffectValue(int coreCfgId, EnCoreQuality quality)
    {
        var (minValue, maxValue) = GetEffectValueRandom(coreCfgId, quality);
        var value = Random.Range(minValue, maxValue + 1);
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
        return (int)EnLoadTarget.Sp_icon_attribute_1001;
    }
    private (int minValue, int maxValue) GetEffectValueRandom(int coreCfgId, EnCoreQuality quality)
    {
        var coreCfg = GameSchedule.Instance.GetCoreCfg0(coreCfgId);
        var testValue = coreCfg.arrEffectParams[0];
        var retult = UICoreUtility.GetEffectValueRange(coreCfgId, quality, testValue);
        return retult;
    }
}
