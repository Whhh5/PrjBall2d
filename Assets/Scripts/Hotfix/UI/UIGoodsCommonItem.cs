using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UIGoodsCommonItem : UIGoodsItemBase
{
    protected override async UniTask<UIGoodsItemIconBase> CreateItemIcon(RectTransform parent, GoodsKey goodsKey, Action action, CancellationToken token, EnSourceId sourceId)
    {
        var goodsCfg = GameSchedule.Instance.GetGoodsCfg0(goodsKey.goodsCfgId);
        var goodsTypeCfg = GameSchedule.Instance.GetGoodsTypeCfg0(goodsCfg.nGoodsTypeId);
        var (isCancel, item) = await UIMgr.Instance.LoadWindowItemAsync<UIGoodsItemIconBase>((EnLoadTarget)goodsTypeCfg.nItemIconAssetId, parent, token)
            .SuppressCancellationThrow();
        if (isCancel)
            throw new System.OperationCanceledException();
        item.Setting(goodsKey, action, sourceId);
        return item;
    }

}
