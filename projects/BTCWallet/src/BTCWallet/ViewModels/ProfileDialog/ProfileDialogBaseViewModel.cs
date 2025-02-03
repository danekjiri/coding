using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;

namespace BTCWallet.ViewModels.ProfileDialog
{
    /// <summary>
    /// Base ViewModel class for profile dialog interactions in the Bitcoin wallet application.
    /// </summary>
    public partial class ProfileDialogBaseViewModel : ViewModelBase
    {
        /// <summary>
        /// Determines whether the Cancel command can execute.
        /// </summary>
        /// <returns>True if there are open tabs in the main window; otherwise, false.</returns>
        private bool CanCancel() => MainWindowViewModel.TabCount > 0;
        
        /// <summary>
        /// Command to cancel the profile dialog and close the profile dialog window.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanCancel))]
        protected void Cancel()
        {
            var profileDialogWindow = 
                from windows in MainWindowViewModel.GetAllWindows()
                where windows.Name == "NewProfileDialog"
                select windows;
            profileDialogWindow.First().Close();
            MainWindowViewModel.GetMainWindow()!.IsEnabled = true;
        }

        /// <summary>
        /// Retrieves the currently open profile dialog window.
        /// </summary>
        /// <returns>The profile dialog window.</returns>
        /// <exception cref="ApplicationException">Thrown when the profile dialog window is not found.</exception>
        protected Window GetOpenProfileDialogWindow()
        {
            var profileDialogWindow =
                (from windows in MainWindowViewModel.GetAllWindows()
                    where windows.Name == "NewProfileDialog"
                    select windows).LastOrDefault();
            if (profileDialogWindow is null)
                throw new ApplicationException("Profile dialog window was not found.");
            return profileDialogWindow;
        }

        /// <summary>
        /// Converts the specified input string to its SHA256 hash representation.
        /// </summary>
        /// <param name="inputString">The input string to hash.</param>
        /// <returns>The SHA256 hash of the input string.</returns>
        // ReSharper disable once InconsistentNaming
        protected static string ToSHA256(string inputString)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(inputString));

            var sb = new StringBuilder();
            // ReSharper disable once InconsistentNaming
            foreach (var B in bytes)
                sb.Append(B.ToString("x2"));
            
            return sb.ToString();
        }
    }
}
