using System.Collections;
using System.Collections.Generic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using UnityEngine;

public interface IGemEffectTypeAnalysist
{
    public int GetIconAssetId(int goodsCfgId);
    public string GetName(int goodsCfgId);
    public string GetDescriptByGemId(int gemId, EnGemQuality quality);
    public string GetDescriptByGoodsCfgId(int goodsCfgId, EnGemQuality quality);
    public float CalculateEffectValue(int goodsCfgId, EnGemQuality quality);
    public void ApplyEntity(int entityId, int gemId);
}
public static class UIGemUtility
{

    private static readonly Dictionary<EnGemEffectTypeId, IGemEffectTypeAnalysist> _EffectAnalysists = new()
    {
        { EnGemEffectTypeId.Attribute, new GemEffectAttributeAnalysist()},
        { EnGemEffectTypeId.AttributePercentage, new GemEffectAttributePercentageAnalysist()},
    };

    private static IGemEffectTypeAnalysist GetAnalysist(EnGemEffectTypeId effectTypeId)
    {
        return _EffectAnalysists[effectTypeId];
    }
    public static string GetDescriptByGemId(int gemId)
    {
        var quality = GetGemQuality(gemId);
        var cfgId = UIGemUtility.GetGemGoodsCfgId(gemId);
        var goodsCfg = GameSchedule.Instance.GetGoodsCfg0(cfgId);
        var gemCfgId = goodsCfg.nLinkCfgId;
        var cfg = GameSchedule.Instance.GetGemCfg0(gemCfgId);
        var analysist = GetAnalysist((EnGemEffectTypeId)cfg.nEffectTypeId);
        var desc = analysist.GetDescriptByGemId(gemId, quality);
        return desc;
    }

    public static string GetDescriptByGemGoodsCfgId(int goodsCfgId, EnGemQuality quality)
    {
        var cfg = GameSchedule.Instance.GetGemCfg0(goodsCfgId);
        var analysist = GetAnalysist((EnGemEffectTypeId)cfg.nEffectTypeId);
        var desc = analysist.GetDescriptByGoodsCfgId(goodsCfgId, quality);
        return desc;
    }
    public static float CalculateEffectValue(int goodsCfgId, EnGemQuality quality)
    {
        var cfg = GameSchedule.Instance.GetGemCfg0(goodsCfgId);
        var analysist = GetAnalysist((EnGemEffectTypeId)cfg.nEffectTypeId);
        var desc = analysist.CalculateEffectValue(goodsCfgId, quality);
        return desc;
    }

    public static void ApplyEntity(int entityId, int gemId)
    {
        var goodsCfgId = GetGemGoodsCfgId(gemId);
        var cfg = GameSchedule.Instance.GetGemCfg0(goodsCfgId);
        var analysist = GetAnalysist((EnGemEffectTypeId)cfg.nEffectTypeId);
        analysist.ApplyEntity(entityId, gemId);
    }
    public static int GetIconAssetId(int gemId)
    {
        var cfgId = GetGemGoodsCfgId(gemId);
        var cfg = GameSchedule.Instance.GetGemCfg0(cfgId);
        var analysist = GetAnalysist((EnGemEffectTypeId)cfg.nEffectTypeId);
        var assetId = analysist.GetIconAssetId(cfgId);
        return assetId;
    }
    public static string GetCoreName(int gemCfgId)
    {
        var cfg = GameSchedule.Instance.GetGemCfg0(gemCfgId);
        var analysist = GetAnalysist((EnGemEffectTypeId)cfg.nEffectTypeId);
        var name = analysist.GetName(gemCfgId);
        return name;
    }

    public static int GetGemGoodsCfgId(int gemId)
    {
        var perData = PersistentMgr.Instance.GetData<PersistentGemData>(EnPersistentId.Gem);
        var goodsCfgId = perData.GetGoodsCfgId(gemId);
        return goodsCfgId;
    }
    public static float GetGemEffectValue(int gemId)
    {
        var perData = PersistentMgr.Instance.GetData<PersistentGemData>(EnPersistentId.Gem);
        var value = perData.GetEffectValue(gemId);
        return value;
    }
    public static EnGemQuality GetGemQuality(int gemId)
    {
        var perData = PersistentMgr.Instance.GetData<PersistentGemData>(EnPersistentId.Gem);
        var value = perData.GetQuality(gemId);
        return value;
    }
    public static EnGemTypeId GetGemType(int gemId)
    {
        var perData = PersistentMgr.Instance.GetData<PersistentGemData>(EnPersistentId.Gem);
        var value = perData.GetGemType(gemId);
        return value;
    }
    public static float GetGemQualityParam(int gemId, int index)
    {
        var quality = GetGemQuality(gemId);
        var cfgId = GetGemGoodsCfgId(gemId);
        var value = GetGemQualityParam(cfgId, quality, index);
        return value;
    }
    public static float GetGemQualityParam(int gemCfgId, EnGemQuality quality, int index)
    {
        var cgemCfg = GameSchedule.Instance.GetGemCfg0(gemCfgId);
        var param = quality switch
        {
            EnGemQuality.Quality1 => cgemCfg.arrEffectParams1,
            EnGemQuality.Quality2 => cgemCfg.arrEffectParams2,
            EnGemQuality.Quality3 => cgemCfg.arrEffectParams3,
            EnGemQuality.Quality4 => cgemCfg.arrEffectParams4,
            EnGemQuality.Quality5 => cgemCfg.arrEffectParams5,
            EnGemQuality.Quality6 => cgemCfg.arrEffectParams6,
            _ => null
        };
        var value = param[index];
        return value;
    }
    public static void SortList(ref List<int> coreIds)
    {
        coreIds.Sort((item1, item2) =>
        {
            var quality1 = UIGemUtility.GetGemQuality(item1);
            var quality2 = UIGemUtility.GetGemQuality(item2);
            if (quality1 > quality2)
                return -1;
            if (quality1 < quality2)
                return 1;
            var cfgId1 = UIGemUtility.GetGemGoodsCfgId(item1);
            var cfgId2 = UIGemUtility.GetGemGoodsCfgId(item2);
            if (cfgId1 < cfgId2)
                return -1;
            if (cfgId1 < cfgId2)
                return 1;
            return 0;
        });
    }
    public static GoodsInfo ClearGemNeedGoodsInfo()
    {
        return new((int)EnGoodsId.Diamond, 10);
    }
    public static List<GoodsInfo> RangeGetGemGoodsCfgId(int count)
    {
        var goodsCount = GameSchedule.Instance.GetGoodsCfgCount();
        var coreList = new List<int>(goodsCount);
        for (int i = 0; i < goodsCount; i++)
        {
            var goodsCfg = GameSchedule.Instance.GetGoodsCfg(i);
            if (goodsCfg.nGoodsTypeId != (int)EnGoodsTypeId.Gem)
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
