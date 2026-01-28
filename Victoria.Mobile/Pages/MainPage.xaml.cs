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
        // ✅ idziemy do My Cases w tej samej sekcji "main"
        await Shell.Current.GoToAsync("//main/cases");
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        TokenStore.Clear();
        await Shell.Current.GoToAsync("//login");
    }
}
