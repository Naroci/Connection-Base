# 📡 Network / Connection Handler

A base C# (.NET) library featuring a simple, event-based **Client/Server handler structure**.
Designed for quick and easy implementation of network communication between clients and servers.

## 🎯 Project Goals

The goal of this library is to provide:

* A straightforward and easy-to-use framework for TCP communication.
* Basic features for sending and receiving **text**, **data**, **files**, and more.
* A minimal yet extendable foundation for network-driven applications.

---

## 🧠 Shared.Core

The core of the project resides in the `Shared.Core` component.
It serves as the main **framework library** required to build a client/server-based structure.

---

## 🧪 Sample Projects

Sample implementations can be found in the following projects:

* `Client`: A minimalistic console application that connects to a server.
* `Server`: A console-based server application.
* `NetUI`, `NetUI.Desktop`, `NetUI.Android`: UI-based demo projects using **Avalonia**, demonstrating how to integrate the library into graphical applications.

> `Client` is the most basic example, while the `NetUI` projects illustrate integration in multi-platform UI apps.

---

## 🛠 Hosting a Server

Hosting a TCP server is straightforward.
Simply create an instance of `HostConnection`, call `.Start(port)` to bind a port, and then call `.Listen()` to start accepting connections.

All incoming packages and connections are handled automatically through **event listeners**.

### Example:

```csharp
namespace Server;

class Program
{
    static void Main(string[] args)
    {
        Console.Clear();
        Console.Title = ".:{[ Server ]}:.";
   
        HostConnection server = new();
        server.Start(45678);
        server.Listen();
    }
}
```

---

## 🔌 Connecting with a Client

Creating a client connection is just as simple.
Instantiate a `ClientConnection`, then connect to a target host and port using `.Connect()`.
You can subscribe to events like `OnPackageReceived` to process incoming data.

### Example:

```csharp
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
        Console.WriteLine($"[{package.GetTimestamp().TimeOfDay}, {package.GetId()}] Received:" +
                          $"\n  ID      : {package.GetShortId()} (Short)" +
                          $"\n  Size    : {package.GetContentSize()}" +
                          $"\n  Content : [{package.GetContentAsString()}]");
    }
}
```

---

## 📦 Features (WIP)

* [x] Basic TCP client/server handler
* [x] Event-driven communication
* [x] String and binary transmission
* [ ] File transfer support *(planned)*
* [ ] Authentication layer *(planned)*

---

## 📁 Structure Overview

```plaintext
├── Shared.Core          # Core networking logic
├── Client               # Minimal console-based client example
├── Server               # Console-based TCP server example
├── NetUI.*              # UI sample apps (Avalonia)
```

---

## 💬 Feedback & Contributions

Feel free to open an issue or pull request if you'd like to contribute or have ideas for improvements!
