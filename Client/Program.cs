using System.Net.Sockets;
using System.Text;
using ClassLibrary1.Connection.Client;

namespace ConsoleApp1;

class Program
{
    static void Main(string[] args)
    {
        Thread.Sleep(3000);
        bool running = true;
        ClientConnection connection = new ClientConnection();
        while (running)
        {
            var message = Console.ReadLine();
            running = message != "exit";
            if (!connection.GetIfConnected())
                connection.Connect("localhost", 5555);

            if (connection.GetIfConnected())
            {
                connection.Send(message);
                Console.WriteLine(connection.ReceiveString());
            }
        }
        Console.WriteLine("Ended.");
    }
}