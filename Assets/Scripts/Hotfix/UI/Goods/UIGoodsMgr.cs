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
        { EnGoodsTypeId.Const, new GoodsTypeCurrentyAnalysist()},
        { EnGoodsTypeId.Core, new GoodsTypeCoreAnalysist()},
        { EnGoodsTypeId.Gem, new GoodsTypeGemAnalysist()},
    };
    private IGoodsTypeAnalysist GetAnalysist(EnGoodsTypeId typeId)
    {
        return _GoodsTypeAnalysists[typeId];
    }
    public void AddGoods(ref List<GoodsInfo> goodsInfos)
    {
        for (int i = 0; i < goodsInfos.Count; i++)
        {
            var item = goodsInfos[i];
            item.goodsId = AddGoods(item.goodsCfgId, item.goodsCount);
            goodsInfos[i] = item;
        }
    }
    public int AddGoods(EnGoodsId goodsCfgId, int count)
    {
        return AddGoods((int)goodsCfgId, count);
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


