using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UIBattleUplevelWindowItemData : IClassPoolDestroy
{
    public EnUIBattleUplevelWindowItemType itemType;
    public IUIBattleUplevelWindow_ItemUserData userData;
    public void OnPoolDestroy()
    {
        if (userData != null)
            ClassPoolMgr.Instance.Push(userData);
        itemType = EnUIBattleUplevelWindowItemType.None;
        userData = null;
    }
}

public enum EnUIBattleUplevelWindowItemType
{
    None,
    SkillBall,
    SkillPassivity,
    SkillBallFussion,
    SkillBallPart,
    Attribute,
    EnumCount,
}
public interface IUIBattleUplevelWindowItem
{
    public List<DescriptionInfo> GetDescriptionList(EnDescriptionLevel startLevel, EnDescriptionLevel endLevel);
    public void Select();
    public Vector3 GetWorldPos();
    public float GetSizeDeltaY();
}
public class UIBattleUplevelWindowUserData : UIWindowUserData
{
    public List<UIBattleUplevelWindowItemData> itemDataList = null;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        itemDataList = null;
    }
}
public class UIBattleUplevelWindowData : UIWindowData<UIBattleUplevelWindow, UIBattleUplevelWindowUserData>
{
    public override EnUIWindowType WindowType => EnUIWindowType.Window;

    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIBattleUplevelWindow;

    private readonly List<UIBattleUplevelWindowItemData> _ItemDataList = new();
    private CancellationTokenSource _TokenSource = null;

    protected override void OnPoolInit(UIBattleUplevelWindowUserData userData)
    {
        if (userData.itemDataList != null)
            _ItemDataList.AddRange(userData.itemDataList);
    }
    public override void OnPoolDestroy()
    {
        foreach (var item in _ItemDataList)
        {
            ClassPoolMgr.Instance.Push(item);
        }
        base.OnPoolDestroy();
        _ItemDataList.Clear();
    }
    public override void OnGODestroy()
    {
        _TokenSource.Cancel();
        base.OnGODestroy();
        _TokenSource = null;
    }
    public override void OnGOCreate(GameEntity entity)
    {
        base.OnGOCreate(entity);

        _TokenSource = new();

        _UIWindow.Setting(_ItemDataList, _TokenSource.Token);
    }
    public List<UIBattleUplevelWindowItemData> CopyItemData()
    {
        var list = new List<UIBattleUplevelWindowItemData>(_ItemDataList.Count);
        for (int i = 0; i < _ItemDataList.Count; i++)
        {
            var item = _ItemDataList[i];
            var itemData = ClassPoolMgr.Instance.Pull<UIBattleUplevelWindowItemData>();
            itemData.itemType = item.itemType;
            itemData.userData = item.userData.Copy();
            list.Add(itemData);
        }
        return list;
    }
}
public class UIBattleUplevelWindow : UIWindow
{
    [SerializeField]
    private Button _CloseBtn = null;
    [SerializeField]
    private RectTransform _SelectScrollRootRect = null;

    private readonly List<IUIBattleUplevelWindowItem> _SelectItemEntitys = new();
    public override void OnUnload()
    {
        ClearItemEntitys();

        _CloseBtn.RemoveListener(OnClick_CloseBtn);
        base.OnUnload();
    }
    private void ClearItemEntitys()
    {
        for (int i = 0; i < _SelectItemEntitys.Count; i++)
        {
            var item = _SelectItemEntitys[i];
            UnregisterUIDissolve(item);
            UIMgr.Instance.UnloadWindowItem(item);
        }
        _SelectItemEntitys.Clear();
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();

        _CloseBtn.AddListener(OnClick_CloseBtn);
    }

    public async void Setting(List<UIBattleUplevelWindowItemData> itemDatas, CancellationToken token)
    {
        for (int i = 0; i < itemDatas.Count; i++)
        {
            var index = i;
            var itemData = itemDatas[index];
            var (isCancel, item) = await CreateItem(i, itemData.itemType, itemData.userData, token)
                .SuppressCancellationThrow();
            if (isCancel)
                break;
            _SelectItemEntitys.Add(item);
            RegisterUIDissolve(item);
        }
    }

