using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICoreInfoDialogUserData : UIWindowUserData
{
    public int coreId;
    public int coreGoodsCfgId;
    public EnCoreQuality quality;
    public EnSourceId sourceId;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        coreId
            = coreGoodsCfgId
            = -1;
        sourceId = EnSourceId.None;
        quality = EnCoreQuality.None;
    }
    public void SettingCoreId(in int coreId, in EnSourceId sourceId)
    {
        this.coreId = coreId;
        this.sourceId = sourceId;
    }
    public void SettingCoreGoodsCfgId(in int coreGoodsCfgId, in EnCoreQuality quality, in EnSourceId sourceId)
    {
        this.coreGoodsCfgId = coreGoodsCfgId;
        this.quality = quality;
        this.sourceId = sourceId;
    }
}
public class UICoreInfoDialogData : UIWindowData<UICoreInfoDialog, UICoreInfoDialogUserData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UICoreInfoDialog;
    public override EnUIWindowType WindowType => EnUIWindowType.Dialog;

    private int _CoreId = -1;
    private int _CoreGoodsCfgId = -1;
    private EnCoreQuality _CoreQuality;
    private EnSourceId _SourceId;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        _CoreId
            = _CoreGoodsCfgId
            = -1;
        _SourceId = EnSourceId.None;
        _CoreQuality = EnCoreQuality.None;
    }
    protected override void OnPoolInit(UICoreInfoDialogUserData userData)
    {
        _CoreId = userData.coreId;
        _CoreGoodsCfgId = userData.coreGoodsCfgId;
        _CoreQuality = userData.quality;
        _SourceId = userData.sourceId;
    }
    public override void OnGOCreate(GameEntity entity)
    {
        base.OnGOCreate(entity);
        _UIWindow.SetHideButton(_SourceId != EnSourceId.UIHallMainCoreWindow);
        if (_CoreId > 0)
            _UIWindow.SetCoreId(_CoreId);
        else if (_CoreGoodsCfgId > 0)
            _UIWindow.SetCoreGoodsCfgId(_CoreGoodsCfgId, _CoreQuality);
    }
}
public class UICoreInfoDialog : UIWindow
{
    [SerializeField, AutoReference]
    private Button _CloseBtn = null;
    [SerializeField, AutoReference]
    private Button _WearBtn = null;
    [SerializeField, AutoReference]
    private Button _UnwearBtn = null;
    [SerializeField, AutoReference]
    private Button _ResetValueBtn = null;
    [SerializeField, AutoReference]
    private Button _InlayBtn = null;

    [SerializeField, AutoReference]
    private TextMeshProUGUI _TitleTxt = null;
    [SerializeField, AutoReference]
    private TextMeshProUGUI _DescTxt = null;
    [SerializeField, AutoReference]
    private TextMeshProUGUI _QualityTxt = null;
    [SerializeField, AutoReference]
    private RectTransform _CoreItemRootRect = null;

    [SerializeField, AutoReference]
    private RectTransform _GemArrtibuteRootRect = null;

    [SerializeField, AutoReference]
    private RectTransform _RefreshNeedGoodsIteRootRect = null;

    private CancellationTokenSource _CreateToken;
    private readonly Dictionary<int, UIGoodsItemCountLeftTip> _NeedGoodsItems = new();
    private readonly Dictionary<int, UICoreInfoDialog_GemAttributeItem> _GemAttributeItems = new();
    private int _CoreId = -1;
    private UIGoodsCoreItemIcon _CoreItem = null;
    private bool _IsHideButton = false;
    public override void OnUnload()
    {
        _CreateToken?.Cancel();
        foreach (var item in _GemAttributeItems)
            UIMgr.Instance.UnloadWindowItem(item.Value);
        foreach (var item in _NeedGoodsItems)
            UIMgr.Instance.UnloadWindowItem(item.Value);

        _CloseBtn.RemoveListener(OnClick_CloseBtn);
        _WearBtn.RemoveListener(OnClick_WearBtn);
        _UnwearBtn.RemoveListener(OnClick_UnwearBtn);
        _ResetValueBtn.RemoveListener(OnClick_ResetValueBtn);
        _InlayBtn.RemoveListener(OnClick_InlayBtn);

        EventMgr.Instance.Unregister<GameGoodsChangeEventData>(EnABBEvent.EVENT_GOODS_CHANGE, 0, 0, OnEvent_GoodsChange);

        if (_CoreId > 0)
            EventMgr.Instance.Unregister<GameCoreChangeEffectValueEventData>(EnABBEvent.EVENT_CORE_CHANGE_EFFECTVALUE, _CoreId, 0, OnEvent_CoreChangeEffectValue);
        if (_CoreItem != null)
            UIMgr.Instance.UnloadWindowItem(_CoreItem);
        base.OnUnload();
        _CoreItem = null;
        _CoreId = -1;
        _NeedGoodsItems.Clear();
        _GemAttributeItems.Clear();
        _CreateToken = null;
        _IsHideButton = false;
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();
        _CloseBtn.AddListener(OnClick_CloseBtn);
        _WearBtn.AddListener(OnClick_WearBtn);
        _UnwearBtn.AddListener(OnClick_UnwearBtn);
        _ResetValueBtn.AddListener(OnClick_ResetValueBtn);
        _InlayBtn.AddListener(OnClick_InlayBtn);

        EventMgr.Instance.Register<GameGoodsChangeEventData>(EnABBEvent.EVENT_GOODS_CHANGE, 0, 0, OnEvent_GoodsChange);

    }
    private void OnClick_CloseBtn()
    {
        UIMgr.Instance.HideWindow(_WindowId);
    }

