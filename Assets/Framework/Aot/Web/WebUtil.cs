using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class WebUtil
{
    public static async UniTask<byte[]> DownloadFileAsync(string localPath)
    {
        var url = Path.Combine(GlobalConfigSO.Instance.DownloadRootUrl, localPath);
        var uri = new Uri(url);
        using var webRep = UnityWebRequest.Get(uri);

        var reqOperation = webRep.SendWebRequest();

        try
        {
            Debug.Log($"开始下载 -> {url}");
            var result = await reqOperation;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }

        if (webRep.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"下载失败 {url}, {webRep.error}");
            return new byte[0];
        }
        return webRep.downloadHandler.data;
    }
    public static async UniTask<bool> UploadContent(string uploadLocalPath, byte[] data)
    {
        var path = Path.Combine(GlobalConfigSO.Instance.ServerDataPath, uploadLocalPath);
        Debug.Log($"上传成功: {path}");
        var dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        await File.WriteAllBytesAsync(path, data);
        return true;
    }
}
