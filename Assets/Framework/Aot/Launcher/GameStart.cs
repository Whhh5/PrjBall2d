using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using WeChatWASM;

public enum EnLoaderType
{
    Editor,
    Assetbundle,
    EnumCount,
}
public interface IUpdateGame
{
    public UniTask InitializationAsync();
}

public class GameStart : MonoBehaviour
{
    private Dictionary<EnLoaderType, IUpdateGame> _UpdateGameController = new();


    private void Awake()
    {
        WX.InitSDK(sdkID=>
        {
            Debug.LogWarning($"WXSDKID: {sdkID}");
        });
        WX.ReportGameStart();

        _UpdateGameController.Add(EnLoaderType.Assetbundle, new HotfixMgr());
        _UpdateGameController.Add(EnLoaderType.Editor, new HotfixTest());
    }
    private async void Start()
    {
        var controller = _UpdateGameController[GlobalConfigSO.Instance.LoaderType];
        await controller.InitializationAsync();

        var type = AotUtility.GetClassesWithGameStartAttribute(typeof(GameStartAttribute));
        var go = new GameObject(type.FullName, type);
        GameObject.DontDestroyOnLoad(go);

    }

}
