using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System;
using Cysharp.Threading.Tasks;
using PlasticPipe.Server;

public static class AssetbundleToolEditor
{
    private static BuildTarget CurBuildTarget => GlobalConfigEditorSO.Instance.BuildTarget;
    public static async void BuildAssetbundle()
    {
        var pathCount = AssetbundleBuilderSO.Instance.abPaths.Length;
        var abBuilds = new List<AssetBundleBuild>();
        for (int i = 0; i < pathCount; i++)
        {
            var path = AssetbundleBuilderSO.Instance.abPaths[i];
            var fullPath = ABBUtil.GetFullPathByUnityPath(path);
            DirLoop(fullPath);
        }

        var outputPath = AssetbundleEditorUtil.GetAbBuilderPath(CurBuildTarget);
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        var buildOption = BuildAssetBundleOptions.None;
        var manifest = BuildPipeline.BuildAssetBundles(outputPath, abBuilds.ToArray(), buildOption, CurBuildTarget);

        Debug.Log(outputPath);

        if (manifest == null)
            throw new System.Exception("build assetbundle error");

        var abMap = CreateAbMapFile(in abBuilds);
        var assetMap = CreateAssetMapFile(in abBuilds, in abMap);
        CreateAbInfo(in abBuilds, in abMap, in assetMap);

        var abCatalogPath = AssetbundleEditorUtil.GetAbCatalogFilePath(CurBuildTarget);
        var catalogList = await CreateAbCatalogListAsync(outputPath, abBuilds);

        if (await UploadAssetbundleAsync(outputPath, abBuilds, catalogList))
            if (await SaveLocalAbCatalogAsync(abCatalogPath, catalogList))
                if (await UploadAbCatalogAsync(abCatalogPath))
                    if (await UploadAbInfoAsync())
                        if (await UploadAssetInfoAsync())
                            Debug.Log($"异步上传文件完成");

        void DirLoop(string path)
        {
            var curDir = new DirectoryInfo(path);
            var abBuild = CreateAbBulid(path, curDir);
            if (abBuild.assetNames.Length > 0)
                abBuilds.Add(abBuild);
            var dirs = curDir.GetDirectories();
            for (int i = 0; i < dirs.Length; i++)
            {
                DirLoop(dirs[i].FullName);
            }
        }
    }

