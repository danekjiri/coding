using System.Collections.ObjectModel;
using BTCWallet.DataModels;

namespace BTCWallet.ViewModels.WalletProfile
{
    /// <summary>
    /// ViewModel for managing the wallet profile addresses.
    /// </summary>
    public class WalletProfileAddressesViewModel : ViewModelBase
    {
        private readonly long _walletId;

        /// <summary>
        /// Gets or sets the collection of receive addresses.
        /// </summary>
        public ObservableCollection<Address> ReceiveAddresses { get; set; } = new();

        /// <summary>
        /// Gets or sets the collection of change addresses.
        /// </summary>
        public ObservableCollection<Address> ChangeAddresses { get; set; } = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="WalletProfileAddressesViewModel"/> class.
        /// </summary>
        /// <param name="walletId">The ID of the wallet.</param>
        public WalletProfileAddressesViewModel(long walletId)
        {
            _walletId = walletId;
            Initialize();
        }

        /// <summary>
        /// Initializes the view model by loading receive and change addresses.
        /// </summary>
        private async void Initialize()
        {
            ReceiveAddresses = new ObservableCollection<Address>(
                await GetAddressesListAsync(areChangeAddresses: false, _walletId));
            ChangeAddresses = new ObservableCollection<Address>(
                await GetAddressesListAsync(areChangeAddresses: true, _walletId));
        }
    }
}
