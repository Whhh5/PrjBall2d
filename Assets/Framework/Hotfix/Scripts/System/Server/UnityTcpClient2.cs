using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Protocol;
using UnityEngine;

public interface INetCliectReceive
{
    public void OnNetMessageReceived(string message);
    public void OnNetConnected();
    public void OnNetDisconnected();
    public void OnNetError(string errorMsg);
}

public class ServerListQueue
{
    private byte[] _List = null;
    private int _HeadIndex;
    private int _RearIndex;
    private readonly int _Capacity = 4;
    private readonly int _HeadSize = 4;
    public int GetCount()
    {
        if (_RearIndex > _HeadIndex)
            return _RearIndex - _HeadIndex;
        else
            return _List.Length - _HeadIndex + _RearIndex;
    }
    public int GetDataCount()
    {
        return GetCount() - _HeadSize;
    }
    public ServerListQueue(int capacity)
    {
        _Capacity = capacity;
        _List = new byte[capacity];
        _HeadIndex = 0;
        _RearIndex = 0;
        _HeadSize = ProtocolUtil.HeadSize;
    }
    public void Push(in byte value)
    {
        _List[_RearIndex] = value;
        _RearIndex = (_RearIndex + 1) % _List.Length;
        CheckSize();
    }
    public void AppendRange(byte[] arr, in int start, in int count)
    {
        for (int i = start; i < count; i++)
        {
            Push(in arr[i]);
        }
    }
    public bool TryGetHeadSize(out int size)
    {
        size = -1;
        if (GetCount() < _HeadSize)
            return false;
        size = BitConverter.ToInt32(_List, _HeadIndex);
        return true;
    }
    private void CheckSize()
    {
        if (_RearIndex != _HeadIndex)
            return;

        var oldList = _List;
        _List = new byte[oldList.Length + _Capacity];
        for (int i = 0; i < oldList.Length; i++)
        {
            _List[i] = oldList[(_HeadIndex + i) % oldList.Length];
        }
        _HeadIndex = 0;
        _RearIndex = oldList.Length;
        //Array.Copy(oldList, _List, oldList.Length);
    }
    public void GetNextRangeArray(int count, ref byte[] bytes)
    {
        if (bytes.Length < count)
            bytes = new byte[count];

        for (int i = 0; i < count; i++)
        {
            var index = (_HeadIndex + i + _HeadSize) % _List.Length;
            bytes[i] = _List[index];
        }

        _HeadIndex = (_HeadIndex + count + _HeadSize) % _List.Length;
    }
}
public class UnityTcpClient2: IUnityClient
{
    [SerializeField] private string serverIp = "192.168.1.110";
    [SerializeField] private int serverPort = 6598;

    private Socket _clientSocket;
    private bool _isConnected;
    private CancellationTokenSource _receiveCancellationToken;

    public INetCliectReceive _NetCliectReceive = null;

    private ServerListQueue _ServerBufferCache = new(ProtocolUtil.SendSize * 2);
    private byte[] _TempByteArray = new byte[ProtocolUtil.SendSize];

    public void Initialization(INetCliectReceive netCliectReceive)
    {
        _NetCliectReceive = netCliectReceive;
    }
    public void OnDestroy()
    {
        _NetCliectReceive = null;
    }

    public bool GetIsConnected()
    {
        return _isConnected;
    }

    // 检查连接状态
    //public bool IsConnected => _isConnected && _clientSocket != null && _clientSocket.Connected;


    // 连接到服务器
    public async UniTask ConnectAsync()
    {
        try
        {
            if (_isConnected)
            {
                Debug.LogWarning("已经连接到服务器");
                return;
            }

            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _clientSocket.SendBufferSize = ushort.MaxValue;
            _clientSocket.ReceiveBufferSize = ushort.MaxValue;
            await _clientSocket.ConnectAsync(serverIp, serverPort);

            _isConnected = true;
            _receiveCancellationToken = new CancellationTokenSource();

            // 触发连接成功事件
            _NetCliectReceive.OnNetConnected();

            Debug.LogWarning("------>>> 服务器连接");
            // 开始接收消息
            await ReceiveMessagesAsync();
    }
        catch (Exception ex)
        {
            // 触发错误事件
            _NetCliectReceive.OnNetError($"连接失败: {ex.Message}");
            Disconnect();
}
    }

    // 接收服务器消息
    private async Task ReceiveMessagesAsync()
    {
        byte[] buffer = new byte[ProtocolUtil.SendSize];

        try
        {
            while (_isConnected && !_receiveCancellationToken.Token.IsCancellationRequested)
            {
                // 检查连接状态
                if (!_clientSocket.Connected)
                {
                    Disconnect();
                    break;
                }

                // 异步接收数据
                int bytesRead = await _clientSocket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None, _receiveCancellationToken.Token);

                if (bytesRead == 0)
                {
                    // 连接已关闭
                    Disconnect();
                    break;
                }

                _ServerBufferCache.AppendRange(buffer, 0, in bytesRead);

                CheckoutMessage();
        }
        }
        catch (OperationCanceledException)
        {
            // 正常取消
        }
        catch (SocketException ex)
        {
            // 触发错误事件
            UnityMainThreadDispatcher.Enqueue(() =>
            {
                _NetCliectReceive.OnNetError($"接收消息失败: {ex.Message}");
            });

            Disconnect();
        }
        catch (Exception ex)
        {
            // 触发错误事件
            UnityMainThreadDispatcher.Enqueue(() =>
            {
                _NetCliectReceive.OnNetError($"接收消息时发生未知错误: {ex.Message}");
            });

            Disconnect();
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

    // 发送消息到服务器
    public async UniTask SendMessageAsync(string message)
    {
        try
        {
            if (!_isConnected || !_clientSocket.Connected)
            {
                _NetCliectReceive.OnNetError("未连接到服务器");
                return;
            }

            byte[] data = Encoding.UTF8.GetBytes(message);
            await _clientSocket.SendAsync(new ArraySegment<byte>(data), SocketFlags.None);
        }
        catch (SocketException ex)
        {
            _NetCliectReceive.OnNetError($"发送消息失败: {ex.Message}");
            Disconnect();
        }
        catch (Exception ex)
        {
            _NetCliectReceive.OnNetError($"发送消息时发生未知错误: {ex.Message}");
            Disconnect();
        }
    }

    // 断开连接
    public void Disconnect()
    {
        try
        {
            _receiveCancellationToken?.Cancel();

            if (_clientSocket != null)
            {
                if (_clientSocket.Connected)
                    _clientSocket.Shutdown(SocketShutdown.Both);

                _clientSocket.Close();
                _clientSocket = null;
            }

            if (_isConnected)
            {
                _isConnected = false;
                _NetCliectReceive.OnNetDisconnected();
            }
        }
        catch (Exception ex)
        {
            _NetCliectReceive.OnNetError($"断开连接时发生错误: {ex.Message}");
        }
        Debug.LogWarning("断开服务器连接");
    }

}
