using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGoodsItemInfoTipsUserData : UITipsUserData
{
    public GoodsKey goodsKey;
    public EnSourceId sourceId;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        goodsKey = default;
        sourceId = EnSourceId.None;
    }
    public void Setting(GoodsKey goodsKey, EnSourceId sourceId)
    {
        this.goodsKey = goodsKey;
        this.sourceId = sourceId;
    }
}
public class UIGoodsItemInfoTipsData : UITipsData<UIGoodsItemInfoTips, UIGoodsItemInfoTipsUserData>
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIGoodsItemInfoTips;
    private GoodsKey _GoodsKey;
    private EnSourceId _SourceId;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        _GoodsKey = default;
        _SourceId = EnSourceId.None;
    }
    protected override void OnPoolInit(UIGoodsItemInfoTipsUserData userData)
    {
        _GoodsKey = userData.goodsKey;
        _SourceId = userData.sourceId;
    }
    public override void OnGOCreate(GameEntity entity)
    {
        base.OnGOCreate(entity);
        _UITips.Setting(_GoodsKey, _SourceId);
    }
}
public class UIGoodsItemInfoTips : UITips
{
    [SerializeField, AutoReference]
    private RectTransform _IconRootRect;
    [SerializeField, AutoReference]
    private TextMeshProUGUI _NameTxt;
    [SerializeField, AutoReference]
    private TextMeshProUGUI _DescTxt;

    private UIGoodsItemIconBase _IconItem = null;
    public override void OnUnload()
    {
        if (_IconItem != null)
        {
            UIMgr.Instance.UnloadWindowItem(_IconItem);
            _IconItem = null;
        }
        base.OnUnload();
    }
    public async void Setting(GoodsKey goodsKey, EnSourceId sourceId)
    {
        var goodsCfg = GameSchedule.Instance.GetGoodsCfg0(goodsKey.goodsCfgId);
        var goodsName = UIGoodsMgr.Instance.GetGoodsName(goodsKey.goodsCfgId);
        _NameTxt.SetText(goodsName);
        _DescTxt.SetLanguageText(goodsCfg.nDescriptLangId);

        var (isCancel, item) = await UIGoodsUtility.CreateGoodsItemIcon(goodsKey.goodsCfgId, _IconRootRect, GetWindowToken())
            .SuppressCancellationThrow();
        if(!isCancel)
        {
            item.Setting(goodsKey, sourceId);
            _IconItem = item;
        }
    }
}
