using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHallMainCoreWindow_CoreFrameItem : UIWindowItem
{
    [SerializeField, AutoReference(SearchScope.Self)]
    private RectTransform _Rect;
    [SerializeField, AutoReference]
    private Image _FrameIconImg;

    public override void Unload()
    {
        base.Unload();
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();
    }

    public void Init(EnCoreType type)
    {
        var cfg = GameSchedule.Instance.GetCoreTypeCfg0((int)type);
        if (ColorUtility.TryParseHtmlString(cfg.strColor, out var col))
        {
            _FrameIconImg.color = col;
        }
    }
    public void SetLocalPos(in Vector2 localPos)
    {
        _Rect.anchoredPosition = localPos;
    }
    public Vector3 GetWorldPos()
    {
        return _Rect.position;
    }
}
