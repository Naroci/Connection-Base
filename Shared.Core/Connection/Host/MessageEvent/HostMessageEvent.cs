using System.Runtime.CompilerServices;
using ClassLibrary1.Connection.Client;

namespace ClassLibrary1.Connection.Host.MessageEvent;

public class HostMessageEvent
{
    private IClientConnection _sender;
    private ConnectionPackage _package;

    public HostMessageEvent(IClientConnection clientSender, ConnectionPackage package)
    {
        this._sender = clientSender;
        this._package = package;
    }

    public IClientConnection GetClient()
    {
        return _sender;
    }

    public ConnectionPackage GetPackage()
    {
        return _package;
    }
}