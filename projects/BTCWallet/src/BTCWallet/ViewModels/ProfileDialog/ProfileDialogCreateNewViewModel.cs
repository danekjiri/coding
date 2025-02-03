using System;
using System.Linq;
using System.Threading.Tasks;
using BTCWallet.DataModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NBitcoin;

namespace BTCWallet.ViewModels.ProfileDialog
{
    /// <summary>
    /// ViewModel for creating or importing a new profile in the Bitcoin wallet application.
    /// </summary>
    public partial class ProfileDialogCreateNewViewModel : ProfileDialogBaseViewModel
    {
        private readonly string _profileUsername;
        private readonly string _profilePassword;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileDialogCreateNewViewModel"/> class.
        /// </summary>
        /// <param name="username">The username for the new profile.</param>
        /// <param name="password">The password for the new profile.</param>
        public ProfileDialogCreateNewViewModel(string username, string password)
        {
            _profileUsername = username;
            _profilePassword = password;

            FillWalletMnemonicTextBoxes();
            FillWalletDerivationPath();
        }

        private bool _areCorrectMnemonics;

        [ObservableProperty] private bool _isInvalidChecksum;
        [ObservableProperty] private bool _isWithPassphrase;

        /// <summary>
        /// Event handler for when the 'IsWithPassphrase' property changes.
        /// </summary>
        /// <param name="value">The new value of the 'IsWithPassphrase' property.</param>
        partial void OnIsWithPassphraseChanged(bool value)
        {
            if (!value)
                ClearWalletPassphrase();
        }

        [ObservableProperty] private bool _isImport;

        /// <summary>
        /// Event handler for when the 'IsImport' property changes.
        /// </summary>
        /// <param name="value">The new value of the 'IsImport' property.</param>
        partial void OnIsImportChanged(bool value)
        {
            if (!value)
            {
                IsReadOnly = false;
                CreateOrImportWalletButton = "CREATE new wallet";

                FillWalletMnemonicTextBoxes();
                FillWalletDerivationPath();
            }
            else
            {
                CreateOrImportWalletButton = "IMPORT new wallet";

                ClearWalletMnemonicTextBoxes();
                ClearWalletDerivationPath();
                ClearROWalletXPub();
            }

            MnemonicEnabled = value;
        }

        [ObservableProperty] private bool _isReadOnly;

        /// <summary>
        /// Event handler for when the 'IsReadOnly' property changes.
        /// </summary>
        /// <param name="value">The new value of the 'IsReadOnly' property.</param>
        partial void OnIsReadOnlyChanged(bool value)
        {
            if (value)
            {
                ClearWalletMnemonicTextBoxes();
                ClearWalletDerivationPath();
                ClearWalletPassphrase();
            }
            else
            {
                ClearROWalletXPub();
            }

            MnemonicEnabled = !value;
            PassphraseEnabled = !value;
            DerivationEnabled = !value;
            XPubEnabled = value;
        }

        [ObservableProperty] private bool _mnemonicEnabled;
        [ObservableProperty] private bool _derivationEnabled = true;
        [ObservableProperty] private bool _xPubEnabled;
        [ObservableProperty] private bool _passphraseEnabled = true;

        [ObservableProperty] private string _passphrase = string.Empty;
        [ObservableProperty] private string _derivationPath = string.Empty;

        /// <summary>
        /// Event handler for when the 'DerivationPath' property changes.
        /// </summary>
        /// <param name="value">The new value of the 'DerivationPath' property.</param>
        partial void OnDerivationPathChanged(string value)
        {
            CreateOrImportWalletCommand.NotifyCanExecuteChanged();
        }

        [ObservableProperty] private string _xPub = string.Empty;

        /// <summary>
        /// Event handler for when the 'XPub' property changes.
        /// </summary>
        /// <param name="value">The new value of the 'XPub' property.</param>
        partial void OnXPubChanged(string value)
        {
            CreateOrImportWalletCommand.NotifyCanExecuteChanged();
        }

        [ObservableProperty] private string[] _typedMnemonics = new string[12];
        [ObservableProperty] private string _createOrImportWalletButton = "CREATE new wallet";

        /// <summary>
        /// Determines whether the CreateOrImportWallet command can execute.
        /// </summary>
        /// <returns>True if the command can execute; otherwise, false.</returns>
        private bool CanCreateOrImportWallet()
        {
            if (IsImport)
            {
                if (IsReadOnly)
                {
                    return IsValidXPubFormat(XPub);
                }
                else // default import
                {
                    return _areCorrectMnemonics && IsValidDerivationPathFormat(DerivationPath);
                }
            }
            else // creating new wallet
            {
                return IsValidDerivationPathFormat(DerivationPath);
            }
        }

