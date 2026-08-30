using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHallGoodsBarWindow_GoodsItem : UIWindowItem
{
    [SerializeField, AutoReference]
    private RectTransform _IconRootRect;
    [SerializeField, AutoReference]
    private TextMeshProUGUI _CountTxt;

    private UIGoodsItemIconBase _IconItem = null;
    private CancellationTokenSource _TokenSource = null;
    public override void Unload()
    {
        if (_IconItem != null)
        {
            UIGoodsUtility.DestroyGoodsItemIcon(_IconItem);
        }
        _TokenSource?.Cancel();
        base.Unload();
        _TokenSource = null;
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();
    }
    public async void Setting(int goodsCfgId)
    {
        var count = UIGoodsMgr.Instance.GetGoodsCount(goodsCfgId);
        SetCount(count);

        _TokenSource = new();
        var (isCancel, item) = await UIGoodsUtility.CreateGoodsItemIcon(goodsCfgId, _IconRootRect, _TokenSource.Token)
            .SuppressCancellationThrow();
        if (!isCancel)
        {
            item.Setting(new(goodsCfgId), EnSourceId.UIHallGoodsBarWindow);
            _IconItem = item;
            _TokenSource = null;
        }
    }
    public void SetCount(int count)
    {
        _CountTxt.SetText($"{count}");
    }
    public float GetIconScale()
    {
        return _IconRootRect.localScale.x;
    }
    public void SetIconScale(float scale)
    {
        _IconRootRect.localScale = Vector3.one * scale;
    }
    public Vector3 GetIconTargetWorldPos()
    {
        return _IconRootRect.position;
    }
}
