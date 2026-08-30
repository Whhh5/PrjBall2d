using System;
using UnityEngine;
using UnityEngine.UI;

public class UIGoodsCommonItemIcon : UIGoodsItemIconBase
{
    [SerializeField, AutoReference]
    private Image _IconImg = null;

    public override void Unload()
    {
        _IconImg.UnloadSprite();
        base.Unload();
    }
    public override void Setting(in GoodsKey goodsKey, Action callback, EnSourceId sourceId)
    {
        base.Setting(goodsKey, callback, sourceId);

        var goodsCfg = GameSchedule.Instance.GetGoodsCfg0(goodsKey.goodsCfgId);
        _IconImg.LoadSprirte(goodsCfg.nIconAssetId);
    }
}
