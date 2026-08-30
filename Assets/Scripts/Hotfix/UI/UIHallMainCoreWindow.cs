using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UIHallMainCoreWindowData : UIWindowData
{
    public override EnUIWindowType WindowType => EnUIWindowType.SubWindow;

    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIHallMainCoreWindow;
}
public class UIHallMainCoreWindow : UIWindow
{
    [SerializeField, AutoReference]
    private RectTransform _CoreItemRootRect;
    [SerializeField, AutoReference]
    private RectTransform _BagCoreItemRootRect;
    [SerializeField, AutoReference]
    private Button _SortBtn;
    [SerializeField, AutoReference]
    private Button _ResolveBtn;


    private readonly Vector2 _CoreItemInterval = new(180, 160);
    private readonly int _TypeUnitCount = 5;

    private readonly Dictionary<EnCoreType, List<UIHallMainCoreWindow_CoreFrameItem>> _Type2Frames = new();
    private readonly Dictionary<int, UIGoodsCoreItemIcon> _CoreItems = new();

    public override void OnUnload()
    {
        foreach (var item in _Type2Frames)
        {
            foreach (var frame in item.Value)
            {
                UIMgr.Instance.UnloadWindowItem(frame);
            }
        }
        foreach (var item in _CoreItems)
        {
            UIMgr.Instance.UnloadWindowItem(item.Value);
        }

        _SortBtn.RemoveListener(OnClick_SortBtn);
        _ResolveBtn.RemoveListener(OnClick_ResolveBtn);
        EventMgr.Instance.Unregister<GameCoreAddEventData>(EnABBEvent.EVENT_CORE_ADD, 0, 0, OnEvent_CoreAdd);
        EventMgr.Instance.Unregister<GameCoreRemoveEventData>(EnABBEvent.EVENT_CORE_REMOVE, 0, 0, OnEvent_CoreRemove);
        EventMgr.Instance.Unregister<GameCoreWearEventData>(EnABBEvent.EVENT_CORE_WEAR, 0, 0, OnEvent_CoreWear);
        EventMgr.Instance.Unregister<GameCoreUnwearEventData>(EnABBEvent.EVENT_CORE_UNWEAR, 0, 0, OnEvent_CoreUnwear);
        base.OnUnload();
        _Type2Frames.Clear();
        _CoreItems.Clear();
    }
    public override async void LoadCompletion()
    {
        base.LoadCompletion();


        EventMgr.Instance.Register<GameCoreAddEventData>(EnABBEvent.EVENT_CORE_ADD, 0, 0, OnEvent_CoreAdd);
        EventMgr.Instance.Register<GameCoreRemoveEventData>(EnABBEvent.EVENT_CORE_REMOVE, 0, 0, OnEvent_CoreRemove);
        EventMgr.Instance.Register<GameCoreWearEventData>(EnABBEvent.EVENT_CORE_WEAR, 0, 0, OnEvent_CoreWear);
        EventMgr.Instance.Register<GameCoreUnwearEventData>(EnABBEvent.EVENT_CORE_UNWEAR, 0, 0, OnEvent_CoreUnwear);
        _SortBtn.AddListener(OnClick_SortBtn);
        _ResolveBtn.AddListener(OnClick_ResolveBtn);

        if (!await CreateCoreFrameAsync().SuppressCancellationThrow())
            if (!await CreateCoreItemAsync().SuppressCancellationThrow())
                if (!await CreateBagCoreItemAsync().SuppressCancellationThrow())
                    return;
    }
    private void OnClick_SortBtn()
    {
        UpdateSort();
    }
    private void OnClick_ResolveBtn()
    {
        var selectList = new List<string>();
        for (var i = EnCoreQuality.None + 1; i < EnCoreQuality.EnumCount; i++)
        {
            var name = LanguageUtility.GetLanguage_CoreQualityName(i);
            selectList.Add(name);
        }
        var windowUserData = ClassPoolMgr.Instance.Pull<UICoreQualitySelectDialogUserData>();
        windowUserData.Setting(selectList, ResolveCore);
        UIMgr.Instance.ShowWindow<UICoreQualitySelectDialogData>(windowUserData);
        ClassPoolMgr.Instance.Push(windowUserData);
    }

    private void OnEvent_CoreAdd(in int _, in int __, in GameCoreAddEventData data)
    {
        CreateBagCoreItem(data.coreId).Forget();
    }
    private void OnEvent_CoreRemove(in int _, in int __, in GameCoreRemoveEventData data)
    {
        DestroyCoreItem(data.coreId);
    }
    private void OnEvent_CoreWear(in int _, in int __, in GameCoreWearEventData data)
    {
        var item = _CoreItems[data.coreId];
        item.SetParent(_CoreItemRootRect);
        SetWearItemPos(data.coreId);
    }
    private void OnEvent_CoreUnwear(in int _, in int __, in GameCoreUnwearEventData data)
    {
        var item = _CoreItems[data.coreId];
        item.SetParent(_BagCoreItemRootRect);
    }

