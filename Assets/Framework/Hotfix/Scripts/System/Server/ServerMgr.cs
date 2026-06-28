using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Protocol;
using UnityEngine;

public interface IUnityClient
{
    public void Initialization(INetCliectReceive netCliectReceive);
    public bool GetIsConnected();
    public void OnDestroy();
    public UniTask ConnectAsync();
    public UniTask SendMessageAsync(string message);
    public void Disconnect();
}
public class ServerMgr : Singleton<ServerMgr>, INetCliectReceive
{
    public override EnManagerFuncType FuncType => base.FuncType | EnManagerFuncType.Update;
    IUnityClient _client = null;
    private Dictionary<int, IServer_RSP> _RSPMap = new();
    private HashSet<int> _WaitSendKey = new();
    public override EnSingletonOrder SingletonOrder => EnSingletonOrder.Server;

    private EnSocketType _SocketType = EnSocketType.Web;

    public string GetServerIp()
    {
        return ProtocolUtil.ServerIp;
    }
    public int GetServerPort()
    {
        return ProtocolUtil.ServerPort;
    }

    private IUnityClient GetUnityClient()
    {
        return _SocketType switch
        {
            EnSocketType.Web => new UnityWebClient(),
             EnSocketType.TCP2 => new UnityTcpClient2(),
            _ => null,
        };
    }

    public override async UniTask AwakeAsync()
    {
        await base.AwakeAsync();

        _client = GetUnityClient();
        _client.Initialization(this);

        await ConnectAsync();

        if (!_client.GetIsConnected())
        {
            Debug.LogError("服务器连接失败");
        }
    }
    public override void Destroy()
    {
        Disconnect();

        _client.OnDestroy();
        _client = null;

        base.Destroy();
    }
    public override void Update()
    {
        base.Update();
        UnityMainThreadDispatcher.Update();
    }
    private async UniTask ConnectAsync()
    {
        _ = _client.ConnectAsync();
        var curTime = Time.time;
        await UniTask.WaitUntil(() => _client.GetIsConnected() || (Time.time - curTime) > 0.1f);
    }

    private void Disconnect()
    {
        // 断开连接
        _client.Disconnect();
    }

    public void OnNetMessageReceived(string message)
    {
        AddMessage($"服务器: {message}");
        var protocolInfo = JsonConvert.DeserializeObject<ProtocolInfo>(message);
        switch (protocolInfo.protocolID)
        {
            case EnProtocol.RSP_DOWNLOAD_FILE:
                {
                    var data = JsonConvert.DeserializeObject<RSP_DOWNLOAD_FILE>(protocolInfo.content);
                    _RSPMap.Add(data.id, data);
                }
                break;
            case EnProtocol.RSP_DOWNLOAD_BYTE:
                {
                    var data = JsonConvert.DeserializeObject<RSP_DOWNLOAD_BYTE>(protocolInfo.content);
                    _RSPMap.Add(data.id, data);
                }
                break;
            default:
                break;
        }
    }

    public async UniTask<string> REQ_DOWNLOAD_FILE(string fileLocalPath)
    {
        var sendKey = ABBUtil.GetTempKey();
        var sendData = new REQ_DOWNLOAD_FILE()
        {
            id = sendKey,
            fileLocalPath = fileLocalPath,
        };
        var info = new ProtocolInfo()
        {
            id = sendKey,
            protocolID = EnProtocol.REQ_DOWNLOAD_FILE,
            content = JsonConvert.SerializeObject(sendData),
        };
        _ = _client.SendMessageAsync(JsonConvert.SerializeObject(info));

        _WaitSendKey.Add(sendKey);
        await UniTask.WaitUntil(() => _RSPMap.ContainsKey(sendKey));
        _WaitSendKey.Remove(sendKey);

        var rspData = _RSPMap[sendKey] as RSP_DOWNLOAD_FILE;
        _RSPMap.Remove(sendKey);

        return rspData.content;
    }
    public async UniTask<byte[]> REQ_DOWNLOAD_BYTE(string fileLocalPath)
    {
        var sendKey = ABBUtil.GetTempKey();
        var sendData = new REQ_DOWNLOAD_BYTE()
        {
            id = sendKey,
            fileLocalPath = fileLocalPath,
        };
        var info = new ProtocolInfo()
        {
            id = sendKey,
            protocolID = EnProtocol.REQ_DOWNLOAD_BYTE,
            content = JsonConvert.SerializeObject(sendData),
        };
        _ = _client.SendMessageAsync(JsonConvert.SerializeObject(info));

        _WaitSendKey.Add(sendKey);
        await UniTask.WaitUntil(() => _RSPMap.ContainsKey(sendKey));
        _WaitSendKey.Remove(sendKey);

        var rspData = _RSPMap[sendKey] as RSP_DOWNLOAD_BYTE;
        _RSPMap.Remove(sendKey);

        return rspData.content;
    }

    public void OnNetConnected()
    {
        AddMessage("已连接到服务器");
    }

    public void OnNetDisconnected()
    {
        AddMessage("已断开与服务器的连接");
    }

    public void OnNetError(string errorMessage)
    {
        AddMessage($"错误: {errorMessage}");
    }

    private void AddMessage(string message)
    {
        Debug.LogWarning(message);

    }
}
