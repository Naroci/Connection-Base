using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace NetUI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private bool isShuttingdown = false;

    private async void Window_OnClosing(object? sender, WindowClosingEventArgs e)
    {
        if (isShuttingdown)
            return;

        isShuttingdown = true;
        if (this.MainView != null)
            this.MainView.Terminate();
        
        if (Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }
}