using System.Net.Sockets;
using System.Text;
using System.Text.Unicode;
using ClassLibrary1.Connection.Host;

namespace ConsoleApp2;

class Program
{
    private static bool Running;

    static void Main(string[] args)
    {
        Console.Clear();
        Console.Title = ".:{[ Server ]}:.";
   
        HostConnection server = new();
        server.Start(5555);
        server.Listen(250);
    }
}