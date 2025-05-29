using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ClassLibrary1.Connection.Client;

public class ClientConnection : IClientConnection
{
    private Guid _uid = Guid.NewGuid();
    private int reconnectAttempts = 0;
    private int currentAttempt = 0;

    private bool _reconnectEnabled = true;

    private bool networkStreamAvailable = false;

    private bool NetworkStreamAvailable
    {
        get => networkStreamAvailable;
        set
        {
            networkStreamAvailable = value;
            if (value == false && ReconnectEnabled == true)
            {
                StartReconnectJob();
            }
        
        }
    }

    public bool ReconnectEnabled
    {
        get => _reconnectEnabled;
        set { _reconnectEnabled = value; }
    }

    private int reconnectAttemptTiming = 2000;

    private string host;
    private int port;


    public void SetReconnectAttempts(int attempt = 0)
    {
        reconnectAttempts = attempt;
    }

    public void SetReconnectAttemptTiming(int timing = 2000)
    {
        reconnectAttemptTiming = timing;
    }

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

    private TcpClient _tcpClient;

    private NetworkStream getStream()
    {
        if (_tcpClient == null || !GetIfConnected())
        {
            return null;
        }

        return _tcpClient.GetStream();
    }

    private bool reconnectJobRunning = false;

    private void StartReconnectJob()
    {
        if (reconnectJobRunning == false)
        {
            reconnectJobRunning = true;
            Thread reconnectThread = new Thread(ReconnectJob);
            reconnectThread.Start();
        }
    }

    private void ReconnectJob()
    {
        currentAttempt = 0;
        bool running = true;
        reconnectAttemptTiming = reconnectAttemptTiming > 50 ? reconnectAttemptTiming : 50;
        while (running)
        {
            Thread.Sleep(reconnectAttemptTiming);
            if (reconnectAttempts == 0 || reconnectAttempts > 0 && currentAttempt <= reconnectAttempts)
            {
                if (GetIfConnected())
                {
                    running = false;
                    break;
                }

                Console.WriteLine($"Reconnecting... ({currentAttempt})");
                try
                {
                    Reconnect(this.host, this.port);
                }
                catch (Exception ex)
                {
                }

                running = !GetIfConnected();
            }

            this.currentAttempt++;
            running = reconnectAttempts == 0 || reconnectAttempts > 0 && currentAttempt < reconnectAttempts;
            
        }
        Console.WriteLine($"Reconnection ended... ({currentAttempt} attempts total)");
        reconnectJobRunning = false;
    }


    public ClientConnection()
    {
        _tcpClient = new TcpClient();
    }

    public ClientConnection(TcpClient client)
    {
        _tcpClient = client;
    }

    public EndPoint GetEndpoint() => _tcpClient.Client.RemoteEndPoint;

    public void Connect(string ip, int port)
    {
        this.host = ip;
        this.port = port;

        if (_tcpClient == null)
            _tcpClient = new TcpClient();

        _tcpClient.Connect(this.host, this.port);
        NetworkStreamAvailable = _tcpClient.Connected;
    }

    public void Reconnect(string ip, int port)
    {
        this.host = ip;
        this.port = port;

        if (_tcpClient != null)
            _tcpClient.Dispose();

        _tcpClient = new TcpClient();

        _tcpClient.Connect(this.host, this.port);
        NetworkStreamAvailable = _tcpClient.Connected;
        if (_tcpClient != null && _tcpClient.Connected)
        {
            Console.WriteLine("Reconnected!");
        }
    }

    public EndPoint GetLocalEndPoint() => _tcpClient.Client.LocalEndPoint;

    public bool GetIfConnected()
    {
        if ((_tcpClient == null || !NetworkStreamAvailable) && _reconnectEnabled || (!_tcpClient.Connected  || !NetworkStreamAvailable) && _reconnectEnabled)
        {
            StartReconnectJob();
            return false;
        }

        return _tcpClient.Connected;
    }


    public void Send(object obj)
    {
        var json = JsonSerializer.Serialize(obj);
        Send(Encoding.UTF8.GetBytes(json));
    }

    public void Send(byte[] objBytes)
    {
        SetConnectionStatus(ConnectionStatus.Sending);
        try
        {
            if (getStream() != null)
                getStream().Write(objBytes, 0, objBytes.Length);

            NetworkStreamAvailable = true;
        }
        catch (Exception ex)
        {
            NetworkStreamAvailable = false;
        }

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
        try
        {
            byte[] buffer = new byte[4096];
            if (getStream() == null) return Array.Empty<byte>();

            int bytesRead = getStream().Read(buffer, 0, buffer.Length);
            NetworkStreamAvailable = true;
            SetConnectionStatus(ConnectionStatus.Waiting);
            return buffer.Take(bytesRead).ToArray();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            NetworkStreamAvailable = false;
            SetConnectionStatus(ConnectionStatus.Waiting);
        }

        return null;
    }


    public string ReceiveString()
    {
        var bytes = ReceiveBytes();
        if (bytes != null && bytes.Length == 0) 
            return string.Empty;
        
        if (bytes == null)
            return string.Empty;
        
        return Encoding.UTF8.GetString(bytes);
    }

    public void Disconnect()
    {
        if (getStream() != null)
            getStream().Close();

        _tcpClient?.Close();
    }
}