    private void OnClick_WearBtn()
    {
        if (!UICoreMgr.Instance.WearCore(_CoreId))
            return;
        UIMgr.Instance.HideWindow(_WindowId);
    }
    private void OnClick_UnwearBtn()
    {
        UICoreMgr.Instance.UnwearCore(_CoreId);
        UIMgr.Instance.HideWindow(_WindowId);
    }
    private void OnClick_ResetValueBtn()
    {
        var needGoodsInfos = UICoreUtility.GetAptitudeNeedGoodsInfos(_CoreId);
        foreach (var item in needGoodsInfos)
        {
            UIGoodsMgr.Instance.RemoveGoods(item.goodsCfgId, item.goodsCount);
        }

        var cfgId = UICoreUtility.GetCoreCfgId(_CoreId);
        var quality = UICoreUtility.GetCoreQuality(_CoreId);
        var value = UICoreUtility.GetRandomCoreEffectValue(cfgId, quality);
        UICoreMgr.Instance.SetCoreEffectValue(_CoreId, value);
    }
    private void OnClick_InlayBtn()
    {
        EventMgr.Instance.FireExecute(EnABBEvent.EVENT_HALLMAIN_CHANGE_PAGE, 0, (int)EnUIHallMainMenuItemId.Inlay);
        UIMgr.Instance.HideWindow(_WindowId);
    }
    private void OnEvent_CoreChangeEffectValue(in int coreId, in int _, in GameCoreChangeEffectValueEventData data)
    {
        var descStr = UICoreUtility.GetDescriptsByCoreId(coreId);
        _DescTxt.SetText(descStr);
    }
    private void OnEvent_GoodsChange(in int goodsCfgId, in int _, in GameGoodsChangeEventData data)
    {
        if (!_NeedGoodsItems.ContainsKey(goodsCfgId))
            return;
        UpdateRefreshInfo();
        UpdateRefreshButtonStatus();
    }
    public void SetHideButton(bool isHideButton)
    {
        _IsHideButton = isHideButton;
    }
    public async void SetCoreGoodsCfgId(int coreGoodsCfgId, EnCoreQuality quality)
    {
        SetBaseInfo(coreGoodsCfgId, quality);

        var descStr = UICoreUtility.GetDescriptsByCoreCfgId(coreGoodsCfgId, quality);
        _DescTxt.SetText(descStr);

        var (isCancel, item) = await UIGoodsUtility.CreateGoodsItemIcon<UIGoodsCoreItemIcon>(EnLoadTarget.Pre_UIGoodsCoreItemIcon, _CoreItemRootRect, GetWindowToken())
            .SuppressCancellationThrow();
        if (isCancel)
            return;
        item.Setting(new(coreGoodsCfgId), null, EnSourceId.UICoreInfoDialog);
        _CoreItem = item;
        UpdateButtonStatus();
    }
    public async void SetCoreId(int coreId)
    {
        _CoreId = coreId;
        var goodsCfgId = UICoreUtility.GetGoodsCfgId(coreId);
        var coreCfgId = UICoreUtility.GetCoreCfgId(coreId);
        var quality = UICoreUtility.GetCoreQuality(coreId);
        SetBaseInfo(goodsCfgId, quality);

        var descStr = UICoreUtility.GetDescriptsByCoreId(coreId);
        _DescTxt.SetText(descStr);

        var (isCancel, item) = await UIGoodsUtility.CreateGoodsItemIcon<UIGoodsCoreItemIcon>(EnLoadTarget.Pre_UIGoodsCoreItemIcon, _CoreItemRootRect, GetWindowToken())
            .SuppressCancellationThrow();
        if (isCancel)
            return;
        item.SettingCoreId(coreId, null, EnSourceId.UICoreInfoDialog);
        _CoreItem = item;
        UpdateButtonStatus();

        UpdateRefreshInfo();
        UpdateRefreshButtonStatus();
        UpdateGemInfos(coreId);

        EventMgr.Instance.Register<GameCoreChangeEffectValueEventData>(EnABBEvent.EVENT_CORE_CHANGE_EFFECTVALUE, coreId, 0, OnEvent_CoreChangeEffectValue);
    }
    private void SetBaseInfo(int goodsCfgId, EnCoreQuality quality)
    {
        var goodsName = UIGoodsMgr.Instance.GetGoodsName(goodsCfgId);
        _TitleTxt.SetText(goodsName);

        var qualityName = LanguageUtility.GetLanguage_CoreQualityName(quality);
        _QualityTxt.SetText(qualityName);
    }
    private void UpdateButtonStatus()
    {
        if (_CoreId <= 0)
        {
            _WearBtn.gameObject.SetActive(false);
            _UnwearBtn.gameObject.SetActive(false);
            _ResetValueBtn.gameObject.SetActive(false);
            _GemArrtibuteRootRect.gameObject.SetActive(false);
        }
        else
        {
            var isWear = UICoreMgr.Instance.IsWear(_CoreId);
            _WearBtn.gameObject.SetActive(!_IsHideButton && !isWear);
            _UnwearBtn.gameObject.SetActive(!_IsHideButton && isWear);
            _ResetValueBtn.gameObject.SetActive(!_IsHideButton);

            var inlayGemCount = UICoreUtility.GetCoreGemInlayCount(_CoreId);
            _GemArrtibuteRootRect.gameObject.SetActive(inlayGemCount > 0);
        }
    }
    private async void UpdateRefreshInfo()
    {
        _CreateToken = new();
        var needGoodsInfos = UICoreUtility.GetAptitudeNeedGoodsInfos(_CoreId);

        foreach (var item in _NeedGoodsItems)
            UIMgr.Instance.UnloadWindowItem(item.Value);
        _NeedGoodsItems.Clear();

        for (int i = 0; i < needGoodsInfos.Count; i++)
        {
            var goodsInfo = needGoodsInfos[i];
            var goodsCount = goodsInfo.goodsCount;
            if (!_NeedGoodsItems.TryGetValue(goodsInfo.goodsCfgId, out var tipsItem))
            {
                var (isCancel, item) = await UIMgr.Instance.LoadWindowItemAsync<UIGoodsItemCountLeftTip>(EnLoadTarget.Pre_UIGoodsItemCountLeftTip, _RefreshNeedGoodsIteRootRect, _CreateToken.Token)
                    .SuppressCancellationThrow();
                if (isCancel)
                    return;
                item.Setting(new(goodsInfo.goodsCfgId, goodsCount, _CoreId), EnSourceId.UICoreInfoDialog);
                tipsItem = item;
                _NeedGoodsItems.Add(goodsInfo.goodsCfgId, tipsItem);
            }
            else
            {
                tipsItem.SetCount(goodsCount);
            }
            var count = UIGoodsMgr.Instance.GetGoodsCount(goodsInfo.goodsCfgId);
            var isValid = count >= goodsCount;
            tipsItem.SetValid(isValid);
        }
        _CreateToken = null;
    }
    private void UpdateRefreshButtonStatus()
    {
        var needGoodsInfos = UICoreUtility.GetAptitudeNeedGoodsInfos(_CoreId);
        var isActive = true;
        foreach (var item in needGoodsInfos)
        {
            var count = UIGoodsMgr.Instance.GetGoodsCount(item.goodsCfgId);
            isActive &= count >= item.goodsCount;
            if (!isActive)
                break;
        }
        _ResetValueBtn.interactable = isActive;

        _ResetValueBtn.gameObject.SetActive(!_IsHideButton);
        _RefreshNeedGoodsIteRootRect.gameObject.SetActive(!_IsHideButton);
    }
    private async void UpdateGemInfos(int coreId)
    {
        var count = UICoreUtility.GetCoreGemGrooveCount(coreId);
        for (int i = 0; i < count; i++)
        {
            if (!UICoreUtility.ContainsGem(coreId, i))
                continue;
            var (isCancel, item) = await UIMgr.Instance.LoadWindowItemAsync<UICoreInfoDialog_GemAttributeItem>(EnLoadTarget.Pre_UICoreInfoDialog_GemAttributeItem, _GemArrtibuteRootRect, GetWindowToken())
                .SuppressCancellationThrow();
            if (isCancel)
                return;
            var gemId = UICoreUtility.GetCoreGrooveGemId(coreId, i);
            item.Setting(gemId);
            _GemAttributeItems.Add(gemId, item);
        }
    }
}
