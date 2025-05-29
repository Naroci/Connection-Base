using System.Net.Sockets;
using System.Text;
using System.Text.Unicode;
using Connection.Shared.Connection.Host;

namespace Server;

class Program
{
    static void Main(string[] args)
    {
        Console.Clear();
        Console.Title = ".:{[ Server ]}:.";
   
        HostConnection server = new();
        server.Start(5555);
        server.Listen(250);
    }
}