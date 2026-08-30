using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class UICoreInfoDialog_GemAttributeItem : UIWindowItem
{
    [SerializeField, AutoReference]
    private TextMeshProUGUI _DescTxt;
    [SerializeField, AutoReference]
    private RectTransform _IconRootRect;

    private UIGoodsGemItemIcon _IconItem;
    public override void Unload()
    {
        if (_IconItem != null)
        {
            UIGoodsUtility.DestroyGoodsItemIcon(_IconItem);
            _IconItem = null;
        }
        base.Unload();
    }
    public async void Setting(int gemId)
    {
        var descStr = UIGemUtility.GetDescriptByGemId(gemId);
        _DescTxt.SetText(descStr);

        var goodsCfgId = UIGemUtility.GetGemGoodsCfgId(gemId);
        var (isCancel, item) = await UIGoodsUtility.CreateGoodsItemIcon<UIGoodsGemItemIcon>(goodsCfgId, _IconRootRect, Token)
            .SuppressCancellationThrow();
        if (!isCancel)
        {
            item.SettingGemId(gemId, null, EnSourceId.UICoreInfoDialog);
            _IconItem = item;
        }
    }
}
