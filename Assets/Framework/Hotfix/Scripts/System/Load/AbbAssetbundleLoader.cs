using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;
using Object = UnityEngine.Object;



public class AssetbundleInfo
{
    public int abID;
    public string abName;
    public int[] abAssetIDs;
    public int[] dependAbList;
}
public class AssetInfo
{
    public int assetID;
    public int abID;
    public string addressableName;
    public string path;
}

public class RuntimeAbInfo : IClassPoolDestroy
{
    public int infoID;
    public int refCount;
    public AssetBundle ab;
    public CancellationTokenSource tokenSource;
    public AssetBundleCreateRequest abRequest;

    public void AddRefCount()
    {
        refCount++;
    }
    public void RemoveRefCount()
    {
        refCount--;
    }
    public bool IsUnLoad()
    {
        return refCount <= 0;
    }
    public void OnPoolDestroy()
    {
        infoID = -1;
        refCount = 0;
        ab = null;
        tokenSource = null;
        abRequest = null;
    }
}
public class RuntimeAssetInfo : IClassPoolDestroy
{
    public int infoID;
    public int assetID;
    public int refCount;
    public Object obj;
    public CancellationTokenSource tokenSource;

    public void AddRefCount()
    {
        refCount++;
    }
    public void RemoveRefCount()
    {
        refCount--;
    }
    public void OnPoolDestroy()
    {
        refCount = 0;
        infoID = -1;
        assetID = 0;
        obj = null;
        tokenSource = null;
    }
    public bool IsUnload()
    {
        return refCount <= 0;
    }
}

public class AbbAssetbundleLoader : IABBAssetLoader2
{
    private Dictionary<int, AssetbundleInfo> _AbInfos = new();
    private Dictionary<int, AssetInfo> _AssetInfos = new();
    private Dictionary<int, string> _AbPathMap = new();
    private Dictionary<int, int> _AssetCfgID2AssetId = new();

    // runtime
    private Dictionary<int, RuntimeAssetInfo> _RuntimeAssetInfos = new();
    private Dictionary<int, RuntimeAbInfo> _RuntimeAbInfos = new();


    public void Log()
    {
        var stringBuilder = new StringBuilder(1000);
        stringBuilder.AppendLine($"abInfo = {{");
        foreach (var abInfo in _RuntimeAbInfos)
        {
            stringBuilder.AppendLine($"\t ID:{abInfo.Key}");
            stringBuilder.AppendLine($"\t {{");
            stringBuilder.AppendLine($"\t\t refCount:{abInfo.Value.refCount}");
            stringBuilder.AppendLine($"\t\t ab:{abInfo.Value.ab}");
            stringBuilder.AppendLine($"\t\t state:{abInfo.Value.abRequest.progress}");
            stringBuilder.AppendLine($"\t\t token:{abInfo.Value.tokenSource}");
            stringBuilder.AppendLine($"\t }}");
        }
        stringBuilder.AppendLine($"abInfo = }}");

        stringBuilder.AppendLine($"");
        stringBuilder.AppendLine($"");
        stringBuilder.AppendLine($"assetInfo = {{");
        foreach (var assetInfo in _RuntimeAssetInfos)
        {
            stringBuilder.AppendLine($"\t ID:{assetInfo.Key}");
            stringBuilder.AppendLine($"\t {{");
            stringBuilder.AppendLine($"\t\t refCount:{assetInfo.Value.refCount}");
            stringBuilder.AppendLine($"\t\t assetID:{(EnLoadTarget)assetInfo.Value.assetID}");
            stringBuilder.AppendLine($"\t\t obj:{assetInfo.Value.obj}");
            stringBuilder.AppendLine($"\t\t token:{assetInfo.Value.tokenSource}");
            stringBuilder.AppendLine($"\t }}");
        }
        stringBuilder.AppendLine($"abInfo = }}");
        Debug.LogWarning(stringBuilder);
    }
    public void Destroy()
    {
        _AbInfos.Clear();
        _AssetInfos.Clear();
        _RuntimeAbInfos.Clear();
        _RuntimeAssetInfos.Clear();
    }

    public static Dictionary<int, AssetInfo> CreateAssetInfos()
    {
        var assetInfos = new Dictionary<int, AssetInfo>();
        var assetInfoPath = AssetbundleUtil.GetCurPlatformAssetInfoFilePath();
        var assetInfoStr = File.ReadAllText(assetInfoPath, System.Text.Encoding.UTF8);
        var assetInfoList = JsonConvert.DeserializeObject<List<AssetInfo>>(assetInfoStr);

        for (int i = 0; i < assetInfoList.Count; i++)
        {
            var assetInfo = assetInfoList[i];
            assetInfos.Add(assetInfo.assetID, assetInfo);
        }


        return assetInfos;
    }

