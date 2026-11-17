using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIControllerDialogUserData : UIWindowUserData
{
    public string content;
    public Action confirmationCallback;
    public Action cancelCallback;
    public List<GoodsInfo> goodsInfos;
    public EnSourceId sourceId;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();

        content = null;
        confirmationCallback = null;
        cancelCallback = null;
        goodsInfos = null;
        sourceId = EnSourceId.None;
    }

    public void Setting(string content, Action confirmationCallback, Action cancelCallback, List<GoodsInfo> goodsInfos, EnSourceId sourceId)
    {
        this.content = content;
        this.confirmationCallback = confirmationCallback;
        this.cancelCallback = cancelCallback;
        this.goodsInfos = goodsInfos;
        this.sourceId = sourceId;
    }
    public void Setting(string content, Action confirmationCallback, Action cancelCallback, GoodsInfo goodsInfo, EnSourceId sourceId)
    {
        Setting(content, confirmationCallback, cancelCallback, new List<GoodsInfo>() { goodsInfo }, sourceId);
    }
}
public class UIControllerDialogData : UIWindowData<UIControllerDialog, UIControllerDialogUserData>
{
    public override EnUIWindowType WindowType => EnUIWindowType.Dialog;

    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIControllerDialog;


    private string _Content;
    private Action _ConfirmationCallback;
    private Action _CancelCallback;
    private List<GoodsInfo> _GoodsInfos;
    private EnSourceId _SourceId;

    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        _Content = null;
        _ConfirmationCallback = null;
        _CancelCallback = null;
        _GoodsInfos = default;
        _SourceId = EnSourceId.None;
    }
    protected override void OnPoolInit(UIControllerDialogUserData userData)
    {
        _Content = userData.content;
        _ConfirmationCallback = userData.confirmationCallback;
        _CancelCallback = userData.cancelCallback;
        _GoodsInfos = userData.goodsInfos ?? new();
        _SourceId = userData.sourceId;
    }

    public override void OnGOCreate(GameEntity entity)
    {
        base.OnGOCreate(entity);
        _UIWindow.Setting(_Content, _ConfirmationCallback, _CancelCallback, _GoodsInfos, _SourceId);
    }
}

public class UIControllerDialog : UIWindow
{
    [SerializeField, AutoReference]
    private Button _ConfirmationBtn;
    [SerializeField, AutoReference]
    private Button _CancelBtn;
    [SerializeField, AutoReference]
    private TextMeshProUGUI _DescTxt;
    [SerializeField, AutoReference]
    private RectTransform _GoodsRootRect;

    private Action _ConfirmationCallback;
    private Action _CancelCallback;
    private readonly List<UIGoodsItemCountBottomTip> _GoodsTips = new();

    public override void OnUnload()
    {
        foreach (var item in _GoodsTips)
            UIMgr.Instance.UnloadWindowItem(item);

        _ConfirmationBtn.RemoveListener(OnClick_ConfirmationBtn);
        _CancelBtn.RemoveListener(OnClick_CancelBtn);
        base.OnUnload();
        _ConfirmationCallback = null;
        _CancelCallback = null;
        _GoodsTips.Clear();
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();
        _ConfirmationBtn.AddListener(OnClick_ConfirmationBtn);
        _CancelBtn.AddListener(OnClick_CancelBtn);
    }
    public void Setting(string content, Action confirmationCallback, Action cancelCallback, List<GoodsInfo> goodsInfos, EnSourceId sourceId)
    {
        _DescTxt.SetText(content);
        _ConfirmationCallback = confirmationCallback;
        _CancelCallback = cancelCallback;

        if (goodsInfos.Count > 0)
        {
            LoadGoodsTips(goodsInfos, sourceId);
        }
        _GoodsRootRect.gameObject.SetActive(goodsInfos.Count > 0);
    }
    private async void LoadGoodsTips(List<GoodsInfo> goodsInfos, EnSourceId sourceId)
    {
        for (int i = 0; i < goodsInfos.Count; i++)
        {
            var (isCancel, item) = await UIMgr.Instance.LoadWindowItemAsync<UIGoodsItemCountBottomTip>(EnLoadTarget.Pre_UIGoodsItemCountBottomTip, _GoodsRootRect, GetWindowToken())
                .SuppressCancellationThrow();
            if (isCancel)
                break;
            var goodsInfo = goodsInfos[i];
            item.Setting(goodsInfo, sourceId);
            _GoodsTips.Add(item);
        }
    }

    private void OnClick_ConfirmationBtn()
    {
        _ConfirmationCallback?.Invoke();
        UIMgr.Instance.HideWindow(_WindowId);
    }
    private void OnClick_CancelBtn()
    {
        _CancelCallback?.Invoke();
        UIMgr.Instance.HideWindow(_WindowId);
    }
}
