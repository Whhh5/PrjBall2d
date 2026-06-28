

using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using UnityEngine;


public static class AssetbundleUtil
{
    public static string GetAbSoPath()
    {
        return Path.Combine("Assets/Framework/Editor/SO", "AbBuilderConfig.asset");
    }
    private static string GetAbRootLoadlPath()
    {
        return Path.Combine($"{CurRuntimePlatform}", "Assetbundle");
    }
    public static string GetAbCatalogLocalPath()
    {
        return Path.Combine(GetAbRootLoadlPath(), "AbCatalog.txt");
    }
    public static string GetAbInfoLocalPath()
    {
        return Path.Combine(GetAbRootLoadlPath(), "abInfo.txt");
    }
    public static string GetAssetInfoLocalPath()
    {
        return Path.Combine(GetAbRootLoadlPath(), "AssetInfo.txt");
    }
    public static string GetAssetbundleLocalPath(string abName)
    {
        return Path.Combine(GetAbRootLoadlPath(), abName);
    }
    public static List<AbCatalog> BytesToCatalogList(byte[] data)
    {
        var jsonStr = Encoding.ASCII.GetString(data, 0, data.Length);
        var catalog = JsonToCatalogList(jsonStr);
        return catalog ?? new();
    }
    public static List<AbCatalog> JsonToCatalogList(string json)
    {
        var catalog = JsonConvert.DeserializeObject<List<AbCatalog>>(json);
        return catalog ?? new();
    }
    public static Dictionary<string, AbCatalog> BytesToCatalogDict(byte[] data)
    {
        var catalogList = BytesToCatalogList(data);
        var dict = ListToCatalogDict(catalogList);
        return dict;
    }
    public static Dictionary<string, AbCatalog> ListToCatalogDict(List<AbCatalog> catalogList)
    {
        var dict = new Dictionary<string, AbCatalog>(catalogList.Count);
        for (int i = 0; i < catalogList.Count; i++)
        {
            dict.Add(catalogList[i].abName, catalogList[i]);
        }
        return dict;
    }
    public static string GetCurPlatformPath()
    {
        return Path.Combine(AotUtility.GetPersistentDataPath(), CurRuntimePlatform.ToString());
    }
    public static string GetCurPlatformAssetbundleVersionFilePath(string abName)
    {
        return Path.Combine(GetCurPlatformAssetbundlePath(abName) + ".version");
    }
    public static string GetCurPlatformAssetbundlePath(string abName)
    {
        return Path.Combine(GetCurPlatformPath(), "Assetbundle", abName);
    }
    public static string GetCurPlatformAssetbundleRoot()
    {
        return Path.Combine(GetCurPlatformPath(), "Assetbundle");
    }
    public static string GetCurPlatformAssetInfoFilePath()
    {
        return Path.Combine(GetCurPlatformAssetbundleRoot(), "AssetInfo.txt");
    }
    public static string GetCurPlatformAbInfoFilePath()
    {
        return Path.Combine(GetCurPlatformAssetbundleRoot(), "AbInfo.txt");
    }
    public static string GetCurPlatformAbCatalogFilePath()
    {
        return Path.Combine(GetCurPlatformAssetbundleRoot(), "AbCatalog.txt");
    }
    public static RuntimePlatform CurRuntimePlatform
    {
        get
        {
#if UNITY_EDITOR
            return BuildTarget2RuntimePlatform(UnityEditor.EditorUserBuildSettings.activeBuildTarget);
#else
            return Application.platform;
#endif
        }
    }
#if UNITY_EDITOR
    public static RuntimePlatform BuildTarget2RuntimePlatform(UnityEditor.BuildTarget buildTarget)
    {
        return buildTarget switch
        {
            UnityEditor.BuildTarget.Android => RuntimePlatform.Android,
            UnityEditor.BuildTarget.iOS => RuntimePlatform.IPhonePlayer,
            UnityEditor.BuildTarget.StandaloneWindows64 => RuntimePlatform.WindowsPlayer,
            UnityEditor.BuildTarget.StandaloneOSX => RuntimePlatform.OSXPlayer,
            UnityEditor.BuildTarget.WebGL => RuntimePlatform.WebGLPlayer,
            _ => throw new System.Exception($"build target error {buildTarget}")
        };
    }
#endif
}
