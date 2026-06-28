using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Diagnostics;

[CustomUnityToolbarAttribute("OpenPersistentDataRoot", EnCustomUnityToolbarType.Right, EnCustomUnityToolbarOrder.OpenPersistentDataRoot)]
public class OpenPersistentDataRootToolbar : ICustomUnityToolbar
{
    public void OnClick()
    {
        EditorUtil.OpenFolder(Application.persistentDataPath);
    }
    
}