        /// <summary>
        /// Validates the format of the specified derivation path.
        /// </summary>
        /// <param name="derivationPath">The derivation path to validate.</param>
        /// <returns>True if the format is valid; otherwise, false.</returns>
        private bool IsValidDerivationPathFormat(string derivationPath)
        {
            return KeyPath.TryParse(derivationPath, out var _);
        }

        /// <summary>
        /// Validates the format of the specified extended public key (xPub).
        /// </summary>
        /// <param name="xPubString">The xPub to validate.</param>
        /// <returns>True if the format is valid; otherwise, false.</returns>
        private bool IsValidXPubFormat(string? xPubString)
        {
            if (xPubString is null)
                return false;
            try
            {
                ExtPubKey.Parse(xPubString, BTCNetwork);
            }
            catch (FormatException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Command to create or import a wallet.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanCreateOrImportWallet))]
        private async Task CreateOrImportWallet()
        {
            Wallet wallet = new Wallet();
            if (IsImport)
            {
                if (IsReadOnly)
                {
                    var masterPubKey = ExtPubKey.Parse(XPub, BTCNetwork);
                    wallet.IsReadOnly = 1;
                    wallet.XpubWif = XPub;
                    CreateAndLinkWalletWithProfile(wallet);
                    await ReadOnlyWalletAddressesGenAsync(masterPubKey, wallet.WalletId);
                }
                else // default import
                {
                    var masterKey =
                        new Mnemonic(string.Join(' ', TypedMnemonics.ToArray()), Wordlist.English).DeriveExtKey(
                            IsWithPassphrase ? Passphrase : null);
                    wallet = ConstructWallet(isReadOnly: false, TypedMnemonics.ToArray(), masterKey, DerivationPath, Passphrase);
                    CreateAndLinkWalletWithProfile(wallet);
                    await DefaultWalletKeysAndAddressesGenAsync(masterKey, new KeyPath(DerivationPath), wallet.WalletId);
                }
            }
            else // creating new wallet
            {
                var masterKey =
                    new Mnemonic(string.Join(' ', TypedMnemonics.ToArray()), Wordlist.English).DeriveExtKey(
                        IsWithPassphrase ? Passphrase : null);
                wallet = ConstructWallet(isReadOnly: false, TypedMnemonics.ToArray(), masterKey, DerivationPath, Passphrase);
                CreateAndLinkWalletWithProfile(wallet);
                await DefaultWalletKeysAndAddressesGenAsync(masterKey, new KeyPath(DerivationPath), wallet.WalletId);
            }

            MainWindowViewModel.AddTab(_profileUsername, wallet.WalletId);
            Cancel();
        }

        /// <summary>
        /// Creates a wallet and links it with the profile in the database.
        /// </summary>
        /// <param name="wallet">The wallet to create and link.</param>
        private void CreateAndLinkWalletWithProfile(Wallet wallet)
        {
            using var db = new WalletDbContext();
            db.Add(wallet);
            db.SaveChanges();
            
            var profile = new Profile()
            {
                Username = _profileUsername,
                PasswordHash = ToSHA256(_profilePassword),
                WalletId = wallet.WalletId
            };
            db.Add(profile);
            db.SaveChanges();
        }

        /// <summary>
        /// Constructs a new wallet with the specified parameters.
        /// </summary>
        /// <param name="isReadOnly">Indicates whether the wallet is read-only.</param>
        /// <param name="mnemonicWordsArray">The mnemonic words for the wallet.</param>
        /// <param name="masterKey">The master key for the wallet.</param>
        /// <param name="derivationPath">The derivation path for the wallet.</param>
        /// <param name="passphrase">The passphrase for the wallet (optional).</param>
        /// <returns>The constructed wallet.</returns>
        private Wallet ConstructWallet(bool isReadOnly, string[] mnemonicWordsArray, ExtKey masterKey, string derivationPath,
            string? passphrase = null)
        {
            var derivedMasterKey = masterKey.Derive(new KeyPath(derivationPath));
            var newWallet = new Wallet()
            {
                IsReadOnly = isReadOnly ? 1 : 0,
                Mnemonic = string.Join(' ', mnemonicWordsArray),
                Derivation = derivationPath,
                Passphrase = passphrase,
                MasterKeyWif = masterKey.GetWif(BTCNetwork).ToString(),
                XpubWif = new ExtPubKey(
                    derivedMasterKey.GetPublicKey(),
                    derivedMasterKey.ChainCode,
                    (byte)new KeyPath(derivationPath).Length,
                    derivedMasterKey.ParentFingerprint,
                    2147483648
                ).GetWif(BTCNetwork).ToString()
            };
            return newWallet;
        }

