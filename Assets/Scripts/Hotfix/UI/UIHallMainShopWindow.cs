using System.Collections.Generic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using UnityEngine;

public class UIHallMainShopWindowData : UIWindowData
{
    public override EnUIWindowType WindowType => EnUIWindowType.SubWindow;

    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIHallMainShopWindow;
}
public class UIHallMainShopWindow : UIWindow
{
    [SerializeField, AutoReference]
    private RectTransform _BoxItemRootRect;


    private readonly List<UIHallMainShopWindow_BoxItem> _BoxItems = new();

    private int CoreLotteryNeedGoodsCfgId => GlobalCfgUtility.Get20121_Int32();
    private int CoreLottery1NeedGoodsCount => GlobalCfgUtility.Get20122_Int32();
    private int CoreLottery10NeedGoodsCount => GlobalCfgUtility.Get20123_Int32();

    private int GemLotteryNeedGoodsCfgId => GlobalCfgUtility.Get20221_Int32();
    private int GemLottery1NeedGoodsCount => GlobalCfgUtility.Get20222_Int32();
    private int GemLottery10NeedGoodsCount => GlobalCfgUtility.Get20223_Int32();


    private int CoinLotteryNeedGoodsCfgId => GlobalCfgUtility.Get20321_Int32();
    private int CoinLottery1NeedGoodsCount => GlobalCfgUtility.Get20322_Int32();
    private int CoinLottery10NeedGoodsCount => GlobalCfgUtility.Get20323_Int32();
    private int LotteryCoinMinCount => GlobalCfgUtility.Get20311_Int32();
    private int LotteryCoinMaxCount => GlobalCfgUtility.Get20312_Int32();


    private int EnergyLotteryNeedGoodsCfgId => GlobalCfgUtility.Get20421_Int32();
    private int EnergyLottery1NeedGoodsCount => GlobalCfgUtility.Get20422_Int32();
    private int EnergyLottery10NeedGoodsCount => GlobalCfgUtility.Get20423_Int32();
    private int LotteryEnergyMinCount => GlobalCfgUtility.Get20411_Int32();
    private int LotteryEnergyMaxCount => GlobalCfgUtility.Get20412_Int32();

