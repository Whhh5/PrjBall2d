using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBattleDieWindowData : UIWindowData
{
    public override EnUIWindowType WindowType => EnUIWindowType.Window;

    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIBattleDieWindow;

    public override void OnPoolDestroy()
    {
        TimeMgr.Instance.SetPause(false, true);
        base.OnPoolDestroy();
    }
    protected override void OnPoolInit(UIWindowUserData userData)
    {
        base.OnPoolInit(userData);
        TimeMgr.Instance.SetPause(true, true);
    }
}
public class UIBattleDieWindow : UIWindow
{
    [SerializeField, AutoReference]
    private Button _FinishBtn = null;
    [SerializeField, AutoReference]
    private Button _ResurrectionBtn = null;
    [SerializeField, AutoReference]
    private RectTransform _ResurrectionNeedGoodsRootRect = null;

    private UIGoodsItemCountBottomTip _NeedGoodsTips;
    public override void OnUnload()
    {
        if (_NeedGoodsTips != null)
        {
            UIMgr.Instance.UnloadWindowItem(_NeedGoodsTips);
            _NeedGoodsTips = null;
        }
        _FinishBtn.RemoveListener(OnClick_FinishBtn);
        _ResurrectionBtn.RemoveListener(OnClick_ResurrectionBtn);
        base.OnUnload();
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();
        _FinishBtn.AddListener(OnClick_FinishBtn);
        _ResurrectionBtn.AddListener(OnClick_ResurrectionBtn);
        UpdateButtonStatus();
        LoadNeedGoodsInfo();
    }
    private async void LoadNeedGoodsInfo()
    {
        var goodsId = GlobalCfgUtility.Get30111_Int32();
        var count = GlobalCfgUtility.Get30112_Int32();
        var (isCancel, item) = await UIMgr.Instance.LoadWindowItemAsync<UIGoodsItemCountBottomTip>(
            EnLoadTarget.Pre_UIGoodsItemCountBottomTip,
            _ResurrectionNeedGoodsRootRect,
            GetWindowToken()).SuppressCancellationThrow();
        if (isCancel)
            return;
        item.Setting(new(goodsId, count), EnSourceId.UIBattleDieWindow);
        item.SetAutoSize();
        _NeedGoodsTips = item;
        UpdateButtonStatus();
    }

    private void OnClick_FinishBtn()
    {
        UIMgr.Instance.HideWindow(_WindowId);
        GameBattleMgr.Instance.SetBattleStatus(EnGameBattleStatus.Faild);
    }
    private void OnClick_ResurrectionBtn()
    {
        var goodsId = GlobalCfgUtility.Get30111_Int32();
        var needCount = GlobalCfgUtility.Get30112_Int32();
        UIGoodsMgr.Instance.RemoveGoods(goodsId, needCount);

        UIMgr.Instance.HideWindow(_WindowId);
        GameBattleMgr.Instance.SetBattleStatus(EnGameBattleStatus.Battle);
        GameBattleMgr.Instance.Resurrection();
    }
    private void UpdateButtonStatus()
    {
        var goodsId = GlobalCfgUtility.Get30111_Int32();
        var needCount = GlobalCfgUtility.Get30112_Int32();
        var curCount = UIGoodsMgr.Instance.GetGoodsCount(goodsId);
        var isActive = curCount >= needCount;
        _ResurrectionBtn.interactable = isActive;
        if (_NeedGoodsTips != null)
            _NeedGoodsTips.SetComValid(isActive);
    }
}
