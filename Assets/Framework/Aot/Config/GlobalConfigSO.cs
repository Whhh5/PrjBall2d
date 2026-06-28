using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu(menuName = "SO/GlobalConfigSO", fileName = "GlobalConfigSO")]
public class GlobalConfigSO : ScriptableObject
{
    [Header("下载根路径")]
    public string DownloadRootUrl = $"http://localhost:8000/RunGame";

    [Header("dll 加载顺序")]
    public string[] HotfixList = null;

    [Header("dll ab 名字")]
    public string HotfixAssetbundleName = "hotfixdlls.ab";

    [Header("config ab 名字")]
    public string ConfigAssetbundleName = "gamecfgjson.ab";

    [Header("dll 元数据 ab字 名")]
    public string HotfixMatadataAssetbundleName = "hotfixmetadatadlls.ab";

    [Header("服务器文件地址")]
    public string ServerDataPath = "/Users/qiuxiaohui/Desktop/MyServer/Server/Data/RunGame";

    [Header("dll bytes 目录")]
    public string DllBytesDirectory = "Assets/Abbresources/HotfixDlls";
    [Header("dll matadata bytes 目录")]
    public string DllMatadataBytesDirectory = "Assets/Abbresources/HotfixMetadataDlls";

    [SerializeField]
    private EnLoaderType _LoaderType = EnLoaderType.Editor;
    public EnLoaderType LoaderType
    {
        get
        {
#if UNITY_EDITOR
            return _LoaderType;
#else
            return EnLoaderType.Assetbundle;
#endif
        }
        set
        {
            _LoaderType = value;
        }
    }

    private static GlobalConfigSO _Instance = null;
    public static GlobalConfigSO Instance
    {
        get
        {
            if (_Instance == null)
            {
                var path = Path.Combine("SO", "GlobalConfigSO");
                _Instance = Resources.Load<GlobalConfigSO>(path);
            }
            return _Instance;
        }
    }

}
