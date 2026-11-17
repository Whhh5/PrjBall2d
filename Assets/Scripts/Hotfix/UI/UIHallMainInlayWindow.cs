using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHallMainInlayWindowData : UIWindowData
{
    public override EnUIWindowType WindowType => EnUIWindowType.SubWindow;

    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIHallMainInlayWindow;
}

public class UIHallMainInlayWindow : UIWindow
{
    [SerializeField, AutoReference]
    private RectTransform _CoreItemRootRect;
    [SerializeField, AutoReference]
    private RectTransform _GemItemRootRect;
    [SerializeField, AutoReference]
    private RectTransform _SelectCoreItemRootRect;
    [SerializeField, AutoReference]
    private Button _ApplyBtn;
    [SerializeField, AutoReference]
    private Button _ClearGemBtn;
    [SerializeField, AutoReference]
    private Button _ResolveGemBtn;
    [SerializeField, AutoReference]
    private Button _SortBtn;
    [SerializeField, AutoReference]
    private RectTransform _ClearNeedGoodsRootRect;
    [SerializeField, AutoReference]
    private TextMeshProUGUI _SelCoreAttTxt;
    [SerializeField, AutoReference]
    private RectTransform _SelCoreGemInfoRootRect;
    [SerializeField, AutoReference]
    private GameObject _ApplyHitGo;

    private int _SelectCoreId;
    private UIGoodsCoreItemIcon _SelectCoreItemIcon;

    private readonly Dictionary<int, UIGoodsCoreItemIcon> _CoreIconItems = new();
    private readonly Dictionary<int, UIGoodsGemItemIcon> _GemIconItems = new();
    private readonly Dictionary<int, int> _InlayGrooveIndex2GemIds = new();
    private readonly Dictionary<int, UIHallMainInlayWindow_GemAttributeItem> _InlayGrooveIndex2GemInfoItem = new();
    private readonly Dictionary<int, UIGoodsItemCountLeftTip> _ClearNeedGoodsInfoItems = new();
    private List<int> _TempIntList = new();
    private readonly HashSet<int> _TempIntHashSet = new();
    private CancellationTokenSource _CreateNeedGoodsInfoToken;
    private CancellationTokenSource _CreateGemAttributeInfoToken;

