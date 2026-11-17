
using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIGoodsItemIconBase : UIWindowItem
{
    [SerializeField, AutoReference]
    private Button _Btn;

    protected int _GoodsCfgId = -1;
    protected int _GoodsId = -1;
    private Action _Callback;

    public override void Unload()
    {
        _Btn.RemoveListener(OnClick_Btn);
        base.Unload();
        _Callback = null;
        _GoodsCfgId = -1;
        _GoodsId = -1;
    }
    public override void LoadCompletion()
    {
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

    private void OnClick_Btn()
    {
        _Callback.Invoke();
    }
}
