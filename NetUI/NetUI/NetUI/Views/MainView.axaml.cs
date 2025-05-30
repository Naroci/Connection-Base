using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Connection.Shared;
using Connection.Shared.Connection.Client;

namespace NetUI.Views;

public partial class MainView : UserControl
{
    public void Terminate()
    {
        if (connection != null && connection.GetIfConnected())
        {
            connection.Disconnect();
            connection = null;
        }
        
        return;
    }
    
    

    private ClientConnection connection = null;
    public MainView()
    {
        InitializeComponent();
        connection = new ClientConnection();
        connection.OnMessageReceived += OnMessageReceived;
    }

    private async void OnMessageReceived(ConnectionPackage obj)
    {
        if (this.entries == null || obj == null)
            return;
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            this.entries.Children.Add(new MessageEntryView(obj)); 
            ScrollViewer.Offset = new Vector(ScrollViewer.Offset.X, ScrollViewer.Extent.Height);
        });
    }

    private void SendButtonClicked(object? sender, RoutedEventArgs e)
    {
        if (connection == null || this.connection.GetIfConnected() == false)
        {
            return;
        }

        if (EntryBox != null && !string.IsNullOrEmpty(EntryBox.Text)) 
            connection.Send(new MessageM() { Msg = EntryBox.Text, User = UsernameBox.Text});

        this.EntryBox.Text = string.Empty;
    }

    private async void ConnectButtonClicked(object? sender, RoutedEventArgs e)
    {
        if (this.IpBox == null || this.portBox == null)
            return;
        
        if (connection == null)
            connection = new ClientConnection();

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            connection.Connect(this.IpBox.Text, int.Parse(this.portBox.Text));
            if (connection.GetIfConnected())
                this.StatusLabel.Content = "Connected";
            else
                this.StatusLabel.Content = "Disconnected";
        });
       
    }
}

public class MessageM
{
    public string Msg { get; set; }
    public string User { get; set; }
}