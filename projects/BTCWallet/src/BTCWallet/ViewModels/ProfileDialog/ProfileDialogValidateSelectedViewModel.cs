using System.Linq;
using BTCWallet.DataModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BTCWallet.ViewModels.ProfileDialog
{
    /// <summary>
    /// ViewModel for validating the selected profile during the login process.
    /// </summary>
    public partial class ProfileDialogValidateSelectedViewModel : ProfileDialogBaseViewModel
    {
        [ObservableProperty] private string _username;
        [ObservableProperty] private string? _typedPassword;

        private readonly string? _validPasswordHash;
        private readonly long? _walletId;

        [ObservableProperty]
        private string? _typedPasswordHash;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileDialogValidateSelectedViewModel"/> class.
        /// </summary>
        /// <param name="username">The username of the profile being validated.</param>
        public ProfileDialogValidateSelectedViewModel(string username)
        {
            _username = username;
            using var db = new WalletDbContext();

            // Retrieve the valid password hash for the given username from the database.
            _validPasswordHash = 
                (from profile in db.Profiles
                 where profile.Username == _username
                 select profile.PasswordHash).FirstOrDefault();

            // Retrieve the wallet ID for the given username from the database.
            _walletId = 
                (from profile in db.Profiles
                 where profile.Username == _username
                 select profile.WalletId).FirstOrDefault();
        }

        /// <summary>
        /// Event handler for when the typed password changes.
        /// </summary>
        /// <param name="value">The new typed password value.</param>
        partial void OnTypedPasswordChanged(string? value)
        {
            LogInCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// Command to navigate back to the profile selection dialog.
        /// </summary>
        [RelayCommand]
        private void Back()
        {
            var profileDialogWindow = GetOpenProfileDialogWindow();
            profileDialogWindow.Content = new ViewLocator()
                .Build(new ProfileDialogViewModel());
        }

        /// <summary>
        /// Determines whether the Log In command can execute.
        /// </summary>
        /// <returns>True if the typed password is correct; otherwise, false.</returns>
        private bool CanLogIn()
        {
            if (TypedPassword is null)
                return false;

            TypedPasswordHash = ToSHA256(TypedPassword);
            return TypedPasswordHash == _validPasswordHash;
        }

        /// <summary>
        /// Command to log in and open the wallet profile tab.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanLogIn))]
        private void LogIn()
        {
            MainWindowViewModel.AddTab(Username, _walletId!.Value);
            Cancel();
        }
    }
}
