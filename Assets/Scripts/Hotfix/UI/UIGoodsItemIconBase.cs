
using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIGoodsItemIconBase : UIWindowItem
{
    [SerializeField, AutoReference(SearchScope.Self)]
    private RectTransform _Rect;
    [SerializeField, AutoReference]
    private Button _Btn;

    protected int _GoodsCfgId = -1;
    protected int _GoodsId = -1;
    private Action _Callback;
    private Vector2 _SizeDelta;

    public override void Unload()
    {
        _Rect.sizeDelta = _SizeDelta;
        _Btn.RemoveListener(OnClick_Btn);
        base.Unload();
        _Callback = null;
        _GoodsCfgId = -1;
        _GoodsId = -1;
    }
    public override void LoadCompletion()
    {
        _SizeDelta = _Rect.rect.size;
        base.LoadCompletion();
        _Btn.AddListener(OnClick_Btn);


    }
    public virtual void Setting(GoodsKey goodsKey, EnSourceId sourceId)
    {
        Setting(in goodsKey, () => UIGoodsUtility.DefaultOnClick(goodsKey, sourceId), sourceId);
    }
    public virtual void Setting(in GoodsKey goodsKey, Action callback, EnSourceId sourceId)
    {
        _GoodsCfgId = goodsKey.goodsCfgId;
        _GoodsId = goodsKey.goodsId;
        _Callback = callback;
        SetInteractable(callback != null);
    }
    protected virtual void SetInteractable(bool interactable)
    {
        _Btn.interactable = interactable;
        _Btn.targetGraphic.raycastTarget = interactable;
    }
    public void SetSizeDelta(in Vector2 sizeDelta)
    {
        _Rect.sizeDelta = sizeDelta;
    }
    public void SetAutoSize()
    {
        _Rect.pivot = Vector2.one * 0.5f;
        _Rect.anchorMin = Vector2.zero;
        _Rect.anchorMax = Vector2.one;
        _Rect.anchoredPosition
            = _Rect.sizeDelta
            = Vector2.zero;
    }
    public void SetWorldPos(in Vector3 worldPos)
    {
        _Rect.position = worldPos;
    }
    public Vector3 GetWorldPos()
    {
        return _Rect.position;
    }
    public void SetSiblingIndex(in int index)
    {
        _Rect.SetSiblingIndex(index);
    }

    private void OnClick_Btn()
    {
        _Callback.Invoke();
    }
}
