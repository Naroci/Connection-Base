using System.Net;
using ClassLibrary1.Connection.Client;
using ClassLibrary1.Connection.Host.MessageEvent;

namespace ClassLibrary1.Connection.Host;

public interface IHostConnection
{
    bool GetIfStarted();
    
    Action<HostMessageEvent> OnDataReceived { get; set; }
    
    List<IClientConnection> GetClients();
    
    void AddClient(IClientConnection client);
    
    void RemoveClient(IClientConnection client);
    
    EndPoint GetEndPointByClient(IClientConnection client);
    
    IClientConnection GetClientByEndPoint(EndPoint endPoint);
    
    void ClearClients();
    
    void Start(int port);
    
    void Start();
    
    void Stop();
    
    void Broadcast(object obj, IClientConnection client);
    void Broadcast(byte[] objBytes, IClientConnection client);
    void Broadcast(string obj, IClientConnection client);
    
    void Send(object obj, IClientConnection client);
    
    void Send(byte[] objBytes, IClientConnection client);

    void Send(string obj, IClientConnection client);
    
    T Receive<T>(IClientConnection client);
    
    byte[] ReceiveBytes(IClientConnection client);
    
    string ReceiveString(IClientConnection client);
}