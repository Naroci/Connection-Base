using System.Net;

namespace ClassLibrary1.Connection.Client;

public interface IClientConnection
{
    Action<ConnectionPackage> OnMessageReceived { get; set; }

    // 0 - endless!
    void SetReconnectAttempts(int attempt = 0);
    
    // default 2000ms
    void SetReconnectAttemptTiming(int timing = 2000);

    void Listen();
    
    
    Guid GetUniqueIdentifier();

    ConnectionStatus GetConnectionStatus();

    EndPoint GetEndpoint();

    void Disconnect();

    void Connect(string ip, int port);

    EndPoint GetLocalEndPoint();

    bool GetIfConnected();

    void Send(object obj);

    void Send(byte[] objBytes);

    void Send(string obj);

    T Receive<T>();

    byte[] ReceiveBytes();

    string ReceiveString();
}

public enum ConnectionStatus
{
    Waiting,
    Listening,
    Sending
}