using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UICharacterUtility
{
    private readonly static List<EnEntityAttributeId> _AptitudeAttributeIds = new();
    public static int GetDefaultCharacterId()
    {
        var cfg = GameSchedule.Instance.GetChapterCfg(0);
        return cfg.nChapterId;
    }
    public static bool IsLock(int charaId)
    {
        var data = PersistentMgr.Instance.GetData<PersistentCharacterData>(EnPersistentId.Character);
        var isLock = data.IsLock(charaId);
        return isLock;
    }
    public static int GetLevel(int charaId)
    {
        var data = PersistentMgr.Instance.GetData<PersistentCharacterData>(EnPersistentId.Character);
        var isLock = data.GetLevel(charaId);
        return isLock;
    }
    public static int GetActiveCharacterId()
    {
        var data = PersistentMgr.Instance.GetData<PersistentCharacterData>(EnPersistentId.Character);
        var isLock = data.GetActiveCharacterId();
        return isLock;
    }
    public static int GetAptitudeValue(int charaId, EnEntityAttributeId attId)
    {
        var data = PersistentMgr.Instance.GetData<PersistentCharacterData>(EnPersistentId.Character);
        var value = data.GetAptitudeValue(charaId, attId);
        return value;
    }
    public static int GetMaxLevel(int charaId)
    {
        var characterCfg = GameSchedule.Instance.GetCharacterCfg0(charaId);
        return characterCfg.nMaxLevel;
    }
    public static List<AttributeInfo> GetAttributeValues(int charaId)
    {
        var level = GetLevel(charaId);
        var characterCfg = GameSchedule.Instance.GetCharacterCfg0(charaId);
        var attValueList = GameAttributeUtility.GetAttributeValueList(characterCfg.nBaseAttributeValueId);
        for (int i = 0; i < attValueList.Count; i++)
        {
            var value = attValueList[i];
            var attCfg = GameSchedule.Instance.GetAttributeCfg0((int)value.attId);
            if (attCfg.bAptitude > 0)
            {
                var aptitudeValue = GetAptitudeValue(charaId, value.attId);
                if (attCfg.bCofficient > 0)
                {
                    var add = Mathf.CeilToInt(value.value * level * characterCfg.fLevelCoefficient * aptitudeValue / 100f);
                    value.value += add;
                    attValueList[i] = value;
                }
            }
        }
        return attValueList;
    }
    public static List<EnEntityAttributeId> GetAptitudeAttributeIds()
    {
        if (_AptitudeAttributeIds.Count == 0)
        {
            var count = GameSchedule.Instance.GetAttributeCfgCount();

            for (int i = 0; i < count; i++)
            {
                var cfg = GameSchedule.Instance.GetAttributeCfg(i);
                if (cfg.bAptitude <= 0)
                    continue;
                _AptitudeAttributeIds.Add((EnEntityAttributeId)cfg.nAttributeId);
            }
        }
        return _AptitudeAttributeIds;
    }
    public static List<GoodsInfo> GetAptitudeNeedGoodsInfos(int characterId)
    {
        var characterCfg = GameSchedule.Instance.GetCharacterCfg0(characterId);
        var diamandCount = Mathf.CeilToInt(10 * (1 + characterCfg.fLevelCoefficient));
        return new List<GoodsInfo>()
        {
            new((int)EnGoodsId.Diamond, diamandCount),
        };
    }
    public static List<GoodsInfo> GetUplevelNeedGoodsInfos(int characterId)
    {
        var level = GetLevel(characterId);
        var goodsInfos = GetUplevelNeedGoodsInfos(characterId, level);
        return goodsInfos;
    }
    public static List<GoodsInfo> GetUplevelNeedGoodsInfos(int characterId, int level)
    {
        var testLevel = level + 1;
        var characterCfg = GameSchedule.Instance.GetCharacterCfg0(characterId);
        var coinCount = Mathf.CeilToInt(testLevel * (100 * (1 + characterCfg.fLevelCoefficient)));
        var goodsInfos = new List<GoodsInfo>() { new((int)EnGoodsId.Coin, coinCount) };
        if (testLevel > 10)
        {
            var diamandCount = Mathf.CeilToInt((testLevel - 10) * (1 + characterCfg.fLevelCoefficient));
            goodsInfos.Add(new ((int)EnGoodsId.Diamond, diamandCount));
        }
        return goodsInfos;
    }
}
