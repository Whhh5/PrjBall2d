using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHallMainWindowData : UIWindowData
{
    public override EnUIWindowType WindowType => EnUIWindowType.Window;

    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIHallMainWindow;
}
public enum EnUIHallMainMenuItemId
{
    None,
    Battle,
    Core,
    Shop,
    Inlay,
    Character,
    Item2,
    EnumCount,
}

public class UIHallMainWindow : UIWindow
{
    [SerializeField, AutoReference]
    private RectTransform _MenuRect;
    [SerializeField, AutoReference]
    private RectTransform _GoodsBarRoorRect;
    [SerializeField, AutoReference]
    private RectTransform _MenuWheelRect;
    [SerializeField, AutoReference]
    private RectTransform _MenuItemRootRect;
    [SerializeField, AutoReference]
    private RectTransform _SubWindowRootRect;
    [SerializeField, AutoReference]
    private Image _MenuSelectIconImg;
    [SerializeField, AutoReference(nameof(_MenuSelectIconImg))]
    private RectTransform _MenuSelectIconRect;

    private float _MenuWheelRadiusSqr;
    private float _MenuWheelSize;
    private bool _IsWheelUpdate = false;

    private EnUIHallMainMenuItemId _CurSelectItemId = EnUIHallMainMenuItemId.None;
    private readonly Dictionary<EnUIHallMainMenuItemId, UIHallMainWindow_MenuItem> _MenuItemId2WindowItem = new();
    private readonly List<EnUIHallMainMenuItemId> _MenuItemIds = new();
    private readonly float _MenuItemAngleUnit = 60;
    private int _CurSelectWindowId = -1;

    private int _GoodsBarWindowId = -1;
    public override void OnUnload()
    {
        UIMgr.Instance.HideWindow(_GoodsBarWindowId);
        foreach (var item in _MenuItemId2WindowItem)
        {
            UIMgr.Instance.UnloadWindowItem(item.Value);
        }
        base.OnUnload();
        if (_CurSelectItemId >= 0)
        {
            UIMgr.Instance.HideWindow(_CurSelectWindowId);
            _MenuSelectIconImg.UnloadSprite();
        }
        EventMgr.Instance.Unregister(EnABBEvent.EVENT_HALLMAIN_CHANGE_PAGE, OnEvent_ChangePage);
        _MenuItemId2WindowItem.Clear();
        _MenuItemIds.Clear();
        _CurSelectItemId = EnUIHallMainMenuItemId.None;
        _IsWheelUpdate = false;
        _CurSelectWindowId = -1;
        _GoodsBarWindowId = -1;
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();
        EventMgr.Instance.Register(EnABBEvent.EVENT_HALLMAIN_CHANGE_PAGE, OnEvent_ChangePage);

        AudioMgr.Instance.Play(EnAudioId.HallBgm);
        _MenuWheelSize = _MenuRect.rect.width;
        _MenuWheelRadiusSqr = Mathf.Pow(_MenuWheelSize / 2 * UIMgr.Instance.GetCanvasScale(), 2);

        UpdateMenuItems();
        SetSelectItemId(EnUIHallMainMenuItemId.None, EnUIHallMainMenuItemId.Battle);

        _GoodsBarWindowId = UIMgr.Instance.ShowWindow<UIHallGoodsBarWindowData>(_GoodsBarRoorRect);
    }
    private void OnEvent_ChangePage(in int _, in int pageId)
    {
        var id = (EnUIHallMainMenuItemId)pageId;
        if (_CurSelectItemId == id)
            return;

        SetSelectItemId(_CurSelectItemId, id);
    }

    private async void UpdateMenuItems()
    {
        var itemIdList = UIHallMainUtility.GetMenuItemList();
        for (int i = 0; i < itemIdList.Count; i++)
        {
            var itemId = itemIdList[i];
            var (isCancel, item) = await UIMgr.Instance.LoadWindowItemAsync<UIHallMainWindow_MenuItem>(EnLoadTarget.Pre_UIHallMainWindow_MenuItem, _MenuItemRootRect, GetWindowToken())
                .SuppressCancellationThrow();
            if (isCancel)
                break;
            _MenuItemIds.Add(itemId);
            _MenuItemId2WindowItem.Add(itemId, item);
            var angle = i * _MenuItemAngleUnit;
            item.Init(itemId, angle);
        }
    }

