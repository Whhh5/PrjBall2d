using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGemGrooveItem : UIWindowItem
{
    [SerializeField, AutoReference]
    private Button _Btn;
    [SerializeField, AutoReference]
    private Image _IconImg;
    [SerializeField, AutoReference]
    private RectTransform _GemIconRootRect;

    private UIGoodsGemItemIcon _IconItem;
    private EnSourceId _SourceId;
    private Action _OnClick;
    public override void Unload()
    {
        ClearInlayGemItem();
        _Btn.RemoveListener(OnClick_Btn);
        _IconImg.UnloadSprite();
        base.Unload();
        _SourceId = EnSourceId.None;
        _OnClick = null;
    }

    public void Setting(EnGemTypeId gemType, Action click, EnSourceId sourceId)
    {
        _SourceId = sourceId;
        var gemTypeCfg = GameSchedule.Instance.GetGemTypeCfg0((int)gemType);
        _IconImg.LoadSprirte(gemTypeCfg.nGrooveAssetId);

        _Btn.AddListener(OnClick_Btn);
        _Btn.interactable = click != null;
        _Btn.targetGraphic.raycastTarget = click != null;
        _OnClick = click;
    }
    private void OnClick_Btn()
    {
        _OnClick.Invoke();
    }

    public void ClearInlayGemItem()
    {
        if (_IconItem == null)
            return;
        UIMgr.Instance.UnloadWindowItem(_IconItem);
        _IconItem = null;
    }
    public async void SetInlayGemId(int gemId, Action click)
    {
        ClearInlayGemItem();
        var (isCancel, item) = await UIGoodsUtility.CreateGoodsItemIcon<UIGoodsGemItemIcon>(EnLoadTarget.Pre_UIGoodsGemItemIcon, _GemIconRootRect, Token)
            .SuppressCancellationThrow();
        if (!isCancel)
        {
            item.SettingGemId(gemId, click, _SourceId);
            _IconItem = item;
        }
    }
}