    public async UniTask Initialization()
    {
        var abInfoPath = AssetbundleUtil.GetCurPlatformAbInfoFilePath();
        var abInfoStr = File.ReadAllText(abInfoPath, System.Text.Encoding.UTF8);
        var abInfoList = JsonConvert.DeserializeObject<List<AssetbundleInfo>>(abInfoStr);

        var abRoot = AssetbundleUtil.GetCurPlatformAssetbundleRoot();
        for (int i = 0; i < abInfoList.Count; i++)
        {
            var abInfo = abInfoList[i];
            _AbInfos.Add(abInfo.abID, abInfo);
            _AbPathMap.Add(abInfo.abID, Path.Combine(abRoot, abInfo.abName));
        }


        var assetInfoPath = AssetbundleUtil.GetCurPlatformAssetInfoFilePath();
        var assetInfoStr = File.ReadAllText(assetInfoPath, System.Text.Encoding.UTF8);
        var assetInfoList = JsonConvert.DeserializeObject<List<AssetInfo>>(assetInfoStr);

        for (int i = 0; i < assetInfoList.Count; i++)
        {
            var assetInfo = assetInfoList[i];
            _AssetInfos.Add(assetInfo.assetID, assetInfo);
        }
        await UniTask.Delay(0);
    }

    public async UniTask<T> LoadAssetAsync<T>(int assetID) where T : Object
    {
        var result = await PullObject<T>(assetID);
        return result;
    }

    public T LoadAsset<T>(int assetID) where T : Object
    {
        throw new Exception($"加载超时 -> {assetID}");
    }

    public void UnloadAsset(in int assetID)
    {
        PushObject(in assetID);
    }
    public async UniTask<T> PullObject<T>(int assetID)
        where T : Object
    {
        if (!await LoadAssetAsync(assetID))
            return null;
        if (!_RuntimeAssetInfos.TryGetValue(assetID, out var runtimeAssetInfo))
            return null;

        runtimeAssetInfo.AddRefCount();
        if (runtimeAssetInfo.obj is not T)
        {
            ABBUtil.LogError($"asset type error {runtimeAssetInfo.obj} not {typeof(T)}");
            PushObject(assetID);
            return null;
        }
        return runtimeAssetInfo.obj as T;
    }
    public void PushObject(in int assetID)
    {
        if (!_RuntimeAssetInfos.TryGetValue(assetID, out var runtimeAssetInfo))
            return;
        runtimeAssetInfo.RemoveRefCount();
        if (runtimeAssetInfo.IsUnload())
            UnloadAsset(assetID);
    }

    private async UniTask<bool> LoadAssetAsync(int assetID)
    {
        if (_RuntimeAssetInfos.TryGetValue(assetID, out var runtimeAssetInfo))
        {
            var curInfoID = runtimeAssetInfo.infoID;
            if (runtimeAssetInfo.tokenSource != null)
                await UniTask.WaitUntil(() => runtimeAssetInfo.tokenSource == null);
            return curInfoID == runtimeAssetInfo.infoID;
        }

        if (!_AssetInfos.TryGetValue(assetID, out var assetInfo))
            return false;

        runtimeAssetInfo = ClassPoolMgr.Instance.Pull<RuntimeAssetInfo>();
        runtimeAssetInfo.tokenSource = new CancellationTokenSource();
        var infoID = ABBUtil.GetTempKey();
        runtimeAssetInfo.infoID = infoID;
        runtimeAssetInfo.assetID = assetID;
        _RuntimeAssetInfos.Add(assetID, runtimeAssetInfo);
        var ab = await LoadAssetbundleAsync(assetInfo.abID);
        if (infoID != runtimeAssetInfo.infoID)
        {
            UnloadAsset(assetID);
            return false;
        }
        //ABBUtil.Log($"加载 asset - {assetID} - {assetInfo.path} - {assetInfo.addressableName}");
        var asset = await ab.LoadAssetAsync<Object>(assetInfo.addressableName);
        if (infoID != runtimeAssetInfo.infoID)
        {
            UnloadAsset(assetID);
            return false;
        }
        runtimeAssetInfo.tokenSource.Dispose();
        runtimeAssetInfo.tokenSource = null;
        if (asset == null)
        {
            UnloadAsset(assetID);
            return false;
        }
        runtimeAssetInfo.obj = asset;
        return true;
    }