    protected override void Update()
    {
        base.Update();

        if (!UIMgr.Instance.IsTop(_WindowId, EnUIWindowType.Window))
            return;

        var mousePos = GameUtility.GetMousePos();
        var menuRectWorldPos = (Vector2)_MenuRect.position;
        var mouseWorldPos = UIUtil.UIViewToWorld(mousePos);
        var disSqr = Vector2.SqrMagnitude(menuRectWorldPos - mouseWorldPos);
        if (Input.GetMouseButtonDown(0) && (disSqr < _MenuWheelRadiusSqr))
        {
            _IsWheelUpdate = true;
        }
        var isMouseInWheel = disSqr < Mathf.Pow(_MenuWheelRect.sizeDelta.x / 2 * UIMgr.Instance.GetCanvasScale(), 2);

        WheelUpdate(isMouseInWheel);
        if (_IsWheelUpdate && !isMouseInWheel)
            WheelSelectUpdate(in mouseWorldPos, in menuRectWorldPos);

        WheelItemUpdate();

        if (Input.GetMouseButtonUp(0))
        {
            _IsWheelUpdate = false;
        }
    }
    private float GetAnimationSpeed()
    {
        return ABBUtil.GetTimeDelta() * 10;
    }
    private void WheelSelectUpdate(in Vector2 mouseWorldPos, in Vector2 centerWorldPos)
    {
        if (_MenuItemIds.Count == 0)
            return;
        var intervalRad = _MenuItemAngleUnit / 360 * Mathf.PI * 2;
        var curDir = (mouseWorldPos - centerWorldPos).normalized;
        var rad = Mathf.Atan2(curDir.y, curDir.x);
        rad = rad - Mathf.PI / 2 + _MenuItemAngleUnit / 2 / 180 * Mathf.PI;
        if (rad < 0)
            rad = Mathf.PI * 2 + rad;

        var selectIndex = Mathf.FloorToInt(rad / intervalRad);
        if (selectIndex >= _MenuItemIds.Count)
            return;
        var itemId = _MenuItemIds[selectIndex];
        var itemIsLock = UIHallMainUtility.IsLock(itemId);
        if (itemIsLock)
            return;
        if (_CurSelectItemId == itemId)
            return;

        SetSelectItemId(_CurSelectItemId, itemId);
    }
    private void WheelItemUpdate()
    {
        for (int i = 0; i < _MenuItemIds.Count; i++)
        {
            var itemId = _MenuItemIds[i];
            var item = _MenuItemId2WindowItem[itemId];
            var scale = _IsWheelUpdate
                ? _CurSelectItemId == itemId ? 1 : 0.8f
                : 0.3f;
            var curSize = item.GetLocalScale();
            curSize = Mathf.Lerp(curSize, scale, GetAnimationSpeed());
            item.SetLocalScale(curSize);
        }
    }
    private void WheelUpdate(bool isMouseInWheel)
    {
        var menuPosY = 0f;
        var menuWheelSize = _MenuWheelSize;

        var sltIconPosY = _MenuWheelSize / 2 / 2;
        var sltIconSize = _MenuWheelRect.sizeDelta.x / 3;
        if (_IsWheelUpdate)
        {
            sltIconPosY = 0;
            menuPosY = _MenuWheelSize / 2 * 1.5f;
            menuWheelSize = _MenuWheelSize / 2 * (isMouseInWheel ? 1.2f : 1f);
            sltIconSize = menuWheelSize / 2;
        }
        {
            var curPos = _MenuRect.anchoredPosition;
            curPos.y = Mathf.Lerp(curPos.y, menuPosY, GetAnimationSpeed());
            _MenuRect.anchoredPosition = curPos;
        }
        {
            var curSize = _MenuWheelRect.sizeDelta;
            curSize.x = curSize.y = Mathf.Lerp(curSize.x, menuWheelSize, GetAnimationSpeed());
            _MenuWheelRect.sizeDelta = curSize;
        }
        {
            var curSltIconPos = _MenuSelectIconRect.anchoredPosition;
            curSltIconPos.y = Mathf.Lerp(curSltIconPos.y, sltIconPosY, GetAnimationSpeed());
            _MenuSelectIconRect.anchoredPosition = curSltIconPos;
        }
        {
            var curSize = _MenuSelectIconRect.sizeDelta;
            curSize.x = curSize.y = Mathf.Lerp(curSize.x, sltIconSize, GetAnimationSpeed());
            _MenuSelectIconRect.sizeDelta = curSize;
        }
    }

    private void SetSelectItemId(EnUIHallMainMenuItemId lastItemId, EnUIHallMainMenuItemId itemId)
    {
        if (lastItemId != EnUIHallMainMenuItemId.None)
        {
            UIMgr.Instance.HideWindow(_CurSelectWindowId);
        }
        var windowId = UIHallMainUtility.GetWindowId(itemId);
        _CurSelectWindowId = UIMgr.Instance.ShowWindow(windowId, _SubWindowRootRect);
        _CurSelectItemId = itemId;
        var icon = UIHallMainUtility.GetIconId(itemId);
        _MenuSelectIconImg.LoadSprirte(icon);
    }
}
