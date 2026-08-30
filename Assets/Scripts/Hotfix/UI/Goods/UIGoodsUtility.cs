using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public static class UIGoodsUtility
{
    private static readonly Dictionary<EnGoodsTypeId, IGoodsTypeAnalysist> _GoodsTypeAnalysists = new()
    {
        {EnGoodsTypeId.Consumables, new  GoodsTypeConsumablesAnalysist()},
        {EnGoodsTypeId.Currenty, new  GoodsTypeConsumablesAnalysist()},
    };
    public static void DestroyGoodsItemIcon(UIGoodsItemIconBase iconItem)
    {
        UIMgr.Instance.UnloadWindowItem(iconItem);
    }
    public static async UniTask<T> CreateGoodsItemIcon<T>(EnLoadTarget loadTarget, RectTransform parent, CancellationToken token)
        where T : UIGoodsItemIconBase
    {
        var (isCancel, item) = await UIMgr.Instance.LoadWindowItemAsync<UIGoodsItemIconBase>(loadTarget, parent, token)
            .SuppressCancellationThrow();
        if (isCancel)
            throw new System.OperationCanceledException();
        item.SetAutoSize();
        return item as T;
    }
    public static async UniTask<UIGoodsItemIconBase> CreateGoodsItemIcon(int goodsCfgId, RectTransform parent, CancellationToken token)
    {
        var goodsCfg = GameSchedule.Instance.GetGoodsCfg0(goodsCfgId);
        var goodsTypeCfg = GameSchedule.Instance.GetGoodsTypeCfg0(goodsCfg.nGoodsTypeId);
        var (isCancel, item) = await UIMgr.Instance.LoadWindowItemAsync<UIGoodsItemIconBase>((EnLoadTarget)goodsTypeCfg.nItemIconAssetId, parent, token)
            .SuppressCancellationThrow();
        if (isCancel)
            throw new System.OperationCanceledException();
        item.SetAutoSize();
        return item;
    }
    public static async UniTask<T> CreateGoodsItemIcon<T>(int goodsCfgId, RectTransform parent, CancellationToken token)
        where T : UIGoodsItemIconBase
    {
        var (isCancel, item) = await CreateGoodsItemIcon(goodsCfgId, parent, token)
            .SuppressCancellationThrow();
        if (isCancel)
            throw new System.OperationCanceledException();
        return item as T;
    }

    public static void DefaultOnClick(in GoodsKey goodsKey, EnSourceId sourceId)
    {
        var goodsCfg = GameSchedule.Instance.GetGoodsCfg0(goodsKey.goodsCfgId);
        switch ((EnGoodsTypeId)goodsCfg.nGoodsTypeId)
        {
            case EnGoodsTypeId.Core:
                {
                    var windwUserData = ClassPoolMgr.Instance.Pull<UICoreInfoDialogUserData>();
                    if (goodsKey.goodsId > 0)
                        windwUserData.SettingCoreId(goodsKey.goodsId, sourceId);
                    else
                        windwUserData.SettingCoreGoodsCfgId(goodsKey.goodsCfgId, EnCoreQuality.None, sourceId);
                    UIMgr.Instance.ShowWindow<UICoreInfoDialogData>(windwUserData);
                    ClassPoolMgr.Instance.Push(windwUserData);
                }
                break;
            case EnGoodsTypeId.Gem:
                {
                    var windwUserData = ClassPoolMgr.Instance.Pull<UIGemInfoDialogUserData>();
                    if (goodsKey.goodsId > 0)
                        windwUserData.SettingGemId(goodsKey.goodsId, sourceId);
                    else
                        windwUserData.SettingGemGoodsCfgId(goodsKey.goodsCfgId, EnGemQuality.None, sourceId);
                    UIMgr.Instance.ShowWindow<UIGemInfoDialogData>(windwUserData);
                    ClassPoolMgr.Instance.Push(windwUserData);
                }
                break;
            default:
                {
                    var windwUserData = ClassPoolMgr.Instance.Pull<UIGoodsItemInfoTipsUserData>();
                    windwUserData.Setting(goodsKey, sourceId);
                    UIMgr.Instance.ShowWindow<UIGoodsItemInfoTipsData>(windwUserData);
                    ClassPoolMgr.Instance.Push(windwUserData);
                }
                break;
        }
    }
}