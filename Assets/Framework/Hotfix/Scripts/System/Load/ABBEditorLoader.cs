
#if UNITY_EDITOR

using UnityEngine;
using Cysharp.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

public class ABBEditorLoader : IABBAssetLoader2
{
    private readonly Dictionary<int, AssetCfg> _AssetDic = new();
    public async UniTask Initialization()
    {
        await UniTask.DelayFrame(1);

        var path = GameSchedule.GetCfgPath<AssetCfg>();
        var file = await File.ReadAllTextAsync(path);
        var assetCfgs = JsonConvert.DeserializeObject<AssetCfg[]>(file);
        for (int i = 0; i < assetCfgs.Length; i++)
        {
            var assetCfg = assetCfgs[i];
            _AssetDic.Add(assetCfg.nAssetID, assetCfg);
        }
    }

    public void Destroy()
    {
        _AssetDic.Clear();
    }

    public async UniTask<T> LoadAssetAsync<T>(int assetID) where T : Object
    {
        var cfg = _AssetDic[assetID];
        var obj = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(cfg.strPath);
        var delay = ABBUtil.GetRange(0, 3);
        await UniTask.DelayFrame(delay);
        if (obj == null)
            return null;
        return obj;
    }

    public void UnloadAsset(in int assetID)
    {
        return;
    }

    public T LoadAsset<T>(int assetID) where T : Object
    {
        var cfg = _AssetDic[assetID];
        var obj = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(cfg.strPath);
        if (obj == null)
            return null;
        return obj;
    }
}
#endif