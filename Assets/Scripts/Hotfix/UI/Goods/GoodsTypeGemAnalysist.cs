


public class GoodsTypeGemAnalysist : IGoodsTypeAnalysist
{
    public int AddGoods(int goodsCfgId, int count)
    {
        var gemId = UIGemMgr.Instance.AddGem(goodsCfgId);
        return gemId;
    }

    public int GetGoodsCount(int goodsCfgId)
    {
        throw new System.NotImplementedException();
    }

    public string GetGoodsName(int goodsCfgId)
    {
        var name = UIGemUtility.GetCoreName(goodsCfgId);
        return name;
    }

    public void RemoveGoods(int goodsId, int count)
    {
        UIGemMgr.Instance.RemoveGem(goodsId);
    }
}