    public override void OnUnload()
    {
        if (_CreateGemAttributeInfoToken != null)
        {
            _CreateGemAttributeInfoToken.Cancel();
            _CreateGemAttributeInfoToken = null;
        }
        if (_CreateNeedGoodsInfoToken != null)
        {
            _CreateNeedGoodsInfoToken.Cancel();
            _CreateNeedGoodsInfoToken = null;
        }
        if (_SelectCoreItemIcon != null)
            UIMgr.Instance.UnloadWindowItem(_SelectCoreItemIcon);

        foreach (var item in _InlayGrooveIndex2GemInfoItem)
            UIMgr.Instance.UnloadWindowItem(item.Value);
        foreach (var item in _ClearNeedGoodsInfoItems)
            UIMgr.Instance.UnloadWindowItem(item.Value);
        foreach (var item in _CoreIconItems)
            UIMgr.Instance.UnloadWindowItem(item.Value);
        foreach (var item in _GemIconItems)
            UIMgr.Instance.UnloadWindowItem(item.Value);

        _ApplyBtn.RemoveListener(OnClick_ApplyBtn);
        _ClearGemBtn.RemoveListener(OnClick_ClearGemBtn);
        _ResolveGemBtn.RemoveListener(OnClick_ResolveGemBtn);
        _SortBtn.RemoveListener(OnClick_SortBtn);
        EventMgr.Instance.Unregister<GameGoodsChangeEventData>(EnABBEvent.EVENT_GOODS_CHANGE, 0, 0, OnEvent_GoodsChange);
        EventMgr.Instance.Unregister(EnABBEvent.EVENT_GEM_REMOVE, 0, 0, OnEvent_RemoveRem);
        base.OnUnload();
        _GemIconItems.Clear();
        _CoreIconItems.Clear();
        _InlayGrooveIndex2GemIds.Clear();
        _SelectCoreId
            = -1;
        _SelectCoreItemIcon = null;
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();
        _ApplyBtn.AddListener(OnClick_ApplyBtn);
        _ClearGemBtn.AddListener(OnClick_ClearGemBtn);
        _ResolveGemBtn.AddListener(OnClick_ResolveGemBtn);
        _SortBtn.AddListener(OnClick_SortBtn);
        EventMgr.Instance.Register<GameGoodsChangeEventData>(EnABBEvent.EVENT_GOODS_CHANGE, 0, 0, OnEvent_GoodsChange);
        EventMgr.Instance.Register(EnABBEvent.EVENT_GEM_REMOVE, 0, 0, OnEvent_RemoveRem);

        CreateCoreIconItems();
        CreateGemIconItems();
        UpdateButtonStatus();
        CreateClearNeedGoodsInfoItems();
        UpdateCoreInfos();
    }
    private void OnClick_ApplyBtn()
    {
        foreach (var item in _InlayGrooveIndex2GemIds)
        {
            _SelectCoreItemIcon.ClearGrooveGemItem(item.Key);
            UICoreMgr.Instance.SetCoreGemId(_SelectCoreId, item.Key, item.Value);
        }
        _InlayGrooveIndex2GemIds.Clear();
        UpdateButtonStatus();
    }
    private void OnClick_ResolveGemBtn()
    {
        var selectList = new List<string>();
        for (var i = EnGemQuality.None + 1; i < EnGemQuality.EnumCount; i++)
        {
            var name = LanguageUtility.GetLanguage_GemQualityName(i);
            selectList.Add(name);
        }
        var windowUserData = ClassPoolMgr.Instance.Pull<UICoreQualitySelectDialogUserData>();
        windowUserData.Setting(selectList, ResolveGem);
        UIMgr.Instance.ShowWindow<UICoreQualitySelectDialogData>(windowUserData);
        ClassPoolMgr.Instance.Push(windowUserData);
    }
    private void OnClick_SortBtn()
    {
        RefreshCoreSort();
        RefreshGemSort();
    }

