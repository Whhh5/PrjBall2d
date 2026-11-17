using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class UIGoodsItemCountLeftTip : UIWindowItem
{
    [SerializeField, AutoReference]
    private RectTransform _IconRootRect = null;
    [SerializeField, AutoReference]
    private TextMeshProUGUI _UpLevelNeedGoodsCountTxt;

    private UIGoodsItemIconBase _IconItem;
    private Color _DefColor;

    public override void Unload()
    {
        _UpLevelNeedGoodsCountTxt.color = _DefColor;

        if (_IconItem != null)
            UIGoodsUtility.DestroyGoodsItemIcon(_IconItem);
        base.Unload();
        _IconItem = null;
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();
        _DefColor = _UpLevelNeedGoodsCountTxt.color;
    }

    public async void Setting(GoodsInfo goodsInfo, EnSourceId sourceId)
    {
        SetCount(goodsInfo.goodsCount);

        var (isCancel, item) = await UIGoodsUtility.CreateGoodsItemIcon(goodsInfo.goodsCfgId, _IconRootRect, Token)
            .SuppressCancellationThrow();
        if (!isCancel)
        {
            item.Setting(new(goodsInfo.goodsCfgId, goodsInfo.goodsId), sourceId);
            _IconItem = item;
        }
    }
    public void SetCount(int count)
    {
        var str = count < 0 ? LanguageUtility.GetLanguage_10035() : $"{count}";
        _UpLevelNeedGoodsCountTxt.SetText(str);
    }
}
