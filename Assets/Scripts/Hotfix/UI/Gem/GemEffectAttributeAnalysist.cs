
using UnityEngine;

public class GemEffectAttributeAnalysist : IGemEffectTypeAnalysist
{
    protected (float min, float max) GetEffectValueRange(int gemCfgId, EnGemQuality quality)
    {
        var min = UIGemUtility.GetGemQualityParam(gemCfgId, quality, 1);
        var max = UIGemUtility.GetGemQualityParam(gemCfgId, quality, 2);
        return (min, max);
    }
    public int GetIconAssetId(int goodsCfgId)
    {
        var attId = (int)UIGemUtility.GetGemQualityParam(goodsCfgId, EnGemQuality.Quality1, 0);
        var attCfg = GameSchedule.Instance.GetAttributeCfg0(attId);
        return attCfg.nIconAssetId;
    }
    public string GetDescriptByGemId(int gemId, EnGemQuality quality)
    {
        var cfgId = UIGemUtility.GetGemGoodsCfgId(gemId);
        var cfg = GameSchedule.Instance.GetGemCfg0(cfgId);
        var effectTypeCfg = GameSchedule.Instance.GetGemEffectTypeCfg0(cfg.nEffectTypeId);

        var effectValue = UIGemUtility.GetGemEffectValue(gemId);
        var attId = (int)UIGemUtility.GetGemQualityParam(gemId, 0);
        var attName = LanguageUtility.GetLanguage_AttiributeName((EnEntityAttributeId)attId);

        var str = LanguageUtility.GetLanguage(effectTypeCfg.nEffectDescLangId, attName, effectValue);
        return str;
    }

    public string GetDescriptByGoodsCfgId(int goodsCfgId, EnGemQuality quality)
    {
        var cfg = GameSchedule.Instance.GetGemCfg0(goodsCfgId);
        var effectTypeCfg = GameSchedule.Instance.GetGemEffectTypeCfg0(cfg.nEffectTypeId);

        var attId = (EnEntityAttributeId)UIGemUtility.GetGemQualityParam(goodsCfgId, quality, 0);
        var attName = LanguageUtility.GetLanguage_AttiributeName(attId);

        var str = LanguageUtility.GetLanguage(effectTypeCfg.nEffectDescLangId, attName);
        return str;
    }

    public string GetName(int goodsCfgId)
    {
        var cfg = GameSchedule.Instance.GetGemCfg0(goodsCfgId);
        var effectTypeCfg = GameSchedule.Instance.GetGemEffectTypeCfg0(cfg.nEffectTypeId);

        var attId = (EnEntityAttributeId)UIGemUtility.GetGemQualityParam(goodsCfgId, EnGemQuality.Quality1, 0);
        var attName = LanguageUtility.GetLanguage_AttiributeName(attId);

        var str = LanguageUtility.GetLanguage(effectTypeCfg.nNameLangId, attName);
        return str;
    }
    public virtual void ApplyEntity(int entityId, int gemId)
    {
        var effectValue = UIGemUtility.GetGemEffectValue(gemId);
        var attId = (EnEntityAttributeId)UIGemUtility.GetGemQualityParam(gemId, 0);

        GameAttributeUtility.AddAddtionalValue(entityId, attId, effectValue);
        if (attId == EnEntityAttributeId.LimitHp)
        {
            GameAttributeUtility.AddAddtionalValue(entityId, EnEntityAttributeId.Hp, effectValue);
        }
    }

    public virtual float CalculateEffectValue(int goodsCfgId, EnGemQuality quality)
    {
        var goodsCfg = GameSchedule.Instance.GetGoodsCfg0(goodsCfgId);
        var gemCfgId = goodsCfg.nLinkCfgId;
        var (min, max) = GetEffectValueRange(gemCfgId, quality);
        var value = Mathf.RoundToInt(Random.Range(min, max));
        return value;
    }
}