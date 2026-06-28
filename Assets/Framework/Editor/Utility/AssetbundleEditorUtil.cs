using System.IO;
using UnityEngine;


public static class AssetbundleEditorUtil
{
    public static string GetAbBuilderPath(RuntimePlatform buildTarget)
    {
        return Path.Combine(ABBUtil.GetUnityMiscPath(), buildTarget.ToString(), "Assetbundle");
    }
    public static string GetAssetInfoFilePath(RuntimePlatform buildTarget)
    {
        return Path.Combine(GetAbBuilderPath(buildTarget), "AssetInfo.txt");
    }
    public static string GetAbInfoFilePath(RuntimePlatform buildTarget)
    {
        return Path.Combine(GetAbBuilderPath(buildTarget), "AbInfo.txt");
    }
    public static string GetAbCatalogFilePath(UnityEditor.BuildTarget buildTarget)
    {
        return Path.Combine(GetAbBuilderPath(buildTarget), "AbCatalog.txt");
    }
    public static string GetAssetInfoFilePath(UnityEditor.BuildTarget buildTarget)
    {
        return GetAssetInfoFilePath(BuildTarget2RuntimePlatform(buildTarget));
    }
    public static string GetAbInfoFilePath(UnityEditor.BuildTarget buildTarget)
    {
        return GetAbInfoFilePath(BuildTarget2RuntimePlatform(buildTarget));
    }
    public static string GetAbBuilderPath(UnityEditor.BuildTarget buildTarget)
    {
        return GetAbBuilderPath(BuildTarget2RuntimePlatform(buildTarget));
    }
    public static RuntimePlatform BuildTarget2RuntimePlatform(UnityEditor.BuildTarget buildTarget)
    {
        return AssetbundleUtil.BuildTarget2RuntimePlatform(buildTarget);
    }
    public static string GetHistoryRootPath()
    {
        var path = Path.Combine(ABBUtil.GetUnityMiscPath(), "Histroy", "Assetbundle");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        return path;
    }
}
