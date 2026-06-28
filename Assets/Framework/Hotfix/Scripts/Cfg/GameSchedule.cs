using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Scripting;

[Preserve]
public class ExportExcelInfo
{
    public ExcelInfo excelInfo;
    public string strDesc;
    public List<string> fieldList = new();
    public Dictionary<string, int> field2ColList = new();
    public List<string> fieldTypeList = new();
    public List<string> descList = new();
}
[Preserve]
public class ExcelInfo
{
    public string excelName;
    public int dataStartRow;
    public int dataStartCol;
    public int fieldRow;
    public int fieldTypeRow;
    public int descRow;
    public HashSet<int> validCol = new();
    public List<List<CfgKeyInfo>> keysCol = new();
}
[Preserve]
public class CfgKeyInfo
{
    public int col;
    public string fieldName;
    public string fieldType;
}
public partial class GameSchedule : Singleton<GameSchedule>
{
    public override EnSingletonOrder SingletonOrder => EnSingletonOrder.GameSchedule;
    public override async UniTask AwakeAsync()
    {
        await base.AwakeAsync();

        //var list = new List<ExportExcelInfo>();
        //list.Add(new());
        //Debug.Log($"GameSchedule ------- 1: {new List<ExportExcelInfo>()}");
        //var serJson = JsonConvert.SerializeObject(list);
        //Debug.Log($"GameSchedule ------- 2: {serJson}");
        //var desJson = JsonConvert.DeserializeObject(serJson, typeof(List<ExportExcelInfo>));
        //Debug.Log($"GameSchedule ------- 3: {desJson}");
        //Debug.Log($"testabcataloglength: {new List<ExportExcelInfo>()}");
        //var catalog = await ReadCfgAsync(EnLoadTarget.Json_CfgCatalog, typeof(List<ExportExcelInfo>));
        //list = catalog as List<ExportExcelInfo>;

        var clss = GetType().Assembly.GetTypes().Where(type => typeof(ICfg).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface).ToList();

        for (int i = 0; i < clss.Count; i++)
        {
            var item = clss[i];
            var name = item.Name;
            var type = this.GetType();
            var cfgType = Type.GetType(name);
            var loadTarget = Enum.Parse<EnLoadTarget>($"Json_{name}");
            var obj = await ReadCfgAsync(loadTarget, cfgType);
            var fieldName = $"m_{name}";
            var fileInfo = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (fileInfo == null)
            {
                ABBUtil.LogError($"read cfg [{name}] error");
                continue;
            }
            fileInfo.SetValue(Instance, obj);
        }

        Initialization();


        //var abPAth = AssetbundleUtil.GetAssetbundleLocalPath(GlobalConfigSO.Instance.ConfigAssetbundleName);
        //var configAb = AssetBundle.LoadFromFile(abPAth);
        //configAb.LoadAsset<TextAsset>();
    }

    public static string GetCfgPath<T>()
    {
        var path = GetCfgPath(typeof(T).ToString());
        return path;
    }
    public static string GetCfgPath(string cfgName)
    {
        var path = Path.Combine(GlobalConfig.CfgRootPath, $"{cfgName}.json");
        return path;
    }
    //public static T[] ReadCfg<T>()
    //    where T : ICfg
    //{
    //    var fileName = $"{typeof(T)}";
    //    var cfg = ReadCfg(fileName, typeof(T));
    //    return cfg as T[];
    //}
    public static async UniTask<object> ReadCfgAsync(EnLoadTarget loadType, Type type)
    {
        var ass = await ABBLoadMgr.Instance.LoadAsync<TextAsset>(loadType);

        var arrType = Array.CreateInstance(type, 0).GetType();

        //var cfg2 = JsonConvert.DeserializeObject(ass.text, typeof(List<ExportExcelInfo>));
        //Debug.Log($"gameschedule 2, cfg2 = {cfg2}");
        var cfg = JsonConvert.DeserializeObject(ass.text, arrType);

        return cfg;
    }
    public static object ReadCfg(EnLoadTarget loadType, Type type)
    {
        var ass = ABBLoadMgr.Instance.Load<TextAsset>(loadType);

        var arrType = Array.CreateInstance(type, 0).GetType();

        var cfg = JsonConvert.DeserializeObject(ass.text, arrType);
        return cfg;
    }
}
