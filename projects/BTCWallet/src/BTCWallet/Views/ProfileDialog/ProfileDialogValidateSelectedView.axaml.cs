using Avalonia.Controls;
using Avalonia.Input;

namespace BTCWallet.Views.ProfileDialog;

public partial class ProfileDialogValidateSelectedView : UserControl
{
    public ProfileDialogValidateSelectedView()
    {
        InitializeComponent();
        var pwdTextBox = this.FindControl<TextBox>("PasswordTextBox");
        if (pwdTextBox != null)
        {
            pwdTextBox.AttachedToVisualTree += (s,e) => pwdTextBox.Focus();
        }
    }
}