


public class GoodsTypeCoreAnalysist : IGoodsTypeAnalysist
{
    public int AddGoods(int goodsCfgId, int count)
    {
        var coreId = UICoreMgr.Instance.AddCore(goodsCfgId);
        return coreId;
    }

    public int GetGoodsCount(int goodsCfgId)
    {
        throw new System.NotImplementedException();
    }

    public string GetGoodsName(int goodsCfgId)
    {
        var goodsCfg = GameSchedule.Instance.GetGoodsCfg0(goodsCfgId);
        var name = UICoreUtility.GetCoreName(goodsCfg.nLinkCfgId);
        return name;
    }

    public void RemoveGoods(int goodsId, int count)
    {
        UICoreMgr.Instance.RemoveCore(goodsId);
    }
}