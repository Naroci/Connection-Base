using System.Net.Sockets;
using System.Text;
using ClassLibrary1.Connection.Client;

namespace ConsoleApp1;

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
        connection.OnMessageReceived += package =>
        {
            Console.WriteLine($"[{package.GetTimestamp().TimeOfDay}, {package.GetId()}] Received: " +
                              $"\n ID : {package.GetShortId()} (Short)" +
                              $"\n Size : {package.GetContentSize()}" +
                              $"\n Content : [{package.GetContentAsString()}]");
        };
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
}