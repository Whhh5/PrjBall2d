using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]

public partial class UISpriteSwitch : UISwitch
{
    private void Awake()
    {
        
    }
    private Image m_TargetImg => GetComponent<Image>();
    [SerializeField]
    private List<UISpriteSwitchItem> m_ArrSprite = new();
    private void Start()
    {
        
    }

    public override void Switch(int index)
    {
        if(index >= m_ArrSprite.Count)
            return;
        var itemData = m_ArrSprite[index];
        m_TargetImg.sprite = itemData.GetSprite();
        m_TargetImg.color = itemData.GetColor();
    }
}

#if UNITY_EDITOR
public partial class UISpriteSwitch
{
    public List<UISpriteSwitchItem> GetArrSprite()
    {
        return m_ArrSprite;
    }

    public override int GetSwitchCount()
    {
        return m_ArrSprite.Count;
    }
}
#endif