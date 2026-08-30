using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHallMainWindow_MenuItem : UIWindowItem
{
    [SerializeField, AutoReference(SearchScope.Self)]
    private RectTransform _Rect;
    [SerializeField, AutoReference]
    private Image _IconImg;
    [SerializeField, AutoReference]
    private GameObject _LockGo;

    private float _MenuItemSize;
    private EnUIHallMainMenuItemId _ItemId;
    public override void Unload()
    {
        _IconImg.UnloadSprite();
        base.Unload();
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();

        _MenuItemSize = _Rect.rect.width;
    }
    public void Init(EnUIHallMainMenuItemId itemId, float angle)
    {
        _ItemId = itemId;
        _Rect.localEulerAngles = Vector3.forward * angle;

        var icon = UIHallMainUtility.GetIconId(itemId);
        _IconImg.LoadSprirte(icon);
        var isLock = UIHallMainUtility.IsLock(itemId);
        _LockGo.SetActive(isLock);
    }
    public float GetUnitSize()
    {
        return _MenuItemSize;
    }
    public void SetScale(float size)
    {
        _Rect.localScale = Vector3.one * size;
    }
    public float GetScale()
    {
        return _Rect.localScale.x;
    }
}
