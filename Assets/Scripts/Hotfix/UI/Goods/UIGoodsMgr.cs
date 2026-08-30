using System.Collections.Generic;

public interface IGoodsTypeAnalysist
{
    public string GetGoodsName(int goodsCfgId);
    public int AddGoods(int goodsCfgId, int count);
    public void RemoveGoods(int goodsId, int count);
    public int GetGoodsCount(int goodsCfgId);
}
public class UIGoodsMgr : Singleton<UIGoodsMgr>
{
    private readonly Dictionary<EnGoodsTypeId, IGoodsTypeAnalysist> _GoodsTypeAnalysists = new()
    {
        { EnGoodsTypeId.Currenty, new GoodsTypeCurrentyAnalysist()},
        { EnGoodsTypeId.Consumables, new GoodsTypeConsumablesAnalysist()},
        { EnGoodsTypeId.Core, new GoodsTypeCoreAnalysist()},
        { EnGoodsTypeId.Gem, new GoodsTypeGemAnalysist()},
    };
    private IGoodsTypeAnalysist GetAnalysist(EnGoodsTypeId typeId)
    {
        return _GoodsTypeAnalysists[typeId];
    }
    public List<int> AddGoods(List<GoodsInfo> goodsInfos)
    {
        var ids = new List<int>(goodsInfos.Count);
        foreach (var item in goodsInfos)
        {
            var goodsId = AddGoods(item.goodsCfgId, item.goodsCount);
            ids.Add(goodsId);
        }
        return ids;
    }
    public int AddGoods(int goodsCfgId, int count)
    {
        var goodsCfg = GameSchedule.Instance.GetGoodsCfg0(goodsCfgId);
        var analysist = GetAnalysist((EnGoodsTypeId)goodsCfg.nGoodsTypeId);
        var goodsId = analysist.AddGoods(goodsCfgId, count);
        return goodsId;
    }
    public void RemoveGoods(int goodsCfgId, int count)
    {
        var goodsCfg = GameSchedule.Instance.GetGoodsCfg0(goodsCfgId);
        var analysist = GetAnalysist((EnGoodsTypeId)goodsCfg.nGoodsTypeId);
        analysist.RemoveGoods(goodsCfgId, count);
    }
    public int GetGoodsCount(int goodsCfgId)
    {
        var goodsCfg = GameSchedule.Instance.GetGoodsCfg0(goodsCfgId);
        var analysist = GetAnalysist((EnGoodsTypeId)goodsCfg.nGoodsTypeId);
        var count = analysist.GetGoodsCount(goodsCfgId);
        return count;
    }
    public string GetGoodsName(int goodsCfgId)
    {
        var goodsCfg = GameSchedule.Instance.GetGoodsCfg0(goodsCfgId);
        var analysist = GetAnalysist((EnGoodsTypeId)goodsCfg.nGoodsTypeId);
        var name = analysist.GetGoodsName(goodsCfgId);
        return name;
    }
}


