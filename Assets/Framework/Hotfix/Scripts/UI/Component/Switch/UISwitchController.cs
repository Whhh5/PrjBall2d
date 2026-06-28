using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed partial class UISwitchController : MonoBehaviour, IUISwitch
{
    [SerializeField]
    private List<UISwitch> m_ArrChildSwitch = new();
    public void Switch(int index)
    {
        foreach(var item in m_ArrChildSwitch)
        {
            item.Switch(index);
        }
    }
}



#if UNITY_EDITOR
public partial class UISwitchController
{
    public void SetArrChildSwitch(List<UISwitch> arrSwitch)
    {
        m_ArrChildSwitch = arrSwitch;
    }
    public List<UISwitch> GetArrChildSwitch()
    {
        return m_ArrChildSwitch;
    }
}

#endif