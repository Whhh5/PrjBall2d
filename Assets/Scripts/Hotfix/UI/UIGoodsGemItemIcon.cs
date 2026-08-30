using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGoodsGemItemIcon : UIGoodsItemIconBase
{
    [SerializeField, AutoReference]
    private Image _QualityImg;
    [SerializeField, AutoReference]
    private Image _TypeImg;
    [SerializeField, AutoReference]
    private Image _IconImg;

    public override void Unload()
    {
        _QualityImg.UnloadSprite();
        _TypeImg.UnloadSprite();
        _IconImg.UnloadSprite();
        base.Unload();
    }

    public override void Setting(in GoodsKey goodsKey, Action callback, EnSourceId sourceId)
    {
        base.Setting(goodsKey, callback, sourceId);

        var quality = UIGemUtility.GetGemQuality(goodsKey.goodsId);
        var type = UIGemUtility.GetGemType(goodsKey.goodsId);

        var qualityCfg = GameSchedule.Instance.GetGemQualityCfg0((int)quality);
        var typeCfg = GameSchedule.Instance.GetGemTypeCfg0((int)type);

        if (ColorUtility.TryParseHtmlString(qualityCfg.strColor, out var color))
        {
            _QualityImg.color = color;
        }
        _QualityImg.LoadSprirte(typeCfg.nLightAssetId);
        _TypeImg.LoadSprirte(typeCfg.nIconAssetId);
        var iconAssetId = UIGemUtility.GetIconAssetId(goodsKey.goodsId);
        _IconImg.LoadSprirte(iconAssetId);

    }

    public void SettingGemId(int gemId, EnSourceId sourceId)
    {
        var goodsCfgId = UIGemUtility.GetGemGoodsCfgId(gemId);
        Setting(new(goodsCfgId, gemId), sourceId);
    }
    public void SettingGemId(int gemId, Action callback, EnSourceId sourceId)
    {
        var goodsCfgId = UIGemUtility.GetGemGoodsCfgId(gemId);
        Setting(new(goodsCfgId, gemId), callback, sourceId);
    }
}
