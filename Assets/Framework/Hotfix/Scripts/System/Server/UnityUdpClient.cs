using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class UnityUdpClient : MonoBehaviour
{
    [SerializeField] private string serverIp = "127.0.0.1";
    [SerializeField] private int serverPort = 6598;
    [SerializeField] private int localPort = 0; // 0 表示自动分配
    [SerializeField] private string _SendMessage = null;

    private UdpClient client;
    private IPEndPoint serverEndpoint;
    private bool isRunning;

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            SendMessage2(_SendMessage);
        }
    }

    private void Initialize()
    {
        try
        {
            serverEndpoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);
            client = new UdpClient(localPort);

            isRunning = true;
            Debug.Log($"UDP 客户端已初始化，监听端口: {((IPEndPoint)client.Client.LocalEndPoint).Port}");

            // 开始接收消息
            _ = ReceiveMessagesAsync();
        }
        catch (Exception ex)
        {
            Debug.LogError($"初始化失败: {ex.Message}");
        }
    }

    // 发送消息
    public async void SendMessage2(string message)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            int bytesSent = await client.SendAsync(data, data.Length, serverEndpoint);
            Debug.Log($"已发送 {bytesSent} 字节");
        }
        catch (Exception ex)
        {
            Debug.LogError($"发送失败: {ex.Message}");
        }
    }

    // 接收消息
    private async Task ReceiveMessagesAsync()
    {
        try
        {
            while (isRunning)
            {
                var result = await client.ReceiveAsync();
                string message = Encoding.UTF8.GetString(result.Buffer);

                // 在主线程处理消息
                OnMessageReceived(message, result.RemoteEndPoint);
            }
        }
        catch (Exception ex)
        {
            if (isRunning)
            {
                Debug.LogError($"接收失败: {ex.Message}");
                isRunning = false;
            }
        }
    }

    // 处理接收到的消息
    private void OnMessageReceived(string message, IPEndPoint sender)
    {
        Debug.Log($"收到来自 {sender} 的消息: {message}");
        // 处理游戏逻辑
    }

    private void OnDestroy()
    {
        isRunning = false;
        client?.Close();
        Debug.Log("UDP 客户端已关闭");
    }
}