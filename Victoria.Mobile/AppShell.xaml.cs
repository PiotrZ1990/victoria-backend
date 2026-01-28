using Victoria.Mobile.Pages;

namespace Victoria.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(MyCasesPage), typeof(MyCasesPage));
        Routing.RegisterRoute(nameof(CaseDetailsPage), typeof(CaseDetailsPage));
    }
}
