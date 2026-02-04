using Victoria.Mobile.Services;

namespace Victoria.Mobile.Pages;

public partial class SettingsPage : ContentPage
{
    private readonly AuthService _auth = new();

    public SettingsPage()
    {
        InitializeComponent();
    }

    private async void OnChangePasswordClicked(object sender, EventArgs e)
    {
        try
        {
            StatusLabel.TextColor = Colors.Gray;
            StatusLabel.Text = "";

            var current = CurrentPasswordEntry.Text ?? "";
            var newPass = NewPasswordEntry.Text ?? "";
            var confirm = ConfirmPasswordEntry.Text ?? "";

            if (string.IsNullOrWhiteSpace(current) || string.IsNullOrWhiteSpace(newPass))
            {
                StatusLabel.TextColor = Colors.Red;
                StatusLabel.Text = "Fill current and new password.";
                return;
            }

            if (newPass != confirm)
            {
                StatusLabel.TextColor = Colors.Red;
                StatusLabel.Text = "New password and confirm do not match.";
                return;
            }

            StatusLabel.Text = "Changing password...";
            await _auth.ChangePasswordAsync(current, newPass);

            StatusLabel.TextColor = Colors.Green;
            StatusLabel.Text = "Password changed ✅";

            CurrentPasswordEntry.Text = "";
            NewPasswordEntry.Text = "";
            ConfirmPasswordEntry.Text = "";
        }
        catch (Exception ex)
        {
            StatusLabel.TextColor = Colors.Red;
            StatusLabel.Text = ex.Message;
        }
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        TokenStore.Clear();
        await ((AppShell)Shell.Current).SetLoggedInAsync(false);
    }
}
