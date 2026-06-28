using UnityEngine;

using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Security.Policy;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using System.Net.WebSockets;
using System.Threading;
using Protocol;

public class UnityWebClient : IUnityClient
{
    [SerializeField] private string serverIp = "127.0.0.1";
    [SerializeField] private int serverPort = 6598;

    private ClientWebSocket webSocket;
    private CancellationTokenSource cancellationTokenSource;
    public bool isConnected { get; private set; }

    private ServerListQueue _ServerBufferCache = new(ProtocolUtil.SendSize * 2);
    private byte[] _TempByteArray = new byte[ProtocolUtil.SendSize];

    private INetCliectReceive _NetCliectReceive = null;

    public void Initialization(INetCliectReceive netCliectReceive)
    {
        _NetCliectReceive = netCliectReceive;
    }


    public bool GetIsConnected()
    {
        return isConnected;
    }

    // 连接到服务器
    public async UniTask ConnectAsync()
    {
        try
        {
            // WebSocket 地址格式：ws://IP:端口 或 wss://IP:端口（加密）
            string webSocketUrl = $"ws://{serverIp}:{serverPort}/ws";
            webSocket = new ClientWebSocket();
            cancellationTokenSource = new CancellationTokenSource();

            // 建立 WebSocket 连接
            await webSocket.ConnectAsync(new Uri(webSocketUrl), cancellationTokenSource.Token);
            isConnected = true;
            Debug.Log("WebSocket 连接成功");

            // 启动接收消息的循环（单独线程，避免阻塞）
            // 启动接收消息的循环（单独线程，避免阻塞）
            _ = ReceiveLoop();
        }
        catch (Exception ex)
        {
            isConnected = false;
            Debug.LogError($"WebSocket 连接失败: {ex.Message}");
        }
    }

    // 接收消息循环
    private async UniTask ReceiveLoop()
    {
        byte[] buffer = new byte[1024 * 4]; // 缓冲区大小根据需求调整
        while (webSocket.State == WebSocketState.Open && !cancellationTokenSource.Token.IsCancellationRequested)
        {
            try
            {
                // 接收服务器消息
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    cancellationTokenSource.Token
                );

                switch (result.MessageType)
                {
                    case WebSocketMessageType.Binary:
                    case WebSocketMessageType.Text:
                        {
                            // 处理文本消息（示例：转换为字符串）
                            //string message = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
                            Debug.Log($"收到消息 length: {result.Count}");
                            // TODO: 在这里添加消息处理逻辑

                            _ServerBufferCache.AppendRange(buffer, 0, result.Count);

                            CheckoutMessage();
                        }
                        break;
                    case WebSocketMessageType.Close:
                        {
                            // 服务器请求关闭连接
                            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "关闭连接", CancellationToken.None);
                            isConnected = false;
                            Debug.Log("服务器关闭连接");
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                //isConnected = false;
                Debug.LogError($"接收消息错误: {ex.Message}");
                break;
            }
        }
    }
    public void CheckoutMessage()
    {
        if (!_ServerBufferCache.TryGetHeadSize(out var size))
            return;
        if (_ServerBufferCache.GetDataCount() < size)
            return;

        _ServerBufferCache.GetNextRangeArray(size, ref _TempByteArray);

        var message = Encoding.UTF8.GetString(_TempByteArray, 0, size);
        // 在主线程触发消息接收事件
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            _NetCliectReceive.OnNetMessageReceived(message);
        });
    }

    // 发送消息（文本类型）
    public async UniTask SendMessageAsync(string message)
    {
        if (!isConnected || webSocket?.State != WebSocketState.Open)
        {
            Debug.LogError("未连接或连接已关闭，无法发送消息");
            return;
        }

        try
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(
                new ArraySegment<byte>(data),
                WebSocketMessageType.Text,
                true,
                cancellationTokenSource.Token
            );
        }
        catch (Exception ex)
        {
            Debug.LogError($"发送消息失败: {ex.Message}");
        }
    }

    // 断开连接
    public void Disconnect()
    {
        if (isConnected)
        {
            isConnected = false;
            cancellationTokenSource?.Cancel(); // 取消接收循环
            webSocket?.CloseAsync(WebSocketCloseStatus.NormalClosure, "主动关闭连接", CancellationToken.None).Wait();
            Debug.Log("已断开 WebSocket 连接");
        }

        webSocket?.Dispose();
        cancellationTokenSource?.Dispose();
        webSocket = null;
        cancellationTokenSource = null;
    }
    public void OnDestroy()
    {
        
    }
}
