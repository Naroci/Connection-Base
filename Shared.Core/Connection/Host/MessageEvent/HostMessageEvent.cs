using System.Runtime.CompilerServices;
using Connection.Shared.Connection.Client;

namespace Connection.Shared.Connection.Host.MessageEvent;

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