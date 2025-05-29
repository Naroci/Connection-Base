using System.Net;
using System.Net.Sockets;
using ClassLibrary1.Connection.Client;

namespace ClassLibrary1.Connection.Host;

public class HostConnection : IHostConnection
{
    private TcpListener _listener;
    private readonly List<IClientConnection> _clients = new();
    private bool _isRunning;

    public bool GetIfStarted() => _isRunning;

    public List<IClientConnection> GetConnectedClients() => _clients;

    public void AddClient(IClientConnection client)
    {
        if (!_clients.Contains(client))
        {   
            _clients.Add(client);
            Console.WriteLine("Client added");
        }
        else
        {
            Console.WriteLine("Client already in list");
        }
    }

    public void RemoveClient(IClientConnection client)
    {
        if (_clients.Contains(client))
            _clients.Remove(client);
    }

    public EndPoint GetEndPointByClient(IClientConnection client) => client.GetEndpoint();

    public IClientConnection GetClientByEndPoint(EndPoint endPoint) =>
        _clients.FirstOrDefault(c => c.GetEndpoint().Equals(endPoint));

    public void ClearClients() => _clients.Clear();

    public void Start(int port)
    {
        _listener = new TcpListener(port);
        _listener.Start();
        Console.WriteLine("Server started on port: " + port);
        _isRunning = true;
        Thread acceptThread = new(() =>
        {
            while (_isRunning)
            {
                var tcpClient = _listener.AcceptTcpClient();
                var client = new ClientConnection(tcpClient);
                AddClient(client);
            }
        });
        acceptThread.IsBackground = true;
        acceptThread.Start();
    }

    public void Start() => Start(5555);

    public void Stop()
    {
        _isRunning = false;
        _listener?.Stop();
        foreach (var client in _clients) client.Disconnect();
        _clients.Clear();
    }

    public void Listen()
    {
        while (_isRunning)
        {
            foreach (var client in _clients)
            {
                if (client.GetIfConnected() )
                {
                    if (client.GetConnectionStatus() == ConnectionStatus.Waiting)
                    {
                        Thread clientListenerTread = new Thread(ListenOnThread);
                        clientListenerTread.Start(client);
                    }
                }
                else
                {
                    RemoveClient(client);
                }
            }
            Thread.Sleep(1000);
        }
    }

    private void ListenOnThread(object clientObj)
    {
        if (clientObj is IClientConnection client && client.GetIfConnected())
        {
            var identifier = client.GetUniqueIdentifier();
            var messageReceived = ReceiveString(client);
            Console.WriteLine($"{identifier}: {messageReceived}");
            if (!string.IsNullOrEmpty(messageReceived))
            {
                
                Send($"Confirmed {identifier}: [{messageReceived}]",client);
            }
        }
    }

    public void Broadcast(object obj)
    {
        foreach (var client in _clients)
            client.Send(obj);
    }

    public void Broadcast(byte[] objBytes)
    {
        foreach (var client in _clients)
            client.Send(objBytes);
    }

    public void Broadcast(string obj)
    {
        foreach (var client in _clients)
            client.Send(obj);
    }

    public void Send(object obj, IClientConnection client) => client.Send(obj);

    public void Send(byte[] objBytes, IClientConnection client) => client.Send(objBytes);

    public void Send(string obj, IClientConnection client) => client.Send(obj);

    public T Receive<T>(IClientConnection client) => client.Receive<T>();

    public byte[] ReceiveBytes(IClientConnection client) => client.ReceiveBytes();

    public string ReceiveString(IClientConnection client) => client.ReceiveString();
}