
using UnityEngine;

public class CoreEffectAttributeAnalysist : ICoreEffectAnalysist
{
    public string GetCoreName(int coreCfgId)
    {
        var coreCfg = GameSchedule.Instance.GetCoreCfg0(coreCfgId);
        var coreEffectCfg = GameSchedule.Instance.GetCoreEffectCfg0(coreCfg.nEffectId);
        var attId = UICoreUtility.GetCoreQualityParam(coreCfgId, EnCoreQuality.Quality1, 0);
        var attName = LanguageUtility.GetLanguage_AttiributeName((EnEntityAttributeId)attId);
        var nameStr = LanguageUtility.GetLanguage(coreEffectCfg.nNameLangId, attName);
        return nameStr;
    }
    public string GetDescriptsByCoreCfgId(int coreCfgId, EnCoreQuality quality)
    {
        var coreCfg = GameSchedule.Instance.GetCoreCfg0(coreCfgId);
        var coreEffectCfg = GameSchedule.Instance.GetCoreEffectCfg0(coreCfg.nEffectId);
        var attId = UICoreUtility.GetCoreQualityParam(coreCfgId, EnCoreQuality.Quality1, 0);
        var attName = LanguageUtility.GetLanguage_AttiributeName((EnEntityAttributeId)attId);
        if (quality > EnCoreQuality.None && quality < EnCoreQuality.EnumCount)
        {
            var (minValue, maxValue) = GetEffectValueRandom(coreCfgId, quality);
            var desc = LanguageUtility.GetLanguage(coreEffectCfg.nEffectDescLangId2, attName, minValue, maxValue);
            return desc;
        }
        else
        {
            var valueStr = LanguageUtility.GetLanguage_10035();
            var desc = LanguageUtility.GetLanguage(coreEffectCfg.nEffectDescLangId2, attName, valueStr, valueStr);
            return desc;
        }
    }

    public string GetDescriptsByCoreId(int coreId, EnCoreQuality quality)
    {
        var coreCfg = UICoreUtility.GetCoreCfg(coreId);
        var coreEffectCfg = GameSchedule.Instance.GetCoreEffectCfg0(coreCfg.nEffectId);
        var effectValue = UICoreMgr.Instance.GetCoreEffectValue(coreId);
        var attId = UICoreUtility.GetCoreQualityParam(coreId, 0);
        var attName = LanguageUtility.GetLanguage_AttiributeName((EnEntityAttributeId)attId);
        var (minValue, maxValue) = GetEffectValueRandom(coreCfg.nCoreId, quality);
        var desc = LanguageUtility.GetLanguage(coreEffectCfg.nEffectDescLangId, attName, effectValue, minValue, maxValue);
        return desc;
    }

    public virtual float CalculateRandomEffectValue(int coreCfgId, EnCoreQuality quality)
    {
        var coreCfg = GameSchedule.Instance.GetCoreCfg0(coreCfgId);
        var (minValue, maxValue) = GetEffectValueRandom(coreCfg.nCoreId, quality);
        var value = Mathf.RoundToInt(Random.Range(minValue, maxValue));
        return value;
    }

    public int GetIconAssetId(int coreCfgId)
    {
        var attId = (int)UICoreUtility.GetCoreQualityParam(coreCfgId, EnCoreQuality.Quality1, 0);
        var attCfg = GameSchedule.Instance.GetAttributeCfg0(attId);
        return attCfg.nIconAssetId;
    }
    public virtual void ApplyEntity(int entityId, int coreId)
    {
        var value = UICoreMgr.Instance.GetCoreEffectValue(coreId);
        var attId = UICoreUtility.GetCoreQualityParam(coreId, 0);
        GameAttributeUtility.AddAddtionalValue(entityId, (EnEntityAttributeId)attId, value);
        if (attId == (int)EnEntityAttributeId.LimitHp)
        {
            GameAttributeUtility.AddAddtionalValue(entityId, EnEntityAttributeId.Hp, value);
        }
    }
    protected (float minValue, float maxValue) GetEffectValueRandom(int coreCfgId, EnCoreQuality quality)
    {
        var minValue = UICoreUtility.GetCoreQualityParam(coreCfgId, quality, 1);
        var maxValue = UICoreUtility.GetCoreQualityParam(coreCfgId, quality, 2);
        return (minValue, maxValue);
    }
}