    private void OnClick_CloseBtn()
    {
        UIMgr.Instance.HideWindow(_WindowId);
    }

    private void OnClick_Item(int index)
    {
        var itemEnttiy = _SelectItemEntitys[index];
        itemEnttiy.Select();
        UIMgr.Instance.HideWindow(_WindowId, EnWindowHideType.SelectItem);
    }

    private async UniTask<IUIBattleUplevelWindowItem> CreateItem(int index, EnUIBattleUplevelWindowItemType type, IClassPool userData, CancellationToken token)
    {
        switch (type)
        {
            case EnUIBattleUplevelWindowItemType.SkillBall:
                {
                    var (isCencel, entityItem) = await UIMgr.Instance.LoadWindowItemAsync<UIBattleUplevelWindow_SkillBallItem>(EnLoadTarget.Pre_UIBattleUplevelWindow_SkillBallItem, _SelectScrollRootRect, token)
                        .SuppressCancellationThrow();
                    if (isCencel)
                        throw new OperationCanceledException();
                    var data = userData as UIBattleUplevelWindow_SkillBallItemUserData;
                    entityItem.Setting(data.ballId, data.star, () => OnClick_Item(index));
                    return entityItem;
                }
            case EnUIBattleUplevelWindowItemType.SkillBallPart:
                {
                    var (isCencel, entityItem) = await UIMgr.Instance.LoadWindowItemAsync<UIBattleUplevelWindow_SkillBallPartItem>(EnLoadTarget.Pre_UIBattleUplevelWindow_SkillBallPartItem, _SelectScrollRootRect, token)
                        .SuppressCancellationThrow();
                    if (isCencel)
                        throw new OperationCanceledException();
                    var data = userData as UIBattleUplevelWindow_SkillBallPartItemUserData;
                    entityItem.Setting(data.partId, data.star, () => OnClick_Item(index));
                    return entityItem;
                }
            case EnUIBattleUplevelWindowItemType.Attribute:
                {
                    var (isCencel, entityItem) = await UIMgr.Instance.LoadWindowItemAsync<UIBattleUplevelWindow_AttributeItem>(EnLoadTarget.Pre_UIBattleUplevelWindow_AttributeItem, _SelectScrollRootRect, token)
                        .SuppressCancellationThrow();
                    if (isCencel)
                        throw new OperationCanceledException();
                    var data = userData as UIBattleUplevelWindow_AttributeItemUserData;
                    entityItem.Setting(data.attId, data.star, data.addValue, data.valueType, () => OnClick_Item(index));
                    return entityItem;
                }
            case EnUIBattleUplevelWindowItemType.SkillBallFussion:
                {
                    var (isCencel, entityItem) = await UIMgr.Instance.LoadWindowItemAsync<UIBattleUplevelWindow_SkillBallFussionItem>(EnLoadTarget.Pre_UIBattleUplevelWindow_SkillBallFussionItem, _SelectScrollRootRect, token)
                        .SuppressCancellationThrow();
                    if (isCencel)
                        throw new OperationCanceledException();
                    var data = userData as UIBattleUplevelWindow_SkillBallFussionItemUserData;
                    entityItem.Setting(data.fussionId, () => OnClick_Item(index));
                    return entityItem;
                }
            case EnUIBattleUplevelWindowItemType.SkillPassivity:
                {
                    var (isCencel, entityItem) = await UIMgr.Instance.LoadWindowItemAsync<UIBattleUplevelWindow_SkillPassivityItem>(EnLoadTarget.Pre_UIBattleUplevelWindow_SkillPassivityItem, _SelectScrollRootRect, token)
                        .SuppressCancellationThrow();
                    if (isCencel)
                        throw new OperationCanceledException();
                    var data = userData as UIBattleUplevelWindow_SkillPassivityItemUserData;
                    entityItem.Setting(data.skillPassivityId, data.star, () => OnClick_Item(index));
                    return entityItem;
                }
            default:
                throw new Exception($"{type} create error");
        }
    }
}