        /// <summary>
        /// Command to check and mark invalid mnemonics.
        /// </summary>
        [RelayCommand]
        private void CheckAndMarkInvalidMnemonics()
        {
            var isCorrect = true;
            for (var i = 0; i < TypedMnemonics.Length; i++)
            {
                if (TypedMnemonics[i] is null)
                {
                    isCorrect = false;
                    break;
                }
                
                if (!Wordlist.English.WordExists(TypedMnemonics[i], out var _))
                {
                    TypedMnemonics[i] = "#ERROR";
                    isCorrect = false;
                }
            }

            if (isCorrect)
            {
                IsInvalidChecksum = !(new Mnemonic(Wordlist.English).IsValidChecksum);
            }

            _areCorrectMnemonics = isCorrect;
            TypedMnemonics = TypedMnemonics.ToArray();
            CreateOrImportWalletCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// Command to navigate back in the profile dialog.
        /// </summary>
        [RelayCommand]
        private void Back()
        {
            var profileDialogWindow = GetOpenProfileDialogWindow();
            profileDialogWindow.Content = new ViewLocator()
                .Build(new ProfileDialogViewModel(_profileUsername, _profilePassword));
        }

        /// <summary>
        /// Fills the wallet mnemonic text boxes with generated mnemonics.
        /// </summary>
        private void FillWalletMnemonicTextBoxes()
        {
            var generatedMnemonics = new Mnemonic(Wordlist.English, WordCount.Twelve);
            TypedMnemonics = generatedMnemonics.Words;
        }

        /// <summary>
        /// Fills the wallet derivation path with a default value.
        /// </summary>
        private void FillWalletDerivationPath() => DerivationPath = "m/44'/1'/0'";

        /// <summary>
        /// Clears the wallet mnemonic text boxes.
        /// </summary>
        private void ClearWalletMnemonicTextBoxes() => TypedMnemonics = new string[12];

        /// <summary>
        /// Clears the wallet derivation path.
        /// </summary>
        private void ClearWalletDerivationPath() => DerivationPath = string.Empty;

        /// <summary>
        /// Clears the wallet extended public key (xPub).
        /// </summary>
        private void ClearROWalletXPub() => XPub = string.Empty;

        /// <summary>
        /// Clears the wallet passphrase.
        /// </summary>
        private void ClearWalletPassphrase() => Passphrase = "type passphrase...";

        /// <summary>
        /// Generates default wallet keys and addresses asynchronously.
        /// </summary>
        /// <param name="masterKey">The master key for the wallet.</param>
        /// <param name="kp">The key path for the wallet.</param>
        /// <param name="walletId">The wallet ID.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task DefaultWalletKeysAndAddressesGenAsync(ExtKey masterKey, KeyPath kp, long walletId)
        {
            var masterKeys = new []{masterKey.Derive(kp).Derive(0), masterKey.Derive(kp).Derive(1)};
            uint walletChain = 0;

            foreach (var derivedMasterKey in masterKeys)
            {
                for (uint seq = 0; seq < AutoGeneratedAddressNumber; seq++)
                {
                    await SaveDefaultNewPubKeyAndAddressAsync(derivedMasterKey, seq, walletId, walletChain);
                }

                walletChain++;
            }
        }

        /// <summary>
        /// Generates read-only wallet addresses asynchronously.
        /// </summary>
        /// <param name="extPubKey">The extended public key for the wallet.</param>
        /// <param name="walletId">The wallet ID.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task ReadOnlyWalletAddressesGenAsync(ExtPubKey extPubKey, long walletId)
        {
            var masterPubKeys = new []{extPubKey.Derive(0), extPubKey.Derive(1)};
            uint walletChain = 0;
            
            foreach (var derivedMasterPubKey in masterPubKeys)
            {
                for (uint seq = 0; seq < AutoGeneratedAddressNumber; seq++)
                {
                    await SaveNewROPubKeyAndAddressAsync(derivedMasterPubKey, seq, walletId, walletChain);
                }
            
                walletChain++;
            }
        }
    }
}
