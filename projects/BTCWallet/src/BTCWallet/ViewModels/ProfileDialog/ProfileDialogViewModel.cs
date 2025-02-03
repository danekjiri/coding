using System.Collections.ObjectModel;
using System.Linq;
using BTCWallet.DataModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BTCWallet.ViewModels.ProfileDialog
{
    /// <summary>
    /// ViewModel for managing profiles in the Bitcoin wallet application.
    /// </summary>
    public partial class ProfileDialogViewModel : ProfileDialogBaseViewModel
    {
        /// <summary>
        /// Collection of existing profiles.
        /// </summary>
        public static ObservableCollection<Profile> ExistingProfiles { get; private set; } = new();

        /// <summary>
        /// Property for new user name input.
        /// </summary>
        [ObservableProperty]
        private string _newUserName = string.Empty;
        
        /// <summary>
        /// Property for new password input.
        /// </summary>
        [ObservableProperty]
        private string _newPassword = string.Empty;

        /// <summary>
        /// Index of the selected existing profile.
        /// </summary>
        [ObservableProperty] 
        private int _selectedExistingProfilesIndex;

        /// <summary>
        /// Default constructor. Initializes the ViewModel and loads existing profiles from the database.
        /// </summary>
        public ProfileDialogViewModel()
        {
            using var db = new WalletDbContext();
            var profiles = db.Profiles;
            ExistingProfiles = new ObservableCollection<Profile>(profiles);
        }

        /// <summary>
        /// Constructor that initializes the ViewModel with a username and password.
        /// </summary>
        /// <param name="username">The new user's username.</param>
        /// <param name="password">The new user's password.</param>
        public ProfileDialogViewModel(string username, string password) : base()
        {
            NewUserName = username;
            NewPassword = password;
        }

        /// <summary>
        /// Called when the new user name changes to notify that the CreateNewProfileCommand can be executed.
        /// </summary>
        /// <param name="value">The new value of the user name.</param>
        partial void OnNewUserNameChanged(string value)
        {
            CreateNewProfileCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// Called when the new password changes to notify that the CreateNewProfileCommand can be executed.
        /// </summary>
        /// <param name="value">The new value of the password.</param>
        partial void OnNewPasswordChanged(string value)
        {
            CreateNewProfileCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// Determines whether a new profile can be created based on the input values and existing profiles.
        /// </summary>
        /// <returns>True if the new profile can be created; otherwise, false.</returns>
        private bool CanCreateNewProfile() => 
            !string.IsNullOrEmpty(NewUserName) && 
            !string.IsNullOrEmpty(NewPassword) &&
            ExistingProfiles.All(p => p.Username != NewUserName);

        /// <summary>
        /// Command to create a new profile.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanCreateNewProfile))]
        private void CreateNewProfile()
        {
            var profileDialogWindow = GetOpenProfileDialogWindow();
            profileDialogWindow.Content = new ViewLocator()
                .Build(new ProfileDialogCreateNewViewModel(NewUserName, NewPassword));
        }

        /// <summary>
        /// Determines whether the selected profile can be opened.
        /// </summary>
        /// <returns>True if a profile can be opened; otherwise, false.</returns>
        private bool CanOpenSelectedProfile() => ExistingProfiles.Count > 0;
        
        /// <summary>
        /// Command to open the selected profile.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanOpenSelectedProfile))]
        private void OpenSelectedProfile()
        {
            var profileDialogWindow = GetOpenProfileDialogWindow();
            profileDialogWindow.Content = new ViewLocator()
                .Build(new ProfileDialogValidateSelectedViewModel(
                    ExistingProfiles[SelectedExistingProfilesIndex].Username
                ));
        }
    }
}
