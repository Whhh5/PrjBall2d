using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class UIGoodsCoreItemIcon : UIGoodsItemIconBase
{
    [SerializeField, AutoReference]
    private Image _QualityImg;
    [SerializeField, AutoReference]
    private Image _TypeImg;
    [SerializeField, AutoReference]
    private Image _IconImg;
    [SerializeField, AutoReference]
    private RectTransform _GemGrooveRootRect;

    private int _CoreId = -1;
    private readonly List<UIGemGrooveItem> _GrooveItem = new();
    private CancellationTokenSource _TokenSource;
    public override void Unload()
    {
        if (_TokenSource != null)
        {
            _TokenSource.Cancel();
            _TokenSource = null;
        }
        foreach (var item in _GrooveItem)
        {
            UIMgr.Instance.UnloadWindowItem(item);
        }
        if (_CoreId > 0)
        {
            EventMgr.Instance.Unregister(EnABBEvent.EVENT_CORE_CLEAR_GEM, _CoreId, 0, OnEvent_CoreClearGem);
            EventMgr.Instance.Unregister<GameCoreInlayGemEventData>(EnABBEvent.EVENT_CORE_INLAY_GEM, _CoreId, 0, OnEvent_CoreInlayGem);
        }

        _TypeImg.UnloadSprite();
        _IconImg.UnloadSprite();
        base.Unload();
        _GrooveItem.Clear();
        _CoreId = -1;
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();
    }

    public override void Setting(in GoodsKey goodsKey, Action callback, EnSourceId sourceId)
    {
        _CoreId = goodsKey.goodsId;
        base.Setting(in goodsKey, callback, sourceId);
        var goodsCfg = GameSchedule.Instance.GetGoodsCfg0(goodsKey.goodsCfgId);
        var coreCfgId = goodsCfg.nLinkCfgId;

        var coreCfg = GameSchedule.Instance.GetCoreCfg0(coreCfgId);
        var typeCfg = GameSchedule.Instance.GetCoreTypeCfg0(coreCfg.nCoreType);

        _TypeImg.LoadSprirte(typeCfg.nFrameAssetId);
        var iconAssetId = UICoreUtility.GetCoreIconAssetId(coreCfgId);
        _IconImg.LoadSprirte(iconAssetId);

        _QualityImg.gameObject.SetActive(goodsKey.goodsId > 0);
        _GemGrooveRootRect.gameObject.SetActive(goodsKey.goodsId > 0);
        if (goodsKey.goodsId > 0)
        {
            var quality = UICoreUtility.GetCoreQuality(goodsKey.goodsId);
            var qualityCfg = GameSchedule.Instance.GetCoreQualityCfg0((int)quality);
            if (ColorUtility.TryParseHtmlString(qualityCfg.strColor, out var qualityCol))
            {
                _QualityImg.color = qualityCol;
            }
            CreateGemGroove(goodsKey.goodsId, sourceId);
        }
        if (_CoreId > 0)
        {
            EventMgr.Instance.Register(EnABBEvent.EVENT_CORE_CLEAR_GEM, _CoreId, 0, OnEvent_CoreClearGem);
            EventMgr.Instance.Register<GameCoreInlayGemEventData>(EnABBEvent.EVENT_CORE_INLAY_GEM, _CoreId, 0, OnEvent_CoreInlayGem);
        }
    }
    private void OnEvent_CoreClearGem(in int gemId, in int grooveIndex)
    {
        ClearGrooveGemItem(grooveIndex);
    }
    private void OnEvent_CoreInlayGem(in int gemId, in int __, in GameCoreInlayGemEventData data)
    {
        SetGrooveGemId(data.grooveIndex, data.gemId, null);
    }
    private async void CreateGemGroove(int coreId, EnSourceId sourceId)
    {
        _TokenSource = new();
        var count = UICoreUtility.GetCoreGemGrooveCount(coreId);
        for (int i = 0; i < count; i++)
        {
            var (isCancel, item) = await UIMgr.Instance.LoadWindowItemAsync<UIGemGrooveItem>(EnLoadTarget.Pre_UIGemGrooveItem, _GemGrooveRootRect, _TokenSource.Token)
                .SuppressCancellationThrow();
            if (isCancel)
                return;
            var gemType = UICoreUtility.GetCoreGrooveGemType(coreId, i);
            item.Setting(gemType, null, sourceId);
            _GrooveItem.Add(item);

            if (UICoreUtility.ContainsGem(coreId, i))
            {
                var gemId = UICoreUtility.GetCoreGrooveGemId(coreId, i);
                item.SetInlayGemId(gemId, null);
            }
        }
        _TokenSource = null;
    }

    public void SettingCoreId(int coreId, EnSourceId sourceId)
    {
        var goodsCfgId = UICoreUtility.GetGoodsCfgId(coreId);
        Setting(new(goodsCfgId, coreId), sourceId);
    }
    public void SettingCoreId(int coreId, Action callback, EnSourceId sourceId)
    {
        var goodsCfgId = UICoreUtility.GetGoodsCfgId(coreId);
        Setting(new(goodsCfgId, coreId), callback, sourceId);
    }
    public void ClearGrooveGemItem(int grooveIndex)
    {
        var groove = _GrooveItem[grooveIndex];
        groove.ClearInlayGemItem();
    }
    public void SetGrooveGemId(int grooveIndex, int gemId, Action callback)
    {
        var groove = _GrooveItem[grooveIndex];
        groove.SetInlayGemId(gemId, callback);
    }
}
