using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

[EnumName("包围盒类型")]
public enum EnPhysicsType
{
    Sphere = 1,
    Box = 2,
}
[EnumName("方形包围盒位置类型")]
public enum EnPhysicsBoxCenterType
{
    None = 0,
    Center = 1,
    Bottom = 2,
}
[EnumName("方形包围盒类型")]
public enum EnPhysicsBoxType
{
    None = 0,
    Once = 1,
    Successive = 2,
}
public class CfgMgr : Singleton<CfgMgr>
{

    public override async UniTask AwakeAsync()
    {
        await base.AwakeAsync();

        var type = Instance.GetType();
    }
}
