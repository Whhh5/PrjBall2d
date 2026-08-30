


public struct GoodsInfo
{
    public int goodsCfgId;
    public int goodsCount;
    public int goodsId;
    public GoodsInfo(int goodsCfgId, int goodsCount, int goodsId)
    {
        this.goodsCfgId = goodsCfgId;
        this.goodsCount = goodsCount;
        this.goodsId = goodsId;
    }
    public GoodsInfo(int goodsCfgId, int goodsCount)
    {
        this.goodsCfgId = goodsCfgId;
        this.goodsCount = goodsCount;
        this.goodsId = goodsCfgId;
    }
    public static implicit operator GoodsKey(GoodsInfo goodsInfo)
    {
        return goodsInfo.AsGoodsKey();
    }
    public readonly GoodsKey AsGoodsKey()
    {
        return new(goodsCfgId, goodsId);
    }
}