    private static async UniTask<bool> UploadAssetbundleAsync(string root, List<AssetBundleBuild> abBuilds, List<AbCatalog> catalogList)
    {
        var catalogPath = AssetbundleUtil.GetAbCatalogLocalPath();
        var data = await WebUtil.DownloadFileAsync(catalogPath);

        var catalogDict = AssetbundleUtil.BytesToCatalogDict(data);

        var curCatalogDic = AssetbundleUtil.ListToCatalogDict(catalogList);

        for (int i = 0; i < abBuilds.Count; i++)
        {
            var abName = abBuilds[i].assetBundleName;
            var curCatalog = curCatalogDic[abName];
            if (catalogDict.TryGetValue(abName, out var abCatalog))
            {
                if (abCatalog.length == curCatalog.length && curCatalog.md5 == abCatalog.md5)
                    continue;
            }
            var abPath = Path.Combine(root, abName);
            var abData = await File.ReadAllBytesAsync(abPath);

            if (!await WebUtil.UploadContent(AssetbundleUtil.GetAssetbundleLocalPath(abName), abData))
                return false;
            curCatalog.version++;
        }
        return true;
    }
    private static async UniTask<bool> UploadAbCatalogAsync(string abCatalogPath)
    {
        var file = File.ReadAllBytes(abCatalogPath);
        var uploadPath = AssetbundleUtil.GetAbCatalogLocalPath();

        var result = await WebUtil.UploadContent(uploadPath, file);
        return result;
    }
    private static async UniTask<bool> UploadAbInfoAsync()
    {
        var abInfoPath = AssetbundleEditorUtil.GetAbInfoFilePath(CurBuildTarget);
        var file = File.ReadAllBytes(abInfoPath);
        var uploadPath = AssetbundleUtil.GetAbInfoLocalPath();

        var result = await WebUtil.UploadContent(uploadPath, file);
        return result;
    }
    private static async UniTask<bool> UploadAssetInfoAsync()
    {
        var assetInfoPath = AssetbundleEditorUtil.GetAssetInfoFilePath(CurBuildTarget);
        var file = File.ReadAllBytes(assetInfoPath);
        var uploadPath = AssetbundleUtil.GetAssetInfoLocalPath();

        var result = await WebUtil.UploadContent(uploadPath, file);
        return result;
    }
    private static async UniTask<bool> SaveLocalAbCatalogAsync(string path, List<AbCatalog> data)
    {
        var jsonStr = JsonConvert.SerializeObject(data);
        await File.WriteAllTextAsync(path, jsonStr);
        Debug.Log($"保存本地文件 -> {path}");
        return true;
    }
    private static async UniTask<List<AbCatalog>> CreateAbCatalogListAsync(string root, List<AssetBundleBuild> abBuilds)
    {
        var localCatalogPath = AssetbundleUtil.GetAbCatalogLocalPath();
        var data = await WebUtil.DownloadFileAsync(localCatalogPath);
        var webcatalogDict = AssetbundleUtil.BytesToCatalogDict(data);

        var catalogList = new List<AbCatalog>();
        for (int i = 0; i < abBuilds.Count; i++)
        {
            var abName = abBuilds[i].assetBundleName;
            var abPath = Path.Combine(root, abName);

            var length = ABBUtil.CalculateLength(abPath);
            var md5 = AotUtility.CalculateFileMD5(abPath);

            catalogList.Add(new AbCatalog()
            {
                version = webcatalogDict.TryGetValue(abName, out var value) ? value.version : 0,
                abName = abName,
                length = length,
                md5 = md5,
            });
        }
        return catalogList;
    }
    private static void CreateAbInfo(in List<AssetBundleBuild> abBuilds, in Dictionary<string, int> abMap, in Dictionary<string, AssetInfo> assetMap)
    {
        var outputPath = AssetbundleEditorUtil.GetAbBuilderPath(CurBuildTarget);
        var abInfos = new List<AssetbundleInfo>();

        var manifestPath = Path.Combine(outputPath, "Assetbundle");
        var unityPath = ABBUtil.GetUnityPathByFullPath(manifestPath);
        AssetBundle manifestBundle = AssetBundle.LoadFromFile(unityPath);
        var manifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        manifestBundle.Unload(false);

        for (int i = 0; i < abBuilds.Count; i++)
        {
            var abBuild = abBuilds[i];

            var assetIDs = new List<int>(abBuild.assetNames.Length);
            for (int j = 0; j < abBuild.assetNames.Length; j++)
            {
                var assetInfo = assetMap[abBuild.assetNames[j]];
                assetIDs.Add(assetInfo.assetID);
            }

            var depAbArray = manifest.GetDirectDependencies(abBuild.assetBundleName);
            var abIDs = new List<int>(depAbArray.Length);

            for (int j = 0; j < depAbArray.Length; j++)
            {
                if (!abMap.TryGetValue(depAbArray[j], out var abID))
                {
                    ABBUtil.LogError($"ab信息不存在 - {depAbArray[j]}");
                    continue;
                }
                abIDs.Add(abID);
            }

            abInfos.Add(new()
            {
                abID = abMap[abBuild.assetBundleName],
                abName = abBuild.assetBundleName,
                abAssetIDs = assetIDs.ToArray(),
                dependAbList = abIDs.ToArray(),
            });
        }
        var content = JsonConvert.SerializeObject(abInfos);
        var path = AssetbundleEditorUtil.GetAbInfoFilePath(CurBuildTarget);
        File.WriteAllText(path, content.ToString(), Encoding.UTF8);
    }
    private static Dictionary<string, AssetInfo> CreateAssetMapFile(in List<AssetBundleBuild> builds, in Dictionary<string, int> abMap)
    {
        var result = new Dictionary<string, AssetInfo>(builds.Count);
        var list = new List<AssetInfo>(builds.Count);
        //var assetCfg = ExcelUtil.GetCfg<AssetCfg>(

        for (int i = 0; i < builds.Count; i++)
        {
            var build = builds[i];
            for (int j = 0; j < builds[i].assetNames.Length; j++)
            {
                var assetPath = builds[i].assetNames[j];
                var adressablePath = builds[i].addressableNames[j];
                var enumName = LoadConfigEditor.GetLoadTargetName(assetPath);

                if (!Enum.TryParse<EnLoadTarget>(enumName, false, out var value))
                {
                    Debug.LogError($"{assetPath} 获取 {enumName} 失败, 请检查 AssetCfg 或者 该资源是否要使用 assetbundle 加载");
                }
                Debug.Log($"{enumName} => {(int)value}");
                var data = new AssetInfo()
                {
                    abID = abMap[build.assetBundleName],
                    assetID = (int)value,
                    path = assetPath,
                    addressableName = adressablePath,
                };
                result.Add(assetPath, data);
                list.Add(data);
            }
        }
        var content = JsonConvert.SerializeObject(list);
        var path = AssetbundleEditorUtil.GetAssetInfoFilePath(CurBuildTarget);
        File.WriteAllText(path, content.ToString(), Encoding.UTF8);
        return result;
    }
    private static Dictionary<string, int> CreateAbMapFile(in List<AssetBundleBuild> builds)
    {
        var result = new Dictionary<string, int>(builds.Count);
        //var content = new StringBuilder(builds.Count * 10);
        for (int i = 0; i < builds.Count; i++)
        {
            result.Add(builds[i].assetBundleName, i + 1);
            //content.AppendLine($"{i},{builds[i].assetBundleName}");
        }
        //var path = AssetbundleUtil.GetAbIDMapFilePath(BuildTarget2RuntimePlatform(CurBuildTarget));
        //File.WriteAllText(path, content.ToString(), Encoding.UTF8);
        return result;
    }

    private static AssetBundleBuild CreateAbBulid(string dir, DirectoryInfo dirInfo)
    {
        var addressableNames = new List<string>(10);
        var assetNames = new List<string>(10);

        var fileInfos = dirInfo.GetFiles();
        for (int i = 0; i < fileInfos.Length; i++)
        {
            var fileInfo = fileInfos[i];
            var suffix = Path.GetExtension(fileInfo.FullName);
            if (!AssetbundleBuilderSO.Instance.IsHotType(suffix))
                continue;
            var unityPath = ABBUtil.GetUnityPathByFullPath(fileInfo.FullName);
            addressableNames.Add(addressableNames.Count.ToString());
            assetNames.Add(unityPath);
        }

        //var num = Random.Range(0, 100);
        return new AssetBundleBuild()
        {
            addressableNames = addressableNames.ToArray(),
            assetBundleName = dirInfo.Name.ToLowerInvariant() + ".ab",
            //assetBundleVariant = ,
            assetNames = assetNames.ToArray(),
        };
    }


}
