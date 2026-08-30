
using UnityEngine;

public class GemEffectAttributeAnalysist : IGemEffectTypeAnalysist
{

    public int GetIconAssetId(int goodsCfgId)
    {
        var goodsCfg = GameSchedule.Instance.GetGoodsCfg0(goodsCfgId);
        var gemCfg = GameSchedule.Instance.GetGemCfg0(goodsCfg.nLinkCfgId);
        var attId = (int)gemCfg.arrEffectParams[0];
        var attCfg = GameSchedule.Instance.GetAttributeCfg0(attId);
        return attCfg.nIconAssetId;
    }
    public string GetDescriptByGemId(int gemId, EnGemQuality quality)
    {
        var cfgId = UIGemUtility.GetGemGoodsCfgId(gemId);
        var cfg = GameSchedule.Instance.GetGemCfg0(cfgId);
        var effectTypeCfg = GameSchedule.Instance.GetGemEffectTypeCfg0(cfg.nEffectTypeId);

        var effectValue = UIGemUtility.GetGemEffectValue(gemId);
        var attId = (EnEntityAttributeId)cfg.arrEffectParams[0];
        var testVaue = cfg.arrEffectParams[1];
        var (min, max) = UIGemUtility.GetEffectValueRange(cfgId, quality, testVaue);
        var attName = LanguageUtility.GetLanguage_AttiributeName(attId);

        var str = LanguageUtility.GetLanguage(effectTypeCfg.nEffectDescLangId, attName, effectValue, min, max);
        return str;
    }

    public string GetDescriptByGoodsCfgId(int goodsCfgId, EnGemQuality quality)
    {
        var cfg = GameSchedule.Instance.GetGemCfg0(goodsCfgId);
        var effectTypeCfg = GameSchedule.Instance.GetGemEffectTypeCfg0(cfg.nEffectTypeId);

        var attId = (EnEntityAttributeId)cfg.arrEffectParams[0];
        var testVaue = cfg.arrEffectParams[1];
        var (min, max) = UIGemUtility.GetEffectValueRange(goodsCfgId, quality, testVaue);
        var attName = LanguageUtility.GetLanguage_AttiributeName(attId);

        var str = LanguageUtility.GetLanguage(effectTypeCfg.nEffectDescLangId, attName, min, max);
        return str;
    }

    public string GetName(int goodsCfgId)
    {
        var cfg = GameSchedule.Instance.GetGemCfg0(goodsCfgId);
        var effectTypeCfg = GameSchedule.Instance.GetGemEffectTypeCfg0(cfg.nEffectTypeId);

        var attId = (EnEntityAttributeId)cfg.arrEffectParams[0];
        var attName = LanguageUtility.GetLanguage_AttiributeName(attId);

        var str = LanguageUtility.GetLanguage(effectTypeCfg.nNameLangId, attName);
        return str;
    }
    public virtual void ApplyEntity(int entityId, int gemId)
    {
        var cfgId = UIGemUtility.GetGemGoodsCfgId(gemId);
        var cfg = GameSchedule.Instance.GetGemCfg0(cfgId);

        var effectValue = UIGemUtility.GetGemEffectValue(gemId);
        var attId = (EnEntityAttributeId)cfg.arrEffectParams[0];

        var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(entityId, EnSceneEntityComType.Attribute);
        attCom.AddAddtionalValue(attId, effectValue);
        if (attId == EnEntityAttributeId.LimitHp)
        {
            attCom.AddAddtionalValue(EnEntityAttributeId.Hp, effectValue);
        }
    }

    public int CalculateEffectValue(int goodsCfgId, EnGemQuality quality)
    {
        var goodsCfg = GameSchedule.Instance.GetGoodsCfg0(goodsCfgId);
        var gemCfgId = goodsCfg.nLinkCfgId;
        var gemCfg = GameSchedule.Instance.GetGemCfg0(gemCfgId);
        var testValue = gemCfg.arrEffectParams[1];
        var (min, max) = UIGemUtility.GetEffectValueRange(gemCfgId, quality, testValue);
        var value = Random.Range(min, max);
        return value;
    }
}