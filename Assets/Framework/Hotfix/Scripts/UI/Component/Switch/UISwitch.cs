using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract partial class UISwitch : MonoBehaviour, IUISwitch
{
    public abstract void Switch(int index);
}

#if UNITY_EDITOR
public partial class UISwitch
{
    public abstract int GetSwitchCount();
}

#endif