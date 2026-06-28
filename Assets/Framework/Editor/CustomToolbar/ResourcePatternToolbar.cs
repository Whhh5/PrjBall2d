using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomUnityToolbarAttribute("ResourcePattern", EnCustomUnityToolbarType.Right, EnCustomUnityToolbarOrder.ResourcePattern)]
public class ResourcePatternToolbar : ICustomUnityToolbar, ICustomUnityToolbarBtnName
{
    public string GetBtnName()
    {
        return GlobalConfigSO.Instance.LoaderType.ToString();
    }

    public void OnClick()
    {
        GlobalConfigSO.Instance.LoaderType =
            (EnLoaderType)((int)(GlobalConfigSO.Instance.LoaderType + 1) % (int)EnLoaderType.EnumCount);
    }
}
