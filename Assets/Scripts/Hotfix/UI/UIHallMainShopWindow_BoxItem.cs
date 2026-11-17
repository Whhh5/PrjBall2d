using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHallMainShopWindow_BoxItem : UIWindowItem
{
    [SerializeField, AutoReference]
    private TextMeshProUGUI _TitleTxt;
    [SerializeField, AutoReference]
    private TextMeshProUGUI _DescriptTxt;
    [SerializeField, AutoReference]
    private Image _IconImg;

    [SerializeField, AutoReference]
    private Button _Lottery10Btn;
    [SerializeField, AutoReference]
    private Button _Lottery1Btn;
    [SerializeField, AutoReference]
    private RectTransform _Lottery10NeedGoodsRootRect;
    [SerializeField, AutoReference]
    private RectTransform _Lottery1NeedGoodsRootRect;
    private readonly List<UIGoodsItemCountLeftTip> _NeedGoodsInfoItems = new();

    private GoodsInfo _Lottery1GoodsInfo;
    private GoodsInfo _Lottery10GoodsInfo;
    private Action _Lottery1Callback;
    private Action _Lottery10Callback;
    public override void Unload()
    {
        _IconImg.UnloadSprite();
        foreach (var item in _NeedGoodsInfoItems)
            UIMgr.Instance.UnloadWindowItem(item);

        EventMgr.Instance.Unregister<GameGoodsChangeEventData>(EnABBEvent.EVENT_GOODS_CHANGE, OnEvent_GoodsChange);
        _Lottery1Btn.RemoveListener(OnClick_Lottery1);
        _Lottery10Btn.RemoveListener(OnClick_Lottery10);
        base.Unload();
        _NeedGoodsInfoItems.Clear();
        _Lottery1GoodsInfo
            = _Lottery10GoodsInfo
            = default;
        _Lottery1Callback
            = _Lottery10Callback
            = default;
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();

        EventMgr.Instance.Register<GameGoodsChangeEventData>(EnABBEvent.EVENT_GOODS_CHANGE, OnEvent_GoodsChange);
        _Lottery1Btn.AddListener(OnClick_Lottery1);
        _Lottery10Btn.AddListener(OnClick_Lottery10);

    }
    public async void Setting(string title, string desc, EnLoadTarget iconAssetId, GoodsInfo lottery1GoodsInfo, Action lottery1Callback, GoodsInfo lottery10GoodsInfo, Action lottery10Callback)
    {
        _Lottery1GoodsInfo = lottery1GoodsInfo;
        _Lottery10GoodsInfo = lottery10GoodsInfo;
        _Lottery1Callback = lottery1Callback;
        _Lottery10Callback = lottery10Callback;

        _TitleTxt.SetText(title);
        _DescriptTxt.SetText(desc);
        _IconImg.LoadSprirte(iconAssetId);

        {
            var (isCancel, item) = await UIMgr.Instance.LoadWindowItemAsync<UIGoodsItemCountRightTip>(EnLoadTarget.Pre_UIGoodsItemCountRightTip, _Lottery1NeedGoodsRootRect, Token)
                .SuppressCancellationThrow();
            if (isCancel)
                return;
            item.Setting(lottery1GoodsInfo, EnSourceId.UIHallMainShopWindow);
            _NeedGoodsInfoItems.Add(item);
            var (isCancel2, item2) = await UIMgr.Instance.LoadWindowItemAsync<UIGoodsItemCountRightTip>(EnLoadTarget.Pre_UIGoodsItemCountRightTip, _Lottery10NeedGoodsRootRect, Token)
                .SuppressCancellationThrow();
            if (isCancel2)
                return;
            item2.Setting(lottery10GoodsInfo, EnSourceId.UIHallMainShopWindow);
            _NeedGoodsInfoItems.Add(item2);
        }
        UpdateButtonStatus();
    }
    private void OnClick_Lottery1()
    {
        _Lottery1Callback.Invoke();
    }
    private void OnClick_Lottery10()
    {
        _Lottery10Callback.Invoke();
    }
    private void OnEvent_GoodsChange(in int goodsCfgId, in int _, in GameGoodsChangeEventData data)
    {
        if (_NeedGoodsInfoItems.Count != 2)
            return;
        if (goodsCfgId == _Lottery1GoodsInfo.goodsCfgId || goodsCfgId != _Lottery10GoodsInfo.goodsCfgId)
        {
            UpdateButtonStatus();
        }
    }
    private void UpdateButtonStatus()
    {
        var lottery1GoodsCount = UIGoodsMgr.Instance.GetGoodsCount(_Lottery1GoodsInfo.goodsCfgId);
        var isLottery1Btn = lottery1GoodsCount >= _Lottery1GoodsInfo.goodsCount;
        _Lottery1Btn.interactable = isLottery1Btn;
        _NeedGoodsInfoItems[0].SetComValid(isLottery1Btn);
        var lottery10GoodsCount = UIGoodsMgr.Instance.GetGoodsCount(_Lottery10GoodsInfo.goodsCfgId);
        var isLottery10Btn = lottery10GoodsCount >= _Lottery10GoodsInfo.goodsCount;
        _Lottery10Btn.interactable = isLottery10Btn;
        _NeedGoodsInfoItems[1].SetComValid(isLottery10Btn);
    }
}