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
        if (this.TimestampLabel != null)
            TimestampLabel.Content = package.GetTimestamp().ToString("HH:mm",CultureInfo.DefaultThreadCurrentCulture);

        if (this.messageContent != null)
            messageContent.Text = package.GetContentAsString();
    }
}