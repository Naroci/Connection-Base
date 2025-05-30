using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Connection.Shared;

namespace NetUI.Views;

public partial class MessageEntryView : UserControl
{
    public MessageEntryView()
    {
        InitializeComponent();
    }
    
    public MessageEntryView(ConnectionPackage package) : this()
    {
        InitializeComponent();

        if (package == null)
            return;

        MessageM msg = package.GetContentAs<MessageM>();
        
        if (this.TimestampLabel != null)
            TimestampLabel.Content = $"{msg.User} ({package.GetTimestamp().ToString("HH:mm",CultureInfo.DefaultThreadCurrentCulture)})";

        if (this.messageContent != null)
            messageContent.Text = msg.Msg;
    }
}