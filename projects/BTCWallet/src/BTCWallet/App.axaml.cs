using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BTCWallet.DataModels;
using BTCWallet.ViewModels;
using BTCWallet.Views;
using Microsoft.EntityFrameworkCore;
using WalletProfileBaseViewModel = BTCWallet.ViewModels.WalletProfile.WalletProfileBaseViewModel;

namespace BTCWallet;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
            desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
            WalletProfileBaseViewModel.OpenNewProfileTab();
        }
        
        base.OnFrameworkInitializationCompleted();
    }
}
