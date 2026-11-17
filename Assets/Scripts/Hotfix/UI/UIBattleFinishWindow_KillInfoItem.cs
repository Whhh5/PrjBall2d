using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIBattleFinishWindow_KillInfoItem : UIWindowItem
{
    [SerializeField, AutoReference]
    private TextMeshProUGUI _TitleTxt;
    [SerializeField, AutoReference]
    private TextMeshProUGUI _CountTxt;

    public void Setting(string title, string count)
    {
        _TitleTxt.SetText(title);
        _CountTxt.SetText(count);
    }
}
