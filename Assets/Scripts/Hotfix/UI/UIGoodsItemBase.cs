using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct GoodsKey
{
    public int goodsCfgId;
    public int goodsId;

    public GoodsKey(int goodsCfgId)
    {
        this.goodsCfgId
            = goodsId
            = goodsCfgId;
    }
    public GoodsKey(int goodsCfgId, int goodsId)
    {
        this.goodsCfgId = goodsCfgId;
        this.goodsId = goodsId;
    }
}
public abstract class UIGoodsItemBase : UIWindowItem
{
    [SerializeField, AutoReference]
    private Image _IconFrameImg;
    [SerializeField, AutoReference]
    private RectTransform _IconRootRect;
    [SerializeField, AutoReference]
    private TextMeshProUGUI _CountTxt;

    private UIGoodsItemIconBase _ItemIcon = null;
    private CancellationTokenSource _TokenSource = null;

    protected abstract UniTask<UIGoodsItemIconBase> CreateItemIcon(RectTransform parent, GoodsKey goodsKey, Action action, CancellationToken token, EnSourceId sourceId);
    public override void Unload()
    {
        if (_TokenSource != null)
        {
            _TokenSource.Cancel();
            _TokenSource = null;
        }
        if (_ItemIcon != null)
        {
            UIMgr.Instance.UnloadWindowItem(_ItemIcon);
        }
        _IconFrameImg.UnloadSprite();
        base.Unload();
        _ItemIcon = null;
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();
    }
    public async void Setting(GoodsInfo goodsInfo, Action action, EnSourceId sourceId)
    {
        var goodsCfg = GameSchedule.Instance.GetGoodsCfg0(goodsInfo.goodsCfgId);
        _IconFrameImg.LoadSprirte(goodsCfg.nIconFrameAssetId);

        _CountTxt.gameObject.SetActive(goodsInfo.goodsCount > 1);
        if (_CountTxt.gameObject.activeSelf)
        {
            var countStr = LanguageUtility.GetLanguage_10040(goodsInfo.goodsCount);
            _CountTxt.SetText(countStr);
        }
        _TokenSource = new();
        var (isCancel, itemIcon) = await CreateItemIcon(_IconRootRect, goodsInfo, action, _TokenSource.Token, sourceId)
            .SuppressCancellationThrow();
        if (!isCancel)
        {
            itemIcon.SetAutoSize();
            _ItemIcon = itemIcon;
            _TokenSource = null;
        }
    }
    public void Setting(GoodsInfo goodsInfo, EnSourceId sourceId)
    {
        Setting(goodsInfo, () => UIGoodsUtility.DefaultOnClick(goodsInfo, sourceId), sourceId);
    }
}