using System.Net.Sockets;
using System.Text;
using Connection.Shared;
using Connection.Shared.Connection.Client;

namespace Client;

class Program
{
    static void Main(string[] args)
    {
        Console.Clear();
        Console.Title = ".:{[ Client ]}:.";

        //Thread.Sleep(3000);
        bool running = true;
        ClientConnection connection = new ClientConnection();
        connection.SetReconnectAttempts(10);
        connection.Connect("localhost", 5555);
        connection.OnPackageReceived += OnPackageReceived;
        while (running)
        {
            var message = Console.ReadLine();
            running = message != "exit";
            if (connection.GetIfConnected())
            {
                connection.Send(message);
                //Console.WriteLine(connection.ReceiveString());
            }
            else
            {
                Console.WriteLine("Not Connected.");
            }
        }
        Console.WriteLine("Ended.");
    }

    static void OnPackageReceived(ConnectionPackage package)
    {
        Console.WriteLine($"[{package.GetTimestamp().TimeOfDay}, {package.GetId()}] Received: " +
                          $"\n ID : {package.GetShortId()} (Short)" +
                          $"\n Size : {package.GetContentSize()}" +
                          $"\n Content : [{package.GetContentAsString()}]");
    }
}