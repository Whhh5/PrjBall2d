using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHallMainCoreWindow_CoreFrameItem : UIWindowItem
{
    [SerializeField, AutoReference]
    private Image _FrameIconImg;

    private EnCoreType _CoreType;
    public override void Unload()
    {
        _FrameIconImg.UnloadSprite();
        base.Unload();
        _CoreType = EnCoreType.None;
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();
    }

    public void Setting(EnCoreType type)
    {
        _CoreType = type;
        SetStatus(false);
    }
    public void SetLocalPos(in Vector2 localPos)
    {
        _Rect.anchoredPosition = localPos;
    }
    public void SetStatus(bool status)
    {
        var cfg = GameSchedule.Instance.GetCoreTypeCfg0((int)_CoreType);
        var assetId = status ? cfg.nActiveGrooveAssetId : cfg.nGrooveAssetId;
        _FrameIconImg.LoadSprirte(assetId);
    }
}
