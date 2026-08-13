using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIDescriptionLevel3WindowItem : UIWindowItem
{
    [SerializeField]
    private TextMeshProUGUI _DescLevel3Txt;


    public void Setting(string content)
    {
        _DescLevel3Txt.SetText(content);
    }
}
