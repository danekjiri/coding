using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BTCWallet.DataModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BTCWallet.ViewModels.WalletProfile
{
    /// <summary>
    /// ViewModel for exporting wallet profile information.
    /// </summary>
    public partial class WalletProfileExportViewModel : ViewModelBase
    {
        private readonly long _walletId;

        /// <summary>
        /// Gets the collection of wallet mnemonics.
        /// </summary>
        public ObservableCollection<string>? WalletMnemonics { get; private set; }

        [ObservableProperty] private Task<string>? _walletDerivationPath;
        [ObservableProperty] private Task<string>? _walletXPub;
        [ObservableProperty] private Task<string>? _walletPassphrase;

        [ObservableProperty] private bool _isSeeded;
        [ObservableProperty] private bool _isWithPassphrase;

        /// <summary>
        /// Initializes a new instance of the <see cref="WalletProfileExportViewModel"/> class.
        /// </summary>
        /// <param name="walletId">The wallet ID associated with this view model.</param>
        public WalletProfileExportViewModel(long walletId)
        {
            _walletId = walletId;
            Initialize();
        }

        /// <summary>
        /// Initializes the view model by loading wallet data.
        /// </summary>
        private async void Initialize()
        {
            WalletMnemonics = new ObservableCollection<string>(await GetWalletsMnemonicArrayAsync(_walletId));
            WalletDerivationPath = GetWalletDerivationPathAsync(_walletId);
            WalletPassphrase = GetWalletPassphraseAsync(_walletId);
            WalletXPub = GetWalletXPubAsync(_walletId);
            await IsSeededWallet(_walletId);
        }

        /// <summary>
        /// Determines if the wallet is seeded.
        /// </summary>
        /// <param name="walletId">The wallet ID to check.</param>
        private async Task IsSeededWallet(long walletId)
        {
            IsSeeded = !(await IsWalletReadOnlyAsync(walletId));
        }

        /// <summary>
        /// Retrieves the extended public key (xpub) of the wallet.
        /// </summary>
        /// <param name="walletId">The wallet ID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the wallet xpub.</returns>
        private async Task<string> GetWalletXPubAsync(long walletId)
        {
            await using var db = new WalletDbContext();
            var wallet = await db.FindAsync<Wallet>(walletId);
            var xpub = wallet!.XpubWif;
            return xpub;
        }

        /// <summary>
        /// Retrieves the passphrase of the wallet.
        /// </summary>
        /// <param name="walletId">The wallet ID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the wallet passphrase.</returns>
        private async Task<string> GetWalletPassphraseAsync(long walletId)
        {
            await using var db = new WalletDbContext();
            var wallet = await db.FindAsync<Wallet>(walletId);
            var passphrase = wallet?.Passphrase;
            if (passphrase is null)
            {
                return string.Empty;
            }

            IsWithPassphrase = true;
            return passphrase;
        }

        /// <summary>
        /// Retrieves the derivation path of the wallet.
        /// </summary>
        /// <param name="walletId">The wallet ID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the wallet derivation path.</returns>
        private async Task<string> GetWalletDerivationPathAsync(long walletId)
        {
            await using var db = new WalletDbContext();
            var wallet = await db.FindAsync<Wallet>(walletId);
            var derivationPath = wallet?.Derivation;
            if (derivationPath is null)
                return string.Empty;
            return derivationPath;
        }

        /// <summary>
        /// Retrieves the mnemonic array of the wallet.
        /// </summary>
        /// <param name="walletId">The wallet ID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the wallet mnemonics as an array.</returns>
        private async Task<string[]> GetWalletsMnemonicArrayAsync(long walletId)
        {
            await using var db = new WalletDbContext();
            var wallet = await db.FindAsync<Wallet>(walletId);
            var mnemonicsArray = wallet?.Mnemonic?.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (mnemonicsArray is null)
                return new string[12];
            return mnemonicsArray;
        }
    }
}
