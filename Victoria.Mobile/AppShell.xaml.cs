using Victoria.Mobile.Pages;
using Victoria.Mobile.Services;

namespace Victoria.Mobile;

public partial class AppShell : Shell
{
    private bool _initialized;

    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("case-details", typeof(CaseDetailsPage));
        Routing.RegisterRoute("case-documents", typeof(CaseDocumentsPage));
        Routing.RegisterRoute("case-invoices", typeof(CaseInvoicesPage));
        Routing.RegisterRoute("invoice-details", typeof(InvoiceDetailsPage));
        Routing.RegisterRoute("news-details", typeof(NewsDetailsPage));
        Routing.RegisterRoute("course-details", typeof(CourseDetailsPage));
        Routing.RegisterRoute("group-details", typeof(CourseGroupDetailsPage));

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_initialized) return;
        _initialized = true;

        var token = await TokenStore.GetAsync();
        SetLoggedInUI(!string.IsNullOrWhiteSpace(token));

        if (string.IsNullOrWhiteSpace(token))
        {
            SetLoggedInUI(false);
            await GoToAsync("//public/home");
        }
        else
        {
            SetLoggedInUI(true);
            await GoToAsync("//app/home");
        }
    }

    public async Task SetLoggedInAsync(bool isLoggedIn)
    {
        SetLoggedInUI(isLoggedIn);

        if (isLoggedIn)
            await GoToAsync("//app/home");
        else
            await GoToAsync("//login");
    }

    private void SetLoggedInUI(bool isLoggedIn)
    {
        AppFlyout.IsVisible = isLoggedIn;
        FlyoutBehavior = isLoggedIn ? FlyoutBehavior.Flyout : FlyoutBehavior.Disabled;
    }
}
