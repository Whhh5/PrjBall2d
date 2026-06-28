using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ABBLoadMgr : Singleton<ABBLoadMgr>
{
    public override EnSingletonOrder SingletonOrder => EnSingletonOrder.Load;
    private Dictionary<EnLoaderType, IABBAssetLoader2> _LoaderList = new();

    public override async UniTask OnEnableAsync()
    {
        await base.OnEnableAsync();
    }
    public override void Destroy()
    {
        foreach (var item in _LoaderList)
        {
            item.Value.Destroy();
        }

#if UNITY_EDITOR
        UnregisterLoader<ABBEditorLoader>(EnLoaderType.Editor);
#endif
        UnregisterLoader<AbbAssetbundleLoader>(EnLoaderType.Assetbundle);
        base.Destroy();
    }
    public override async UniTask AwakeAsync()
    {
        await base.AwakeAsync();
#if UNITY_EDITOR
        RegisterLoader(EnLoaderType.Editor, new ABBEditorLoader());
#endif
        RegisterLoader(EnLoaderType.Assetbundle, new AbbAssetbundleLoader());

        var curLoader = GetLoader();
        await curLoader.Initialization();
    }
    private void RegisterLoader(EnLoaderType loadType, IABBAssetLoader2 loader)
    {
        _LoaderList.Add(loadType, loader);
    }
    private void UnregisterLoader<T>(EnLoaderType loadType)
    {
        _LoaderList.Remove(loadType);
    }

    private IABBAssetLoader2 GetLoader(EnLoaderType loadType)
    {
        return _LoaderList[loadType];
    }
    private IABBAssetLoader2 GetLoader()
    {
        return GetLoader(GlobalConfigSO.Instance.LoaderType);
    }
    public async UniTask<T> LoadAsync<T>(EnLoadTarget loadTarget)
        where T : Object
    {
        var result = await LoadAsync<T>((int)loadTarget);
        return result;
    }
    public async UniTask<T> LoadAsync<T>(int assetCfgID)
        where T : Object
    {
        var loader = GetLoader();
        var result = await loader.LoadAssetAsync<T>(assetCfgID);
        return result;
    }

    public T Load<T>(int assetID)
           where T : Object
    {
        var loader = GetLoader();
        var result = loader.LoadAsset<T>(assetID);
        return result;
    }
    public T Load<T>(EnLoadTarget loadTarget)
           where T : Object
    {
        var result = Load<T>((int)loadTarget);
        return result;
    }
    public void Unload(EnLoadTarget loadTarget)
    {
        Unload((int)loadTarget);
    }
    public void Unload(int assetID)
    {
        var loader = GetLoader();
        loader.UnloadAsset(in assetID);
    }
}