    private void UnloadAsset(int assetID)
    {
        if (!_RuntimeAssetInfos.TryGetValue(assetID, out var runtimeAssetInfo))
            return;
        if (!_AssetInfos.TryGetValue(assetID, out var assetInfo))
            return;
        if (!_AbInfos.TryGetValue(assetInfo.abID, out var abInfo))
            return;
        if (!_RuntimeAbInfos.TryGetValue(assetInfo.abID, out var runtimeAbInfo))
            return;

        if (runtimeAssetInfo.tokenSource != null && !runtimeAssetInfo.tokenSource.IsCancellationRequested)
        {
            runtimeAssetInfo.tokenSource.Cancel();
            runtimeAssetInfo.tokenSource.Dispose();
            runtimeAssetInfo.tokenSource = null;
        }

        //ABBUtil.Log($"卸载 asset - {assetID} - {runtimeAssetInfo.obj}");
        _RuntimeAssetInfos.Remove(assetID);
        ClassPoolMgr.Instance.Push(runtimeAssetInfo);

        runtimeAbInfo.RemoveRefCount();
        if (runtimeAbInfo.IsUnLoad())
        {
            UnloadAssetbundle(in abInfo.abID);
        }

    }

    private void UnloadAssetbundle(in int abID)
    {
        if (!_AbInfos.TryGetValue(abID, out var abInfo))
            return;
        if (!_RuntimeAbInfos.TryGetValue(abID, out var runtimeAbInfo))
            return;
        if (runtimeAbInfo.tokenSource != null && !runtimeAbInfo.tokenSource.IsCancellationRequested)
        {
            runtimeAbInfo.tokenSource.Cancel();
            runtimeAbInfo.tokenSource.Dispose();
            runtimeAbInfo.tokenSource = null;
            Debug.LogError("UnloadAssetbundle");

            if (runtimeAbInfo.abRequest != null && runtimeAbInfo.abRequest.assetBundle != null)
            {
                runtimeAbInfo.abRequest.assetBundle.Unload(true);
                runtimeAbInfo.abRequest = null;
            }
        }
        else
        {
            runtimeAbInfo.ab.Unload(true);
        }

        //ABBUtil.Log($"卸载 ab - {abID}");
        _RuntimeAbInfos.Remove(abID);
        ClassPoolMgr.Instance.Push(runtimeAbInfo);

        foreach (var depAbID in abInfo.dependAbList)
        {
            if (!_RuntimeAbInfos.TryGetValue(depAbID, out var depAbInfo))
                continue;
            depAbInfo.RemoveRefCount();
            if (depAbInfo.IsUnLoad())
                UnloadAssetbundle(in depAbID);
        }

    }
    private async UniTask<AssetBundle> LoadAssetbundleAsync(int abID)
    {
        if (_RuntimeAbInfos.TryGetValue(abID, out var runtimeAbInfo))
        {
            var curInfoId = runtimeAbInfo.infoID;
            runtimeAbInfo.AddRefCount();
            if (runtimeAbInfo.tokenSource != null)
                await UniTask.WaitUntil(() => runtimeAbInfo.tokenSource == null);
            return curInfoId == runtimeAbInfo.infoID ? runtimeAbInfo.ab : null;
        }
        if (!_AbInfos.TryGetValue(abID, out var abInfo))
            return null;
        //ABBUtil.Log($"请求加载 ab - {abID}");

        var unitaskList = new List<UniTask>();
        foreach (var depAbID in abInfo.dependAbList)
        {
            var unitask = LoadAssetbundleAsync(depAbID);
            //if (_RuntimeAbInfos.TryGetValue(depAbID, out var depAbInfo))
            //    depAbInfo.AddRefCount();
            unitaskList.Add(unitask);
        }

        runtimeAbInfo = ClassPoolMgr.Instance.Pull<RuntimeAbInfo>();
        runtimeAbInfo.tokenSource = new CancellationTokenSource();
        var abFileHandle = AssetBundle.LoadFromFileAsync(_AbPathMap[abID]);
        var abLoadHandle = abFileHandle.ToUniTask(cancellationToken: runtimeAbInfo.tokenSource.Token);
        unitaskList.Add(abLoadHandle);
        runtimeAbInfo.infoID = ABBUtil.GetTempKey();
        _RuntimeAbInfos.Add(abID, runtimeAbInfo);
        runtimeAbInfo.AddRefCount();
        runtimeAbInfo.abRequest = abFileHandle;
        try
        {
            await UniTask.WhenAll(unitaskList);
            runtimeAbInfo.ab = abFileHandle.assetBundle;
            runtimeAbInfo.tokenSource.Dispose();
            runtimeAbInfo.tokenSource = null;
        }
        catch (OperationCanceledException ex)
        {
            Debug.Log($"家在取消 {ex.Message}");
        }

        return abFileHandle.assetBundle;
    }
    //public void LoadExcelAsync(EnLoadTarget loadTarget)
    //{
    //    AssetbundleUtil.GetAbCatalogLocalPath
    //}
}