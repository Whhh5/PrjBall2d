using UnityEngine;
using Cysharp.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System;
using System.Reflection;
using HybridCLR;
using System.Collections.Generic;
using System.Text;
using System.Linq;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class GameStartAttribute : Attribute
{

}
public class AbVersionInfo
{
    public int version;
}
public class HotfixMgr : IUpdateGame
{
    public async UniTask InitializationAsync()
    {

        Debug.LogWarning("开始检查更新 Assetbundle");
        await CheckCatalogAsync();
        Debug.LogWarning("开始加载程序集");
        await LoadAssemblyAsync();
        await LoadAssemblyMatadataAsync();
        Debug.LogWarning("更新完成");

    }

    private async UniTask CheckCatalogAsync()
    {
        var path = AssetbundleUtil.GetAbCatalogLocalPath();
        var webCatalogData = await WebUtil.DownloadFileAsync(path);
        var catalogStr = Encoding.UTF8.GetString(webCatalogData);
        var webCatalogList = AssetbundleUtil.JsonToCatalogList(catalogStr);

        var localCatalogPath = AssetbundleUtil.GetCurPlatformAbCatalogFilePath();

        for (int i = 0; i < webCatalogList.Count; i++)
        {
            var webCatalog = webCatalogList[i];
            var versionPath = AssetbundleUtil.GetCurPlatformAssetbundleVersionFilePath(webCatalog.abName);

            var versionInfoStr = File.Exists(versionPath) ? await File.ReadAllTextAsync(versionPath) : "";
            var versionInfo = JsonConvert.DeserializeObject<AbVersionInfo>(versionInfoStr) ?? new();
            if (versionInfo.version == webCatalog.version)
                continue;

            var abLoadPath = AssetbundleUtil.GetAssetbundleLocalPath(webCatalog.abName);
            var abFileByte = await WebUtil.DownloadFileAsync(abLoadPath);
            if (abFileByte.Length != webCatalog.length
                || AotUtility.CalculateFileMD5(abFileByte) != webCatalog.md5)
            {
                // 下载失败重复下载当前文件
                Debug.LogError($"下载失败重新下载 -> {abLoadPath}");
                i--;
                continue;
            }
            // 下载校验成功保存文件
            var abPath = AssetbundleUtil.GetCurPlatformAssetbundlePath(webCatalog.abName);
            var root = Path.GetDirectoryName(abPath);
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);
            Debug.Log($"保存文件 -> {abPath}, {abFileByte.Length}");
            File.WriteAllBytes(abPath, abFileByte);
            Debug.Log($"保存文件2 -> {abPath}, {abFileByte.Length}");
            await AssetBundle.RecompressAssetBundleAsync(abPath, abPath, BuildCompression.LZ4Runtime);
            versionInfo.version = webCatalog.version;
            var versionInfoJson = JsonConvert.SerializeObject(versionInfo);
            Debug.Log($"保存文件 -> {versionPath}, {versionInfoJson}");
            File.WriteAllText(versionPath, versionInfoJson);
        }
        File.WriteAllBytes(localCatalogPath, webCatalogData);

        // 下载assetinfo 和 abinfo
        var abInfo = AssetbundleUtil.GetAbInfoLocalPath();
        var abInfoLocalPath = AssetbundleUtil.GetCurPlatformAbInfoFilePath();
        await DownloadFile(abInfo, abInfoLocalPath);

        var assetInfo = AssetbundleUtil.GetAssetInfoLocalPath();
        var assetInfoLocalPath = AssetbundleUtil.GetCurPlatformAssetInfoFilePath();
        await DownloadFile(assetInfo, assetInfoLocalPath);

    }
    private async UniTask DownloadFile(string webLocalPath, string savePath)
    {
        var abInfoData = await WebUtil.DownloadFileAsync(webLocalPath);
        File.WriteAllBytes(savePath, abInfoData);
    }
    private async UniTask LoadAssemblyAsync()
    {
        var abPath = AssetbundleUtil.GetCurPlatformAssetbundlePath(GlobalConfigSO.Instance.HotfixAssetbundleName);
        var ab = await AssetBundle.LoadFromFileAsync(abPath);

        for (int i = 0; i < GlobalConfigSO.Instance.HotfixList.Length; i++)
        {
            var name = GlobalConfigSO.Instance.HotfixList[i];
            var file = ab.LoadAssetAsync<TextAsset>(name);
            Debug.Log($"loadassembly: {name}");

            var data = file.asset as TextAsset;

#if UNITY_EDITOR
            // Assembly hotUpdateAss = null; // AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == name);
#else
            Assembly hotUpdateAss = Assembly.Load(data.bytes);
            Debug.LogWarning($"load hotfixassembly: {hotUpdateAss.FullName}");
#endif
        }
        ab.Unload(true);
    }

    private async UniTask LoadAssemblyMatadataAsync()
    {
        var metaDataAbPath = AssetbundleUtil.GetCurPlatformAssetbundlePath(GlobalConfigSO.Instance.HotfixMatadataAssetbundleName);
        var metadataAb = await AssetBundle.LoadFromFileAsync(metaDataAbPath);
        var metadataNames = metadataAb.GetAllAssetNames();

        for (int i = 0; i < metadataNames.Length; i++)
        {
            var name = metadataNames[i];
            var file = metadataAb.LoadAssetAsync<TextAsset>(name);
            Debug.Log(file.asset);

            var data = file.asset as TextAsset;

#if UNITY_EDITOR

#else
            RuntimeApi.LoadMetadataForAOTAssembly(data.bytes, HomologousImageMode.SuperSet);
#endif
        }
    }
}