    private void OnClick_ClearGemBtn()
    {
        if (UIMgr.Instance.ContainsWindow<UIControllerDialogData>())
            return;
        var needGoods = UIGemUtility.ClearGemNeedGoodsInfo();
        var goodsName = UIGoodsMgr.Instance.GetGoodsName(needGoods.goodsCfgId);
        var content = string.Format( LanguageUtility.GetLanguage(30102), needGoods.goodsCount, goodsName);
        UIUtil.ShowControllerDialog(content, EnSourceId.UIHallMainInlayWindow, ClearCoreGem, null, needGoods);
    }
    private void ClearCoreGem()
    {
        var needGood = UIGemUtility.ClearGemNeedGoodsInfo();
        UIGoodsMgr.Instance.RemoveGoods(needGood.goodsCfgId, needGood.goodsCount);

        var grooveCount = UICoreUtility.GetCoreGemGrooveCount(_SelectCoreId);
        for (int i = 0; i < grooveCount; i++)
        {
            if (!UICoreUtility.ContainsGem(_SelectCoreId, i))
                continue;
            var gemId = UICoreUtility.GetCoreGrooveGemId(_SelectCoreId, i);
            UICoreMgr.Instance.ClearCoreGem(_SelectCoreId, i);
            CreateBagGemIconItem(gemId).Forget();
        }
        UpdateButtonStatus();
        UpdateCoreInfos();
    }
    private void OnClick_BagGemIconItem(int gemId)
    {
        var ud = ClassPoolMgr.Instance.Pull<UIGemInfoDialogUserData>();
        ud.SettingGemId(gemId, EnSourceId.UIHallInlayWindow);
        if (_SelectCoreId > 0)
        {
            ud.inlayCallback = () =>
            {
                var result = InlayGem(gemId);
                return result;
            };
        }
        UIMgr.Instance.ShowWindow<UIGemInfoDialogData>(ud);
        ClassPoolMgr.Instance.Push(ud);
    }
    private void OnEvent_GoodsChange(in int goodsCfgId, in int _, in GameGoodsChangeEventData data)
    {
        if (_ClearNeedGoodsInfoItems.ContainsKey(goodsCfgId))
        {
            CreateClearNeedGoodsInfoItems();
            UpdateButtonStatus();
        }
    }
    private void OnEvent_RemoveRem(in int goodsCfgId, in int gemId)
    {
        DestroyBagGemIconItem(gemId);
    }
    private async void CreateClearNeedGoodsInfoItems()
    {
        _CreateNeedGoodsInfoToken?.Cancel();
        _CreateNeedGoodsInfoToken = new();
        foreach (var (_, goodsItem) in _ClearNeedGoodsInfoItems)
            UIMgr.Instance.UnloadWindowItem(goodsItem);
        _ClearNeedGoodsInfoItems.Clear();

        var goodsInfo = UIGemUtility.ClearGemNeedGoodsInfo();
        var (isCancel, item) = await UIMgr.Instance.LoadWindowItemAsync<UIGoodsItemCountBottomTip>(EnLoadTarget.Pre_UIGoodsItemCountBottomTip, _ClearNeedGoodsRootRect, _CreateNeedGoodsInfoToken.Token)
            .SuppressCancellationThrow();
        if (isCancel)
            return;
        item.Setting(goodsInfo, EnSourceId.UIHallMainInlayWindow);
        item.SetAutoSize();
        _ClearNeedGoodsInfoItems.Add(goodsInfo.goodsCfgId, item);

        var goodsCount = UIGoodsMgr.Instance.GetGoodsCount(goodsInfo.goodsCfgId);
        item.SetComValid(goodsCount >= goodsInfo.goodsCount);

        _CreateNeedGoodsInfoToken = null;
    }
    private async void CreateCoreIconItems()
    {
        var idList = new List<int>(100);
        UICoreMgr.Instance.GetAlreadyCoreIds(ref idList);
        UICoreUtility.SortList(ref idList);
        for (int i = 0; i < idList.Count; i++)
        {
            var coreId = idList[i];
            var isCancel = await CreateBagCoreIconItem(coreId)
                .SuppressCancellationThrow();
            if (isCancel)
                break;
        }
    }
    private async void CreateGemIconItems()
    {
        var idList = new List<int>(100);
        UIGemMgr.Instance.GetAleadyGemIds(ref idList);
        UIGemUtility.SortList(ref idList);
        for (int i = 0; i < idList.Count; i++)
        {
            var gemId = idList[i];
            if (UIGemMgr.Instance.GetGemInlayStatus(gemId))
                continue;
            var isCancel = await CreateBagGemIconItem(gemId)
                .SuppressCancellationThrow();
            if (isCancel)
                break;
        }
    }
    private async void UpdateCoreInfos()
    {
        _CreateGemAttributeInfoToken?.Cancel();
        foreach (var item in _InlayGrooveIndex2GemInfoItem)
            UIMgr.Instance.UnloadWindowItem(item.Value);
        _InlayGrooveIndex2GemInfoItem.Clear();

        _SelCoreGemInfoRootRect.gameObject.SetActive(_SelectCoreId > 0);
        _SelCoreAttTxt.gameObject.SetActive(_SelectCoreId > 0);
        if (_SelectCoreId > 0)
        {
            _CreateGemAttributeInfoToken = new();
            var descStr = UICoreUtility.GetDescriptsByCoreId(_SelectCoreId);
            _SelCoreAttTxt.SetText(descStr);

            var gemGrooveCount = UICoreUtility.GetCoreGemGrooveCount(_SelectCoreId);
            for (int i = 0; i < gemGrooveCount; i++)
            {
                if (!_InlayGrooveIndex2GemIds.TryGetValue(i, out var gemId))
                {
                    if (!UICoreUtility.ContainsGem(_SelectCoreId, i))
                        continue;
                    gemId = UICoreUtility.GetCoreGrooveGemId(_SelectCoreId, i);
                }
                var (isCancel, item) = await UIMgr.Instance.LoadWindowItemAsync<UIHallMainInlayWindow_GemAttributeItem>(EnLoadTarget.Pre_UIHallMainInlayWindow_GemAttributeItem, _SelCoreGemInfoRootRect, _CreateGemAttributeInfoToken.Token)
                    .SuppressCancellationThrow();
                if (isCancel)
                    return;
                item.Setting(gemId);
                _InlayGrooveIndex2GemInfoItem.Add(i, item);
            }
            _CreateGemAttributeInfoToken = null;
        }
    }
    private void UpdateButtonStatus()
    {
        var isApplyBtn = _SelectCoreId > 0 && _InlayGrooveIndex2GemIds.Count > 0;
        _ApplyBtn.interactable = isApplyBtn;
        _ApplyBtn.SetComValid(isApplyBtn);
        _ApplyHitGo.SetActive(isApplyBtn);

        var isClearGemBtn = _SelectCoreId > 0;
        if (isClearGemBtn)
        {
            var gemCount = UICoreUtility.GetCoreGemInlayCount(_SelectCoreId);
            isClearGemBtn &= gemCount > 0;
            if (isClearGemBtn)
            {
                var goodsInfo = UIGemUtility.ClearGemNeedGoodsInfo();
                var goodsCount = UIGoodsMgr.Instance.GetGoodsCount(goodsInfo.goodsCfgId);
                isClearGemBtn &= goodsCount >= goodsInfo.goodsCount;
            }
        }
        _ClearGemBtn.interactable = isClearGemBtn;
        _ClearGemBtn.SetComValid(isClearGemBtn);
    }
    private async void LoadSelectCoreItem(int coreId)
    {
        var (isCancel, item) = await UIGoodsUtility.CreateGoodsItemIcon<UIGoodsCoreItemIcon>(EnLoadTarget.Pre_UIGoodsCoreItemIcon, _SelectCoreItemRootRect, GetWindowToken())
            .SuppressCancellationThrow();
        if (isCancel)
            throw new System.OperationCanceledException();
        item.SettingCoreId(coreId, () => UnselectCore(coreId), EnSourceId.UIHallMainGemWindow);
        _SelectCoreItemIcon = item;
    }
    private void UnselectCore(int coreId)
    {
        if (_InlayGrooveIndex2GemIds.Count > 0)
        {
            foreach (var item in _InlayGrooveIndex2GemIds)
            {
                _SelectCoreItemIcon.ClearGrooveGemItem(item.Key);
                CreateBagGemIconItem(item.Value).Forget();
            }
            _InlayGrooveIndex2GemIds.Clear();
        }
        UIMgr.Instance.UnloadWindowItem(_SelectCoreItemIcon);
        CreateBagCoreIconItem(coreId).Forget();
        _SelectCoreId = -1;
        _SelectCoreItemIcon = null;
        UpdateButtonStatus();
        UpdateCoreInfos();
    }
    private void SelectCore(int coreId)
    {
        if (_SelectCoreId > 0)
        {
            UnselectCore(_SelectCoreId);
        }
        DestroyBagCoreIconItem(coreId);
        _SelectCoreId = coreId;
        LoadSelectCoreItem(coreId);
        UpdateButtonStatus();
        UpdateCoreInfos();
    }
    private void UninlayGem(int grooveIndex, int gemId)
    {
        _SelectCoreItemIcon.ClearGrooveGemItem(grooveIndex);
        CreateBagGemIconItem(gemId).Forget();
        _InlayGrooveIndex2GemIds.Remove(grooveIndex);
        UpdateButtonStatus();
        UpdateCoreInfos();
    }
    private bool InlayGem(int gemId)
    {
        var curSelCoreGrooveCount = UICoreUtility.GetCoreGemGrooveCount(_SelectCoreId);
        var gemType = UIGemUtility.GetGemType(gemId);
        var grooveIndex = -1;
        for (int i = 0; i < curSelCoreGrooveCount; i++)
        {
            if (_InlayGrooveIndex2GemIds.ContainsKey(i))
                continue;
            if (UICoreUtility.ContainsGem(_SelectCoreId, i))
                continue;
            var coreGemType = UICoreUtility.GetCoreGrooveGemType(_SelectCoreId, i);
            if (coreGemType != gemType)
                continue;
            grooveIndex = i;
            break;
        }
        if (grooveIndex < 0)
        {
            UIUtil.ShowHitTips(30101);
            return false;
        }

        if (_InlayGrooveIndex2GemIds.TryGetValue(grooveIndex, out var alreadyGemId))
        {
            UninlayGem(grooveIndex, alreadyGemId);
        }
        _SelectCoreItemIcon.SetGrooveGemId(grooveIndex, gemId, () => UninlayGem(grooveIndex, gemId));

        _InlayGrooveIndex2GemIds.Add(grooveIndex, gemId);
        DestroyBagGemIconItem(gemId);
        UpdateButtonStatus();
        UpdateCoreInfos();
        return true;
    }
    private async UniTask CreateBagGemIconItem(int gemId)
    {
        var (isCancel, item) = await UIMgr.Instance.LoadWindowItemAsync<UIGoodsGemItemIcon>(EnLoadTarget.Pre_UIGoodsGemItemIcon, _GemItemRootRect, GetWindowToken())
            .SuppressCancellationThrow();
        if (isCancel)
            throw new System.OperationCanceledException();
        item.SettingGemId(gemId, () => OnClick_BagGemIconItem(gemId), EnSourceId.UIHallInlayWindow);
        _GemIconItems.Add(gemId, item);
    }
    private void DestroyBagGemIconItem(int gemId)
    {
        var iconItem = _GemIconItems[gemId];
        _GemIconItems.Remove(gemId);
        UIMgr.Instance.UnloadWindowItem(iconItem);
    }
    private async UniTask CreateBagCoreIconItem(int coreId)
    {
        var (isCancel, item) = await UIMgr.Instance.LoadWindowItemAsync<UIGoodsCoreItemIcon>(EnLoadTarget.Pre_UIGoodsCoreItemIcon, _CoreItemRootRect, GetWindowToken())
            .SuppressCancellationThrow();
        if (isCancel)
            throw new System.OperationCanceledException();
        item.SettingCoreId(coreId, () => SelectCore(coreId), EnSourceId.UIHallInlayWindow);
        _CoreIconItems.Add(coreId, item);
    }
    private void DestroyBagCoreIconItem(int coreId)
    {
        var iconItem = _CoreIconItems[coreId];
        _CoreIconItems.Remove(coreId);
        UIMgr.Instance.UnloadWindowItem(iconItem);
    }

