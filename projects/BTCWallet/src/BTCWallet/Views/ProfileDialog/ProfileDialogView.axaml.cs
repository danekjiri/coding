using Avalonia.Controls;

namespace BTCWallet.Views.ProfileDialog;

public partial class ProfileDialogView : UserControl
{
    public ProfileDialogView()
    {
        InitializeComponent();
        var unameTextBox = this.FindControl<TextBox>("ProfileNameTextBox");
        if (unameTextBox != null)
        {
            unameTextBox.AttachedToVisualTree += (s,e) => unameTextBox.Focus();
        }
    }
}