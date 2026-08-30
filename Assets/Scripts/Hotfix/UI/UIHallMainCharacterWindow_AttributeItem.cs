using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHallMainCharacterWindow_AttributeItem : UIWindowItem
{
    [SerializeField, AutoReference]
    private Image _IconImg;
    [SerializeField, AutoReference]
    private TextMeshProUGUI _NameTxt;
    [SerializeField, AutoReference]
    private TextMeshProUGUI _ValueTxt;

    public override void Unload()
    {
        _IconImg.UnloadSprite();
        base.Unload();
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();


    }
    public void Setting(EnEntityAttributeId attId, string value)
    {
        var attCfg = GameSchedule.Instance.GetAttributeCfg0((int)attId);
        _IconImg.LoadSprirte(attCfg.nIconAssetId);
        _NameTxt.SetLanguageText(attCfg.nNameLangId);
        SetValue(value);
    }
    public void SetValue(string value)
    {
        _ValueTxt.SetText(value);
    }

}
