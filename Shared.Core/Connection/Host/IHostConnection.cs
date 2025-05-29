using System.Net;
using ClassLibrary1.Connection.Client;

namespace ClassLibrary1.Connection.Host;

public interface IHostConnection
{
    bool GetIfStarted();
    
    List<IClientConnection> GetConnectedClients();
    
    void AddClient(IClientConnection client);
    
    void RemoveClient(IClientConnection client);
    
    EndPoint GetEndPointByClient(IClientConnection client);
    
    IClientConnection GetClientByEndPoint(EndPoint endPoint);
    
    void ClearClients();
    
    void Start(int port);
    
    void Start();
    
    void Stop();
    
    void Broadcast(object obj);
    void Broadcast(byte[] objBytes);
    void Broadcast(string obj);
    
    void Send(object obj, IClientConnection client);
    
    void Send(byte[] objBytes, IClientConnection client);

    void Send(string obj, IClientConnection client);
    
    T Receive<T>(IClientConnection client);
    
    byte[] ReceiveBytes(IClientConnection client);
    
    string ReceiveString(IClientConnection client);
}