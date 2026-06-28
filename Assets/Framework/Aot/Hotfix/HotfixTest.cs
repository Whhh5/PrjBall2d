using Cysharp.Threading.Tasks;
using UnityEngine;

public class HotfixTest : IUpdateGame
{
    public async UniTask InitializationAsync()
    {
        await UniTask.Yield();
    }
}
