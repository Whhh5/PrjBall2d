using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public sealed partial class UISpriteSwitchItem
{
    [SerializeField]
    private Sprite m_Sprite = null;
    [SerializeField]
    private Color m_Color = Color.white;

    public Sprite GetSprite()
    {
        return m_Sprite;
    }
    public Color GetColor()
    {
        return m_Color;
    }
}

#if UNITY_EDITOR
public partial class UISpriteSwitchItem
{
    public void SetSprite(Sprite sprite)
    {
        m_Sprite = sprite;
    }
    public void SetColor(Color color)
    {
        m_Color = color;
    }
}

#endif