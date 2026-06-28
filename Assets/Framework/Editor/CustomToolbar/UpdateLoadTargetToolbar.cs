using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomUnityToolbarAttribute("Update Load Target", EnCustomUnityToolbarType.Left, EnCustomUnityToolbarOrder.UpdateLoadTarget)]
public class UpdateLoadTargetToolbar : ICustomUnityToolbar
{
    public void OnClick()
    {
        LoadConfigEditor.CreateLoadConfigJson();
    }
}
