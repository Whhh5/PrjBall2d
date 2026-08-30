using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGemInfoDialogUserData : UIWindowUserData
{
    public int gemId;
    public int gemGoodsCfgId;
    public EnGemQuality quality;
    public EnSourceId sourceId;
    public Func<bool> inlayCallback;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        gemId
            = gemGoodsCfgId
            = -1;
        sourceId = EnSourceId.None;
        quality = EnGemQuality.None;
        inlayCallback = null;
    }
    public void SettingGemId(in int gemId, in EnSourceId sourceId)
    {
        this.gemId = gemId;
        this.sourceId = sourceId;
    }
    public void SettingGemGoodsCfgId(in int gemGoodsCfgId, in EnGemQuality quality, in EnSourceId sourceId)
    {
        this.gemGoodsCfgId = gemGoodsCfgId;
        this.quality = quality;
        this.sourceId = sourceId;
    }
}
public class UIGemInfoDialogData : UIWindowData<UIGemInfoDialog, UIGemInfoDialogUserData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIGemInfoDialog;
    public override EnUIWindowType WindowType => EnUIWindowType.Dialog;

    private int _GemId = -1;
    private int _GemGoodsCfgId = -1;
    private EnGemQuality _GemQuality;
    private EnSourceId _SourceId;
    private Func<bool> _InlayCallback;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        _GemId
            = _GemGoodsCfgId
            = -1;
        _SourceId = EnSourceId.None;
        _GemQuality = EnGemQuality.None;
    }
    protected override void OnPoolInit(UIGemInfoDialogUserData userData)
    {
        _GemId = userData.gemId;
        _GemGoodsCfgId = userData.gemGoodsCfgId;
        _GemQuality = userData.quality;
        _SourceId = userData.sourceId;
        _InlayCallback = userData.inlayCallback;
    }
    public override void OnGOCreate(GameEntity entity)
    {
        base.OnGOCreate(entity);

        if (_GemId > 0)
            _UIWindow.SetGemId(_GemId, _InlayCallback);
        else if (_GemGoodsCfgId > 0)
            _UIWindow.SetGemGoodsCfgId(_GemGoodsCfgId, _GemQuality);
    }
}
public class UIGemInfoDialog : UIWindow
{
    [SerializeField, AutoReference]
    private Button _CloseBtn = null;
    [SerializeField, AutoReference]
    private Button _InlayBtn = null;

    [SerializeField, AutoReference]
    private TextMeshProUGUI _TitleTxt = null;
    [SerializeField, AutoReference]
    private TextMeshProUGUI _DescTxt = null;
    [SerializeField, AutoReference]
    private TextMeshProUGUI _QualityTxt = null;
    [SerializeField, AutoReference]
    private RectTransform _GemItemRootRect = null;

    private Func<bool> _InlayCallback;
    private readonly Dictionary<int, UIGoodsItemCountLeftTip> _NeedGoodsItems = new();
    private int _GemId = -1;
    private UIGoodsGemItemIcon _GemItem = null;
    public override void OnUnload()
    {
        foreach (var item in _NeedGoodsItems)
        {
            UIMgr.Instance.UnloadWindowItem(item.Value);
        }
        _CloseBtn.RemoveListener(OnClick_CloseBtn);
        _InlayBtn.RemoveListener(OnClick_InlayBtn);

        if (_GemItem != null)
            UIMgr.Instance.UnloadWindowItem(_GemItem);
        base.OnUnload();
        _GemItem = null;
        _GemId = -1;
        _NeedGoodsItems.Clear();
        _InlayCallback = null;
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();
        _CloseBtn.AddListener(OnClick_CloseBtn);
        _InlayBtn.AddListener(OnClick_InlayBtn);
    }
    private void OnClick_CloseBtn()
    {
        UIMgr.Instance.HideWindow(_WindowId);
    }
    private void OnClick_InlayBtn()
    {
        var isClose = _InlayCallback.Invoke();
        if (isClose)
        {
            UIMgr.Instance.HideWindow(_WindowId);
        }
    }
    public async void SetGemGoodsCfgId(int gemGoodsCfgId, EnGemQuality quality)
    {
        SetBaseInfo(gemGoodsCfgId, quality);
        UpdateButtonStatus();

        var descStr = UIGemUtility.GetDescriptByGemGoodsCfgId(gemGoodsCfgId, quality);
        _DescTxt.SetText(descStr);

        var (isCancel, item) = await UIGoodsUtility.CreateGoodsItemIcon<UIGoodsGemItemIcon>(EnLoadTarget.Pre_UIGoodsGemItemIcon, _GemItemRootRect, GetWindowToken())
            .SuppressCancellationThrow();
        if (isCancel)
            return;
        item.Setting(new(gemGoodsCfgId), null, EnSourceId.UIGemInfoDialog);
        _GemItem = item;
    }
    public async void SetGemId(int gemId, Func<bool> inlayCallback)
    {
        _GemId = gemId;
        _InlayCallback = inlayCallback;
        var goodsCfgId = UIGemUtility.GetGemGoodsCfgId(gemId);
        var quality = UIGemUtility.GetGemQuality(gemId);
        SetBaseInfo(goodsCfgId, quality);
        UpdateButtonStatus();


        var descStr = UIGemUtility.GetDescriptByGemId(gemId);
        _DescTxt.SetText(descStr);

        var (isCancel, item) = await UIGoodsUtility.CreateGoodsItemIcon<UIGoodsGemItemIcon>(EnLoadTarget.Pre_UIGoodsGemItemIcon, _GemItemRootRect, GetWindowToken())
            .SuppressCancellationThrow();
        if (isCancel)
            return;
        item.SettingGemId(gemId, null, EnSourceId.UIGemInfoDialog);
        _GemItem = item;
    }
    private void SetBaseInfo(int goodsCfgId, EnGemQuality quality)
    {
        var goodsName = UIGoodsMgr.Instance.GetGoodsName(goodsCfgId);
        _TitleTxt.SetText(goodsName);

        var qualityName = LanguageUtility.GetLanguage_GemQualityName(quality);
        _QualityTxt.SetText(qualityName);
    }
    private void UpdateButtonStatus()
    {
        _InlayBtn.gameObject.SetActive(_InlayCallback != null);
    }
}