    private void RefreshCoreSort()
    {
        _TempIntList.Clear();
        foreach (var item in _CoreIconItems)
        {
            if (_SelectCoreId == item.Key)
                continue;
            _TempIntList.Add(item.Key);
        }
        UICoreUtility.SortList(ref _TempIntList);
        for (int i = 0; i < _TempIntList.Count; i++)
        {
            var id = _TempIntList[i];
            var iconItem = _CoreIconItems[id];
            iconItem.SetSiblingIndex(i);
        }
    }
    private void RefreshGemSort()
    {
        _TempIntList.Clear();
        _TempIntHashSet.Clear();
        foreach (var item in _InlayGrooveIndex2GemIds)
            _TempIntHashSet.Add(item.Value);

        foreach (var item in _GemIconItems)
        {
            if (_TempIntHashSet.Contains(item.Key))
                continue;
            _TempIntList.Add(item.Key);
        }
        UIGemUtility.SortList(ref _TempIntList);
        for (int i = 0; i < _TempIntList.Count; i++)
        {
            var id = _TempIntList[i];
            var iconItem = _GemIconItems[id];
            iconItem.SetSiblingIndex(i);
        }
    }
    private void ResolveGem(HashSet<int> list)
    {
        _TempIntList.Clear();
        _TempIntHashSet.Clear();
        foreach (var item in _InlayGrooveIndex2GemIds)
            _TempIntHashSet.Add(item.Value);
        UIGemMgr.Instance.GetAleadyGemIds(ref _TempIntList);
        for (int i = 0; i < _TempIntList.Count; i++)
        {
            var gemId = _TempIntList[i];
            if (UIGemMgr.Instance.GetGemInlayStatus(gemId))
                continue;
            if (_TempIntHashSet.Contains(gemId))
                continue;
            var quality = UIGemUtility.GetGemQuality(gemId);
            if (!list.Contains((int)quality - 1))
                continue;
            UIGemMgr.Instance.RemoveGem(gemId);
        }
    }
}
