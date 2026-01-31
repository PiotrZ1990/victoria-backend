using Victoria.Mobile.Services;

namespace Victoria.Mobile.Pages;

public partial class LoginPage : ContentPage
{
    private readonly AuthService _auth = new();

    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        try
        {
            StatusLabel.Text = "Logging in...";

            var token = await _auth.LoginAsync(
                EmailEntry.Text?.Trim() ?? "",
                PasswordEntry.Text ?? "");

            await TokenStore.SaveAsync(token);

            StatusLabel.Text = "Login OK ✅ Token saved.";

            await ((AppShell)Shell.Current).SetLoggedInAsync(true);

        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Login FAIL ❌ {ex.Message}";
        }
    }

    private void OnClearTokenClicked(object sender, EventArgs e)
    {
        TokenStore.Clear();
        StatusLabel.Text = "Token cleared.";
    }
}