    private async UniTask CreateCoreFrameAsync()
    {
        var posXOffset = -((int)EnCoreType.Count - 1 - 1) / 2 * _CoreItemInterval.x;
        for (var i = EnCoreType.None + 1; i < EnCoreType.Count; i++)
        {
            var list = new List<UIHallMainCoreWindow_CoreFrameItem>(_TypeUnitCount);
            _Type2Frames.Add(i, list);
            var posX = ((int)i - 1) * _CoreItemInterval.x + posXOffset;
            for (int j = 0; j < _TypeUnitCount; j++)
            {
                var posY = j * _CoreItemInterval.y;
                var (isCancel, item) = await UIMgr.Instance.LoadWindowItemAsync<UIHallMainCoreWindow_CoreFrameItem>(EnLoadTarget.Pre_UIHallMainCoreWindow_CoreFrameItem, _CoreItemRootRect, GetWindowToken())
                    .SuppressCancellationThrow();
                if (isCancel)
                    throw new System.OperationCanceledException();
                item.Init(i);
                item.SetLocalPos(new(posX, posY));
                list.Add(item);
            }
        }
    }
    private async UniTask CreateCoreItemAsync()
    {
        var wearCoreIds = new List<int>(_TypeUnitCount * (int)EnCoreType.Count);
        UICoreMgr.Instance.GetWearCoreIds(ref wearCoreIds);
        for (int i = 0; i < wearCoreIds.Count; i++)
        {
            var coreId = wearCoreIds[i];
            var (isCancel, item) = await UIMgr.Instance.LoadWindowItemAsync<UIGoodsCoreItemIcon>(EnLoadTarget.Pre_UIGoodsCoreItemIcon, _CoreItemRootRect, GetWindowToken())
                .SuppressCancellationThrow();
            if (isCancel)
                throw new System.OperationCanceledException();
            item.SettingCoreId(coreId, EnSourceId.UIHallMainCoreWindow);
            _CoreItems.Add(coreId, item);
            SetWearItemPos(coreId);
        }
    }
    private void SetWearItemPos(int coreId)
    {
        var item = _CoreItems[coreId];
        var wearIndex = UICoreMgr.Instance.GetWearIndex(coreId);
        var coreType = UICoreUtility.GetCoreType(coreId);
        var worldPos = GetCoreFrameWorldPos(coreType, wearIndex);
        item.SetWorldPos(worldPos);
    }
    private async UniTask CreateBagCoreItemAsync()
    {
        var wearCoreIds = new List<int>(100);
        UICoreMgr.Instance.GetBagCoreIds(ref wearCoreIds);
        UICoreUtility.SortList(ref wearCoreIds);
        for (int i = 0; i < wearCoreIds.Count; i++)
        {
            var coreId = wearCoreIds[i];
            var isCancel = await CreateBagCoreItem(coreId)
                .SuppressCancellationThrow();
            if (isCancel)
                throw new System.OperationCanceledException();
            var result = await UniTask.DelayFrame(1, cancelImmediately: true, cancellationToken: GetWindowToken())
                .SuppressCancellationThrow();
            if (result)
                throw new System.OperationCanceledException();
        }
    }

    private async UniTask CreateBagCoreItem(int coreId)
    {
        var (isCancel, item) = await UIMgr.Instance.LoadWindowItemAsync<UIGoodsCoreItemIcon>(EnLoadTarget.Pre_UIGoodsCoreItemIcon, _BagCoreItemRootRect, GetWindowToken())
            .SuppressCancellationThrow();
        if (isCancel)
            throw new System.OperationCanceledException();
        item.SettingCoreId(coreId, EnSourceId.UIHallMainCoreWindow);
        _CoreItems.Add(coreId, item);
    }

    private void DestroyCoreItem(int coreId)
    {
        if (!_CoreItems.TryGetValue(coreId, out var item))
            return;
        _CoreItems.Remove(coreId);
        UIMgr.Instance.UnloadWindowItem(item);
    }

    public Vector3 GetCoreFrameWorldPos(EnCoreType type, int index)
    {
        if (!_Type2Frames.TryGetValue(type, out var indexList))
            return Vector3.zero;
        var item = indexList[index];
        var worldPos = item.GetWorldPos();
        return worldPos;
    }

    private void OnClick_CoreItem(int coreId)
    {
        var windowUserData = ClassPoolMgr.Instance.Pull<UICoreInfoDialogUserData>();
        windowUserData.SettingCoreId(coreId, EnSourceId.UIHallMainCoreWindow);
        UIMgr.Instance.ShowWindow<UICoreInfoDialogData>(windowUserData);
        ClassPoolMgr.Instance.Push(windowUserData);
    }

    private void UpdateSort()
    {
        var list = new List<int>(100);
        UICoreMgr.Instance.GetBagCoreIds(ref list);
        UICoreUtility.SortList(ref list);
        for (int i = 0; i < list.Count; i++)
        {
            var coreId = list[i];
            var item = _CoreItems[coreId];
            item.SetSiblingIndex(i);
        }
    }
    private void ResolveCore(HashSet<int> qualitys)
    {
        var list = new List<int>(100);
        UICoreMgr.Instance.GetBagCoreIds(ref list);
        for (int i = 0; i < list.Count; i++)
        {
            var coreId = list[i];
            var quality = UICoreUtility.GetCoreQuality(coreId);
            if (!qualitys.Contains((int)quality - 1))
                continue;
            UICoreMgr.Instance.RemoveCore(coreId);
        }
    }
}