using Victoria.Mobile.Services;

namespace Victoria.Mobile.Pages;

public partial class MainPage : ContentPage
{
    private readonly ApiClient _api = new();

    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnTestApiClicked(object sender, EventArgs e)
    {
        try
        {
            ResultLabel.Text = "Łączę się...";
            var json = await _api.GetStringAsync("swagger/v1/swagger.json");
            ResultLabel.Text = $"API OK ✅ (swagger.json length: {json.Length})";
        }
        catch (Exception ex)
        {
            ResultLabel.Text = $"API FAIL ❌ {ex.Message}";
        }
    }

    private async void OnTestAuthClicked(object sender, EventArgs e)
    {
        try
        {
            ResultLabel.Text = "Testing auth...";
            var json = await _api.GetStringAsync("api/leads"); // jeśli to jest chronione JWT u Ciebie
            ResultLabel.Text = $"AUTH OK ✅ (api/leads length: {json.Length})";
        }
        catch (Exception ex)
        {
            ResultLabel.Text = $"AUTH FAIL ❌ {ex.Message}";
        }
    }
}
