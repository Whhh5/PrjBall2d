using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBattleEntityAttributeWindow_AttributeItem : UIWindowItem
{
    [SerializeField]
    private TextMeshProUGUI _NameTxt;
    [SerializeField]
    private TextMeshProUGUI _ValueTxt;
    [SerializeField]
    private Image _IconImg;

    private EnEntityAttributeId _AttId;
    public override void LoadCompeletion()
    {
        base.LoadCompeletion();
    }
    public override void Unload()
    {
        _IconImg.UnloadSprite();
        base.Unload();
        _AttId = EnEntityAttributeId.None;
    }

    public void Setting(EnEntityAttributeId attId, float value)
    {
        _AttId = attId;
        var valueStr = LanguageUtility.GetLanguage_AttributeValue(attId, value);
        _ValueTxt.SetText(valueStr);

        var attCfg = GameSchedule.Instance.GetAttributeCfg0((int)attId);
        _IconImg.LoadSprirte((EnLoadTarget)attCfg.nIconAssetId);

        _NameTxt.SetLanguageText(attCfg.nNameLangId);
    }

    public void SetValue(float value)
    {
        var valueStr = LanguageUtility.GetLanguage_AttributeValue(_AttId, value);
        _ValueTxt.SetText(valueStr);
    }
}
