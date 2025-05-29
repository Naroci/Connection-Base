using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ClassLibrary1.Connection.Client;

public class ClientConnection : IClientConnection
{
    private Guid _uid = Guid.NewGuid();
    public Guid GetUniqueIdentifier()
    {
        return _uid;
    }

    private ConnectionStatus _connectionStatus;
    public ConnectionStatus GetConnectionStatus() => _connectionStatus;

    private void SetConnectionStatus(ConnectionStatus connectionStatus)
    {
        _connectionStatus = connectionStatus;
    }

    private readonly TcpClient _tcpClient;
    private NetworkStream getStream() => _tcpClient.GetStream();
    
    
    public ClientConnection()
    {
        _tcpClient = new TcpClient();
        //_stream = _tcpClient.GetStream();
    }

    public ClientConnection(TcpClient client)
    {
        _tcpClient = client;
        //_stream = _tcpClient.GetStream();
    }

    public EndPoint GetEndpoint() => _tcpClient.Client.RemoteEndPoint;

    public void Connect(string ip, int port)
    {
        _tcpClient.Connect(ip, port);
       // if (_tcpClient.Connected)
       //     _stream = _tcpClient.GetStream();
        
    }

    public EndPoint GetLocalEndPoint() => _tcpClient.Client.LocalEndPoint;

    public bool GetIfConnected() => _tcpClient.Connected;

    public void Send(object obj)
    {
        var json = JsonSerializer.Serialize(obj);
        Send(Encoding.UTF8.GetBytes(json));
    }

    public void Send(byte[] objBytes)
    {
        SetConnectionStatus(ConnectionStatus.Sending);
        if (getStream() != null)
            getStream() .Write(objBytes, 0, objBytes.Length);
        SetConnectionStatus(ConnectionStatus.Waiting);
    }

    public void Send(string obj)
    {
        var data = Encoding.UTF8.GetBytes(obj);
        Send(data);
    }

    public T Receive<T>()
    {
        var json = ReceiveString();
        if (string.IsNullOrEmpty(json)) return default;
        return JsonSerializer.Deserialize<T>(json);
    }

    public byte[] ReceiveBytes()
    {
        SetConnectionStatus(ConnectionStatus.Listening);
        byte[] buffer = new byte[4096];
        if (getStream()  == null) return Array.Empty<byte>();
        
        int bytesRead = getStream() .Read(buffer, 0, buffer.Length);
        SetConnectionStatus(ConnectionStatus.Waiting);
        return buffer.Take(bytesRead).ToArray();
    }

    public string ReceiveString()
    {
        var bytes = ReceiveBytes();
        if (bytes.Length == 0) return string.Empty;
        return Encoding.UTF8.GetString(bytes);
    }

    public void Disconnect()
    {
        if (getStream()  != null)
            getStream().Close();
        
        _tcpClient?.Close();
    }
}