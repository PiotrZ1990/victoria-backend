using Victoria.Mobile.Pages;
using Victoria.Mobile.Services;

namespace Victoria.Mobile;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
    }

    protected override async void OnStart()
    {
        base.OnStart();

        var token = await TokenStore.GetAsync();
        if (string.IsNullOrWhiteSpace(token))
            await Shell.Current.GoToAsync("//LoginPage");
        else
            await Shell.Current.GoToAsync("//MainPage");
    }
}
