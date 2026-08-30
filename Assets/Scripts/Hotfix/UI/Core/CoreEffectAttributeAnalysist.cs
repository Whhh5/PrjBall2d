
using UnityEngine;

public class CoreEffectAttributeAnalysist : ICoreEffectAnalysist
{
    public string GetCoreName(int coreCfgId)
    {
        var coreCfg = GameSchedule.Instance.GetCoreCfg0(coreCfgId);
        var coreEffectCfg = GameSchedule.Instance.GetCoreEffectCfg0(coreCfg.nEffectId);
        var attId = coreCfg.arrEffectParams[0];
        var attName = LanguageUtility.GetLanguage_AttiributeName((EnEntityAttributeId)attId);
        var nameStr = LanguageUtility.GetLanguage(coreEffectCfg.nNameLangId, attName);
        return nameStr;
    }
    public string GetDescriptsByCoreCfgId(int coreCfgId, EnCoreQuality quality)
    {
        var coreCfg = GameSchedule.Instance.GetCoreCfg0(coreCfgId);
        var coreEffectCfg = GameSchedule.Instance.GetCoreEffectCfg0(coreCfg.nEffectId);
        var attId = coreCfg.arrEffectParams[0];
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
        var attId = coreCfg.arrEffectParams[0];
        var attName = LanguageUtility.GetLanguage_AttiributeName((EnEntityAttributeId)attId);
        var (minValue, maxValue) = GetEffectValueRandom(coreCfg.nCoreId, quality);
        var desc = LanguageUtility.GetLanguage(coreEffectCfg.nEffectDescLangId, attName, effectValue, minValue, maxValue);
        return desc;
    }

    public int CalculateRandomEffectValue(int coreCfgId, EnCoreQuality quality)
    {
        var coreCfg = GameSchedule.Instance.GetCoreCfg0(coreCfgId);
        var (minValue, maxValue) = GetEffectValueRandom(coreCfg.nCoreId, quality);
        var value = Random.Range(minValue, maxValue + 1);
        return value;
    }

    public int GetIconAssetId(int coreCfgId)
    {
        var coreCfg = GameSchedule.Instance.GetCoreCfg0(coreCfgId);
        var attId = (int)coreCfg.arrEffectParams[0];
        var attCfg = GameSchedule.Instance.GetAttributeCfg0(attId);
        return attCfg.nIconAssetId;
    }
    public virtual void ApplyEntity(int entityId, int coreId)
    {
        var attCom = SceneEntityMgr.Instance.GetEntityCom<SceneEntityAttributeComData>(entityId, EnSceneEntityComType.Attribute);
        var value = UICoreMgr.Instance.GetCoreEffectValue(coreId);
        var coreCfg = UICoreUtility.GetCoreCfg(coreId);
        var attId = (EnEntityAttributeId)coreCfg.arrEffectParams[0];
        attCom.AddAddtionalValue(attId, value);
        if (attId == EnEntityAttributeId.LimitHp)
        {
            attCom.AddAddtionalValue(EnEntityAttributeId.Hp, value);
        }
    }
    private (int minValue, int maxValue) GetEffectValueRandom(int coreCfgId, EnCoreQuality quality)
    {
        var coreCfg = GameSchedule.Instance.GetCoreCfg0(coreCfgId);
        var testValue = coreCfg.arrEffectParams[1];
        var result = UICoreUtility.GetEffectValueRange(coreCfgId, quality, testValue);
        return result;
    }
}