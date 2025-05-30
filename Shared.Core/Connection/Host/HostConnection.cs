using System.Net;
using System.Net.Sockets;
using Connection.Shared.Connection.Client;
using Connection.Shared.Connection.Host.MessageEvent;

namespace Connection.Shared.Connection.Host;

public class HostConnection : IHostConnection
{
    private TcpListener _listener;
    private readonly List<IClientConnection> _clients = new();
    private bool _isRunning;

    public Action<HostMessageEvent> OnDataReceived { get; set; }

    public bool GetIfStarted() => _isRunning;

    public List<IClientConnection> GetClients()
    {
        return new List<IClientConnection>(_clients);
    }

    public void AddClient(IClientConnection client)
    {
        if (!_clients.Contains(client))
        {
            client.OnMessageReceived += package =>
            {
                var message = new HostMessageEvent(client, package);
                this.OnDataReceived?.Invoke(message);
                OnDataMessageReceived(message);
            };
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
        {
            client.OnMessageReceived -= package => { };
            _clients.Remove(client);
            Console.WriteLine($"[{client.GetUniqueIdentifier()}] - Client removed");
        }
    }

    public EndPoint GetEndPointByClient(IClientConnection client) => client.GetEndpoint();

    public IClientConnection GetClientByEndPoint(EndPoint endPoint) =>
        _clients.FirstOrDefault(c => c.GetEndpoint().Equals(endPoint));

    public void ClearClients() => _clients.Clear();

    public void Start(int port)
    {
        try
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
                    client.ReconnectEnabled = false;
                    AddClient(client);
                }
            });
            acceptThread.IsBackground = true;
            acceptThread.Start();
        }
        catch (Exception ex)
        {
            if (ex is SocketException socketException)
            {
                Console.WriteLine("Could not start the Server (" + socketException.Message + ")");
            }
        }
    }

    public void Start() => Start(5555);

    public void Stop()
    {
        _isRunning = false;
        _listener?.Stop();
        foreach (var client in _clients) client.Disconnect();
        _clients.Clear();
    }

    private bool listening = false;

    private void OnDataMessageReceived(HostMessageEvent message)
    {
        var identifier = message.GetClient().GetUniqueIdentifier();
        var messageReceived = message.GetPackage().GetContentAsString();
        Console.WriteLine($"{identifier}: {messageReceived}");
        if (!string.IsNullOrEmpty(messageReceived))
        {
            Broadcast(messageReceived, message.GetClient());
        }
    }

    public void Listen(int tickrate = 1000)
    {
        while (_isRunning)
        {
            lock (_clients)
            {
                foreach (var client in GetClients())
                {
                    if (client.GetIfConnected())
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

                Thread.Sleep(tickrate);
            }
        }
    }

    private void ListenOnThread(object clientObj)
    {
        if (clientObj is IClientConnection client && client.GetIfConnected())
        {
            var identifier = client.GetUniqueIdentifier();
            client.Listen();
        }
    }

    public void Broadcast(object obj, IClientConnection baseclient)
    {
        baseclient.Send(obj);
        foreach (var client in _clients)
        {
            if (client == baseclient)
                continue;

            client.Send(obj);
        }
    }

    public void Broadcast(byte[] objBytes, IClientConnection baseclient)
    {
        baseclient.Send(objBytes);
        foreach (var client in _clients)
        {
            if (client == baseclient)
                continue;

            client.Send(objBytes);
        }
    }

    public void Broadcast(string obj, IClientConnection baseclient)
    {
        baseclient.Send(obj);
        foreach (var client in _clients)
        {
            if (client == baseclient)
                continue;

            client.Send(obj);
        }
    }

    public void Send(object obj, IClientConnection client) => client.Send(obj);

    public void Send(byte[] objBytes, IClientConnection client) => client.Send(objBytes);

    public void Send(string obj, IClientConnection client) => client.Send(obj);

    public T Receive<T>(IClientConnection client) => client.Receive<T>();

    public byte[] ReceiveBytes(IClientConnection client) => client.ReceiveBytes();

    public string ReceiveString(IClientConnection client) => client.ReceiveString();
}