    public override void OnUnload()
    {
        foreach (var item in _BoxItems)
            UIMgr.Instance.UnloadWindowItem(item);

        base.OnUnload();
        _BoxItems.Clear();
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();

        LoadBoxItems();
    }
    private async void LoadBoxItems()
    {
        {
            var (isCancel, item) = await UIMgr.Instance.LoadWindowItemAsync<UIHallMainShopWindow_BoxItem>(EnLoadTarget.Pre_UIHallMainShopWindow_BoxItem, _BoxItemRootRect, GetWindowToken())
                .SuppressCancellationThrow();
            if (isCancel)
                return;
            var title = LanguageUtility.GetLanguage(22011);
            var desc = LanguageUtility.GetLanguage(22012);
            item.Setting(title, desc, EnLoadTarget.Sp_ShangCheng_baoxiang1,
                new(CoreLotteryNeedGoodsCfgId, CoreLottery1NeedGoodsCount), OnClick_CoreLottery1,
                new(CoreLotteryNeedGoodsCfgId, CoreLottery10NeedGoodsCount), OnClick_CoreLottery10);
            _BoxItems.Add(item);
        }
        {
            var (isCancel2, item2) = await UIMgr.Instance.LoadWindowItemAsync<UIHallMainShopWindow_BoxItem>(EnLoadTarget.Pre_UIHallMainShopWindow_BoxItem, _BoxItemRootRect, GetWindowToken())
                .SuppressCancellationThrow();
            if (isCancel2)
                return;
            var title = LanguageUtility.GetLanguage(22021);
            var desc = LanguageUtility.GetLanguage(22022);
            item2.Setting(title, desc, EnLoadTarget.Sp_ShangCheng_baoxiang1,
                new(GemLotteryNeedGoodsCfgId, GemLottery1NeedGoodsCount), OnClick_GemLottery1,
                new(GemLotteryNeedGoodsCfgId, GemLottery10NeedGoodsCount), OnClick_GemLottery10);
            _BoxItems.Add(item2);
        }
        {
            var (isCancel3, item3) = await UIMgr.Instance.LoadWindowItemAsync<UIHallMainShopWindow_BoxItem>(EnLoadTarget.Pre_UIHallMainShopWindow_BoxItem, _BoxItemRootRect, GetWindowToken())
                .SuppressCancellationThrow();
            if (isCancel3)
                return;
            var title = LanguageUtility.GetLanguage(22031);
            var desc = string.Format(LanguageUtility.GetLanguage(22032), LotteryCoinMinCount, LotteryCoinMaxCount);
            item3.Setting(title, desc, EnLoadTarget.Sp_ShangCheng_baoxiang1,
                new(CoinLotteryNeedGoodsCfgId, CoinLottery1NeedGoodsCount), OnClick_CoinLottery1,
                new(CoinLotteryNeedGoodsCfgId, CoinLottery10NeedGoodsCount), OnClick_CoinLottery10);
            _BoxItems.Add(item3);
        }
        {
            var (isCancel4, item4) = await UIMgr.Instance.LoadWindowItemAsync<UIHallMainShopWindow_BoxItem>(EnLoadTarget.Pre_UIHallMainShopWindow_BoxItem, _BoxItemRootRect, GetWindowToken())
                .SuppressCancellationThrow();
            if (isCancel4)
                return;
            var title = LanguageUtility.GetLanguage(22041);
            var desc = string.Format(LanguageUtility.GetLanguage(22042), LotteryEnergyMinCount, LotteryEnergyMaxCount);
            item4.Setting(title, desc, EnLoadTarget.Sp_ShangCheng_baoxiang1,
                new(EnergyLotteryNeedGoodsCfgId, EnergyLottery1NeedGoodsCount), OnClick_EnergyLottery1,
                new(EnergyLotteryNeedGoodsCfgId, EnergyLottery10NeedGoodsCount), OnClick_EnergyLottery10);
            _BoxItems.Add(item4);
        }
    }
    private void OnClick_CoreLottery1()
    {
        UIGoodsMgr.Instance.RemoveGoods(CoreLotteryNeedGoodsCfgId, CoreLottery1NeedGoodsCount);

        var goodsInfoList = UICoreUtility.RangeGetCoreGoodsCfgId(1);
        UIRewardMgr.Instance.SendReward(goodsInfoList);
    }
    private void OnClick_CoreLottery10()
    {
        UIGoodsMgr.Instance.RemoveGoods(CoreLotteryNeedGoodsCfgId, CoreLottery10NeedGoodsCount);

        var goodsInfoList = UICoreUtility.RangeGetCoreGoodsCfgId(10);
        UIRewardMgr.Instance.SendReward(goodsInfoList);
    }
    private void OnClick_GemLottery1()
    {
        UIGoodsMgr.Instance.RemoveGoods(GemLotteryNeedGoodsCfgId, GemLottery1NeedGoodsCount);

        var goodsInfoList = UIGemUtility.RangeGetGemGoodsCfgId(1);
        UIRewardMgr.Instance.SendReward(goodsInfoList);
    }
    private void OnClick_GemLottery10()
    {
        UIGoodsMgr.Instance.RemoveGoods(GemLotteryNeedGoodsCfgId, GemLottery10NeedGoodsCount);

        var goodsInfoList = UIGemUtility.RangeGetGemGoodsCfgId(10);
        UIRewardMgr.Instance.SendReward(goodsInfoList);
    }
    private void OnClick_CoinLottery1()
    {
        UIGoodsMgr.Instance.RemoveGoods(CoinLotteryNeedGoodsCfgId, CoinLottery1NeedGoodsCount);

        var count = Random.Range(LotteryCoinMinCount, LotteryCoinMaxCount + 1);
        UIRewardMgr.Instance.SendReward((int)EnGoodsId.Coin, count);
    }
    private void OnClick_CoinLottery10()
    {
        UIGoodsMgr.Instance.RemoveGoods(CoinLotteryNeedGoodsCfgId, CoinLottery10NeedGoodsCount);

        var list = new List<GoodsInfo>();
        for (int i = 0; i < 10; i++)
        {
            var count = Random.Range(LotteryCoinMinCount, LotteryCoinMaxCount + 1);
            list.Add(new((int)EnGoodsId.Coin, count));
        }
        UIRewardMgr.Instance.SendReward(list);
    }
    private void OnClick_EnergyLottery1()
    {
        UIGoodsMgr.Instance.RemoveGoods(EnergyLotteryNeedGoodsCfgId, EnergyLottery1NeedGoodsCount);

        var count = Random.Range(LotteryEnergyMinCount, LotteryEnergyMaxCount + 1);
        UIRewardMgr.Instance.SendReward((int)EnGoodsId.Energy, count);
    }
    private void OnClick_EnergyLottery10()
    {
        UIGoodsMgr.Instance.RemoveGoods(EnergyLotteryNeedGoodsCfgId, EnergyLottery10NeedGoodsCount);

        var list = new List<GoodsInfo>();
        for (int i = 0; i < 10; i++)
        {
            var count = Random.Range(LotteryEnergyMinCount, LotteryEnergyMaxCount + 1);
            list.Add(new((int)EnGoodsId.Energy, count));
        }
        UIRewardMgr.Instance.SendReward(list);
    }
}