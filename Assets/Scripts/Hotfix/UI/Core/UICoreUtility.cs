using System.Collections;
using System.Collections.Generic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using UnityEngine;

public static class UICoreUtility
{
    private static readonly Dictionary<EnCoreEffectType, ICoreEffectAnalysist> _EffectAnalysists = new()
    {
        { EnCoreEffectType.Attribute, new CoreEffectAttributeAnalysist()},
        { EnCoreEffectType.AttributePercentage, new CoreEffectAttributePercentageAnalysist()},
        { EnCoreEffectType.RebountSpeed, new CoreEffectReboundSpeedAnalysist()},
    };

    public static int GetWearUnitCount()
    {
        return 5;
    }
    public static int GetCoreIconAssetId(int coreCfgId)
    {
        var cfg = GameSchedule.Instance.GetCoreCfg0(coreCfgId);
        var effectType = (EnCoreEffectType)cfg.nEffectId;
        var analysis = _EffectAnalysists[effectType];
        var iconId = analysis.GetIconAssetId(coreCfgId);
        return iconId;
    }
    public static float GetRandomCoreEffectValue(int coreCfgId, EnCoreQuality quality)
    {
        var coreCfg = GameSchedule.Instance.GetCoreCfg0(coreCfgId);
        var effectType = (EnCoreEffectType)coreCfg.nEffectId;
        var analysis = _EffectAnalysists[effectType];
        var iconId = analysis.CalculateRandomEffectValue(coreCfgId, quality);
        return iconId;
    }
    public static string GetDescriptsByCoreCfgId(int coreCfgId, EnCoreQuality quality)
    {
        var coreCfg = GameSchedule.Instance.GetCoreCfg0(coreCfgId);
        var effectType = (EnCoreEffectType)coreCfg.nEffectId;
        var analysis = _EffectAnalysists[effectType];
        var iconId = analysis.GetDescriptsByCoreCfgId(coreCfgId, quality);
        return iconId;
    }
    public static string GetDescriptsByCoreId(int coreId)
    {
        var coreCfg = GetCoreCfg(coreId);
        var quality = GetCoreQuality(coreId);
        var effectType = (EnCoreEffectType)coreCfg.nEffectId;
        var analysis = _EffectAnalysists[effectType];
        var iconId = analysis.GetDescriptsByCoreId(coreId, quality);
        return iconId;
    }
    public static string GetCoreName(int coreCfgId)
    {
        var coreCfg = GameSchedule.Instance.GetCoreCfg0(coreCfgId);
        var effectType = (EnCoreEffectType)coreCfg.nEffectId;
        var analysis = _EffectAnalysists[effectType];
        var name = analysis.GetCoreName(coreCfgId);
        return name;
    }
    public static void ApplyEntity(int entityId, int coreId)
    {
        var coreCfg = GetCoreCfg(coreId);
        var effectType = (EnCoreEffectType)coreCfg.nEffectId;
        var analysis = _EffectAnalysists[effectType];
        analysis.ApplyEntity(entityId, coreId);
    }
    public static int GetGoodsCfgId(int coreId)
    {
        var perData = PersistentMgr.Instance.GetData<PersistentCoreData>(EnPersistentId.Core);
        var goodsCfgId = perData.GetCoreGoodsCfgId(coreId);
        return goodsCfgId;
    }
    public static EnCoreQuality GetCoreQuality(int coreId)
    {
        var perData = PersistentMgr.Instance.GetData<PersistentCoreData>(EnPersistentId.Core);
        var goodsCfgId = perData.GetQuality(coreId);
        return goodsCfgId;
    }
    public static bool ContainsGem(int coreId, int grooveIndex)
    {
        var perData = PersistentMgr.Instance.GetData<PersistentCoreData>(EnPersistentId.Core);
        var contains = perData.ContainsGem(coreId, grooveIndex);
        return contains;
    }
    public static int GetCoreGrooveGemId(int coreId, int grooveIndex)
    {
        var perData = PersistentMgr.Instance.GetData<PersistentCoreData>(EnPersistentId.Core);
        var gemId = perData.GetGemId(coreId, grooveIndex);
        return gemId;
    }
    public static EnGemTypeId GetCoreGrooveGemType(int coreId, int grooveIndex)
    {
        var perData = PersistentMgr.Instance.GetData<PersistentCoreData>(EnPersistentId.Core);
        var gemType = perData.GetGemType(coreId, grooveIndex);
        return gemType;
    }
    public static int GetCoreGemGrooveCount(int coreId)
    {
        var quality = GetCoreQuality(coreId);
        var gemGrooves = GetCoreGemGrooveCount(quality);
        return gemGrooves;
    }
    public static int GetCoreGemInlayCount(int coreId)
    {
        var quality = GetCoreQuality(coreId);
        var gemGrooves = GetCoreGemGrooveCount(quality);
        var count = 0;
        for (int i = 0; i < gemGrooves; i++)
        {
            if (ContainsGem(coreId, i))
                count++;
        }
        return count;
    }
    public static int GetCoreGemGrooveCount(EnCoreQuality quality)
    {
        var coreCfg = GameSchedule.Instance.GetCoreQualityCfg0((int)quality);
        return coreCfg.nGemGrooveCount;
    }
    public static int GetCoreCfgId(int coreId)
    {
        var goodsCfgId = GetGoodsCfgId(coreId);
        var goodsCfg = GameSchedule.Instance.GetGoodsCfg0(goodsCfgId);
        return goodsCfg.nLinkCfgId;
    }
    public static EnCoreType GetCoreType(int coreId)
    {
        var cfgId = GetCoreCfgId(coreId);
        var cfg = GameSchedule.Instance.GetCoreCfg0(cfgId);
        return (EnCoreType)cfg.nCoreType;
    }
    public static CoreCfg GetCoreCfg(int coreId)
    {
        var cfgId = GetCoreCfgId(coreId);
        var cfg = GameSchedule.Instance.GetCoreCfg0(cfgId);
        return cfg;
    }
    public static List<GoodsInfo> GetAptitudeNeedGoodsInfos(int coreId)
    {
        var quality = GetCoreQuality(coreId);
        var diamandCount = (int)quality;
        var goodsInfos = new List<GoodsInfo>() { new((int)EnGoodsId.Diamond, diamandCount) };
        return goodsInfos;
    }
    public static float GetCoreQualityParam(int coreId, int index)
    {
        var quality = GetCoreQuality(coreId);
        var cfgId = GetCoreCfgId(coreId);
        var value = GetCoreQualityParam(cfgId, quality, index);
        return value;
    }
    public static float GetCoreQualityParam(int coreCfgId, EnCoreQuality quality, int index)
    {
        var coreCfg = GameSchedule.Instance.GetCoreCfg0(coreCfgId);
        var param = quality switch
        {
            EnCoreQuality.Quality1 => coreCfg.arrEffectParams1,
            EnCoreQuality.Quality2 => coreCfg.arrEffectParams2,
            EnCoreQuality.Quality3 => coreCfg.arrEffectParams3,
            EnCoreQuality.Quality4 => coreCfg.arrEffectParams4,
            EnCoreQuality.Quality5 => coreCfg.arrEffectParams5,
            EnCoreQuality.Quality6 => coreCfg.arrEffectParams6,
            _ => null
        };
        var value = param[index];
        return value;
    }
    public static void SortList(ref List<int> coreIds)
    {
        coreIds.Sort((item1, item2) =>
        {
            var quality1 = UICoreUtility.GetCoreQuality(item1);
            var quality2 = UICoreUtility.GetCoreQuality(item2);
            if (quality1 > quality2)
                return -1;
            if (quality1 < quality2)
                return 1;
            //var cfgId1 = UICoreUtility.GetCoreCfgId(item1);
            //var cfgId2 = UICoreUtility.GetCoreCfgId(item2);
            if (item1 < item2)
                return -1;
            if (item1 > item2)
                return 1;
            return 0;
        });
    }
    public static List<GoodsInfo> RangeGetCoreGoodsCfgId(int count)
    {
        var goodsCount = GameSchedule.Instance.GetGoodsCfgCount();
        var coreList = new List<int>(goodsCount);
        for (int i = 0; i < goodsCount; i++)
        {
            var goodsCfg = GameSchedule.Instance.GetGoodsCfg(i);
            if (goodsCfg.nGoodsTypeId != (int)EnGoodsTypeId.Core)
                continue;
            coreList.Add(goodsCfg.nGoodsId);
        }
        var goodsInfoList = new List<GoodsInfo>(count);
        for (int i = 0; i < count; i++)
        {
            var index = Random.Range(0, coreList.Count);
            goodsInfoList.Add(new(coreList[index], 1));
        }
        return goodsInfoList;
    }
}