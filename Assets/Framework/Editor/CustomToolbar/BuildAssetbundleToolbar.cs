using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomUnityToolbarAttribute("BuildAssetbundle", EnCustomUnityToolbarType.Left, EnCustomUnityToolbarOrder.BuildAssetbundle)]
public class BuildAssetbundleToolbar : ICustomUnityToolbar
{
    public void OnClick()
    {
        AssetbundleToolEditor.BuildAssetbundle();
    }
}
