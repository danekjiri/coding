using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Platform;
using BTCWallet.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BTCWallet.ViewModels.WalletProfile
{
    /// <summary>
    /// Base ViewModel for managing the wallet profile interface.
    /// </summary>
    public partial class WalletProfileBaseViewModel : ViewModelBase
    {
        public readonly int TabId;
        private readonly long _walletId;
        private bool _isReadOnly;

        /// <summary>
        /// Gets the heading of the tab.
        /// </summary>
        public string TabHeading { get; init; }

        /// <summary>
        /// Gets the list of menu items for the wallet profile.
        /// </summary>
        public ObservableCollection<WalletProfileMenuItem> WalletProfileMenuItemsList { get; private set; } = new();

        [ObservableProperty] private int _selectedTabIndex;
        [ObservableProperty] private bool _isPaneOpen = true;
        [ObservableProperty] private ViewModelBase? _currentContent;
        [ObservableProperty] private WalletProfileMenuItem? _selectedMenuItem;

        /// <summary>
        /// Called when the selected menu item changes.
        /// </summary>
        /// <param name="value">The newly selected menu item.</param>
        partial void OnSelectedMenuItemChanged(WalletProfileMenuItem? value)
        {
            if (value is null)
                return;
            var instance = Activator.CreateInstance(value.ModelType, _walletId);
            CurrentContent = (ViewModelBase?)instance ?? CurrentContent;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WalletProfileBaseViewModel"/> class.
        /// </summary>
        /// <param name="tabId">The ID of the tab.</param>
        /// <param name="tabHeading">The heading of the tab.</param>
        /// <param name="walletId">The ID of the wallet.</param>
        public WalletProfileBaseViewModel(int tabId, string tabHeading, long walletId)
        {
            TabId = tabId;
            TabHeading = tabHeading;
            _walletId = walletId;
            Initialize(walletId);
        }

        /// <summary>
        /// Initializes the view model by loading wallet data and menu items.
        /// </summary>
        /// <param name="walletId">The ID of the wallet.</param>
        private async void Initialize(long walletId)
        {
            _isReadOnly = await IsWalletReadOnlyAsync(walletId);

            WalletProfileMenuItemsList =
            [
                new("home", typeof(WalletProfileHomeViewModel)),
                new("addresses", typeof(WalletProfileAddressesViewModel)),
                new("receive", typeof(WalletProfileReceiveViewModel))
            ];
            if (!_isReadOnly)
                WalletProfileMenuItemsList.Add(new WalletProfileMenuItem("send", typeof(WalletProfileSendViewModel)));

            CurrentContent = new WalletProfileHomeViewModel(walletId);
        }

        /// <summary>
        /// Toggles the visibility of the pane.
        /// </summary>
        [RelayCommand]
        private void TriggerPane() => IsPaneOpen = !IsPaneOpen;

        /// <summary>
        /// Opens a new profile tab in a new window.
        /// </summary>
        [RelayCommand]
        public static void OpenNewProfileTab()
        {
            var win = new Window
            {
                Height = 550,
                Width = 800,
                MinHeight = 520,
                MinWidth = 800,
                Content = new ViewLocator().Build(new ProfileDialog.ProfileDialogViewModel()),
                Name = "NewProfileDialog",
                Topmost = true,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.NoChrome,
                ExtendClientAreaToDecorationsHint = true
            };
            win.Show();

            MainWindowViewModel.GetMainWindow()!.IsEnabled = false;
        }

        /// <summary>
        /// Closes the current tab.
        /// </summary>
        [RelayCommand]
        private void CloseCurrentTab()
        {
            MainWindowViewModel.RemoveTabById(TabId);
        }
    }
}
