using Victoria.Mobile.Pages;

namespace Victoria.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // ROUTY do nawigacji
        Routing.RegisterRoute(nameof(MyCasesPage), typeof(MyCasesPage));
    }
}
