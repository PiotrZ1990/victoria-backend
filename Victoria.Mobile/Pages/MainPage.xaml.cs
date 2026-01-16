using Victoria.Mobile.Services;

namespace Victoria.Mobile.Pages;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnOpenCasesClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(MyCasesPage));
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        TokenStore.Clear();
        await Shell.Current.GoToAsync("//LoginPage");
    }
}
