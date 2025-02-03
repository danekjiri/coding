using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using BTCWallet.ViewModels.WalletProfile;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BTCWallet.ViewModels
{
    /// <summary>
    /// ViewModel for the main window of the BTC Wallet application.
    /// </summary>
    public partial class MainWindowViewModel : ViewModelBase
    {
        private static int _tabIdentifierIndex;

        /// <summary>
        /// Collection of wallet profile tabs.
        /// </summary>
        [ObservableProperty] private static ObservableCollection<WalletProfileBaseViewModel> _tabs = new();

        /// <summary>
        /// Gets the number of open tabs.
        /// </summary>
        public static int TabCount => _tabs.Count;

        /// <summary>
        /// Removes a tab by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the tab to remove.</param>
        public static void RemoveTabById(int id)
        {
            int searchTabIndex = -1;
            for (int i = 0; i < _tabs.Count; i++)
            {
                if (_tabs[i].TabId == id)
                {
                    searchTabIndex = i;
                    break;
                }
            }

            if (searchTabIndex != -1)
                _tabs.RemoveAt(searchTabIndex);
            if (_tabs.Count == 0)
                WalletProfileBaseViewModel.OpenNewProfileTab();
        }

        /// <summary>
        /// Adds a new tab with the specified heading and wallet ID.
        /// </summary>
        /// <param name="tabHeading">The heading of the new tab.</param>
        /// <param name="walletId">The wallet ID associated with the new tab.</param>
        public static void AddTab(string tabHeading, long walletId)
        {
            _tabs.Add(new WalletProfileBaseViewModel(_tabIdentifierIndex++, tabHeading, walletId));
        }

        /// <summary>
        /// Gets the main window of the application.
        /// </summary>
        /// <returns>The main window.</returns>
        public static Window? GetMainWindow()
        {
            return ((IClassicDesktopStyleApplicationLifetime)Application.Current?.ApplicationLifetime!).MainWindow;
        }

        /// <summary>
        /// Gets all windows of the application.
        /// </summary>
        /// <returns>A read-only list of all windows.</returns>
        public static IReadOnlyList<Window> GetAllWindows()
        {
            return ((IClassicDesktopStyleApplicationLifetime)Application.Current?.ApplicationLifetime!).Windows;
        }
    }
}
