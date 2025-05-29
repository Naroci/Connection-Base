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
        HostConnection server = new();
        server.Start(5555);
        server.Listen(250);
    }
    /*
    private static void StartHandleClient(object client)
    {
       if (client is not TcpClient tcpClient) return;

       while (Running && tcpClient.Connected)
       {
           var currentStream = tcpClient.GetStream();
           byte[] buffer = new byte[1024];
           if (currentStream.CanRead)
           {
               currentStream.Read(buffer, 0, buffer.Length);

               string message = UTF8Encoding.UTF8.GetString(buffer);
               Console.WriteLine(message);
           }
       }
    }*/
}