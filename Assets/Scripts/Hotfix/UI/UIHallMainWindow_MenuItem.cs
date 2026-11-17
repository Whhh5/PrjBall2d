using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHallMainWindow_MenuItem : UIWindowItem
{
    [SerializeField, AutoReference(nameof(_IconImg))]
    private RectTransform _IconRect;
    [SerializeField, AutoReference]
    private Image _IconImg;
    [SerializeField, AutoReference]
    private GameObject _LockGo;

    private float _MenuItemSize;
    private EnUIHallMainMenuItemId _ItemId;
    private Vector2 _IconSize;
    public override void Unload()
    {
        _IconRect.sizeDelta = _IconSize;
        _IconImg.UnloadSprite();
        base.Unload();
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();
        _IconSize = _IconRect.sizeDelta;

        _MenuItemSize = _Rect.rect.width;
    }
    public void Init(EnUIHallMainMenuItemId itemId, float angle)
    {
        _ItemId = itemId;
        _Rect.localEulerAngles = Vector3.forward * angle;
        _IconRect.localEulerAngles = -Vector3.forward * angle;

        var icon = UIHallMainUtility.GetIconId(itemId);
        _IconImg.LoadSprirte(icon);
        var isLock = UIHallMainUtility.IsLock(itemId);
        _LockGo.SetActive(isLock);
    }
    public float GetUnitSize()
    {
        return _MenuItemSize;
    }
    public override void SetLocalScale(float scale)
    {
        base.SetLocalScale(scale);

        _IconRect.sizeDelta = (0.1f + (0.9f * scale)) * _IconSize;

        //var iconPos = _IconRect.anchoredPosition;
        //iconPos.y = (scale - 0.3f)
    }
}
