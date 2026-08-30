
public class GoodsTypeCurrentyAnalysist : IGoodsTypeAnalysist
{

    public string GetGoodsName(int goodsCfgId)
    {
        var goodsCfg = GameSchedule.Instance.GetGoodsCfg0(goodsCfgId);
        var name = LanguageUtility.GetLanguage(goodsCfg.nNameLangId);
        return name;
    }
    public int AddGoods(int goodsCfgId, int count)
    {
        var goodsData = PersistentMgr.Instance.GetData<PersistentGoodsData>(EnPersistentId.Goods);
        var curCount = goodsData.GetGoodsCount(goodsCfgId);
        var toCount = curCount + count;
        goodsData.SetGoodsCount(goodsCfgId, toCount);

        var evUserData = ClassPoolMgr.Instance.Pull<GameGoodsChangeEventData>();
        evUserData.Setting(curCount, toCount);
        EventMgr.Instance.FireExecute(EnABBEvent.EVENT_GOODS_CHANGE, goodsCfgId, 0, evUserData);
        ClassPoolMgr.Instance.Push(evUserData);
        return goodsCfgId;
    }

    public string AddGoods(int goodsCfgId)
    {
        throw new System.NotImplementedException();
    }

    public int GetGoodsCount(int goodsCfgId)
    {
        var goodsData = PersistentMgr.Instance.GetData<PersistentGoodsData>(EnPersistentId.Goods);
        var count = goodsData.GetGoodsCount(goodsCfgId);
        return count;
    }

    public void RemoveGoods(int goodsId, int count)
    {
        var goodsData = PersistentMgr.Instance.GetData<PersistentGoodsData>(EnPersistentId.Goods);
        var curCount = goodsData.GetGoodsCount(goodsId);
        var toCount = curCount - count;
        goodsData.SetGoodsCount(goodsId, toCount);

        var evUserData = ClassPoolMgr.Instance.Pull<GameGoodsChangeEventData>();
        evUserData.Setting(curCount, toCount);
        EventMgr.Instance.FireExecute(EnABBEvent.EVENT_GOODS_CHANGE, goodsId, 0, evUserData);
        ClassPoolMgr.Instance.Push(evUserData);
    }
}