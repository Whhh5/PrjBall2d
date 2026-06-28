
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Plastic.Newtonsoft.Json.Converters;
using UnityEngine;

[CustomUnityToolbarAttribute("ClearCache", EnCustomUnityToolbarType.Right, EnCustomUnityToolbarOrder.ClearCache)]
public class ClearCacheToolbar : ICustomUnityToolbar
{
    public void OnClick()
    {
        ToolsMenu.CleanEditorMemory();
        var settings = new JsonSerializerSettings
        {
            ContractResolver = new PrivateFieldContractResolver(),
            Formatting = Formatting.Indented // 格式化输出，便于阅读,
        };
        Debug.Log(JsonConvert.SerializeObject(new SkillStageInfo(), settings));
    }

}


