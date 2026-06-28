
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IABBAssetLoader
{
    public void Initialization();
    public void Destroy();
    public UniTask<int> LoadAssetAsync<T>(string assetPath, CancellationTokenSource cancellation)
        where T: Object;
    public int LoadAsset<T>(string assetPath)
        where T: Object;
    public void UnloadAsset(int obj);
    public Object GetObject(int objID);
}

public interface IABBAssetLoader2
{
    public UniTask Initialization();
    public void Destroy();
    public UniTask<T> LoadAssetAsync<T>(int assetID)
        where T : Object;

    public T LoadAsset<T>(int assetID)
        where T : Object;
    public void UnloadAsset(in int assetID);
}