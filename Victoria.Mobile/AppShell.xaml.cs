using Victoria.Mobile.Helpers;
using Victoria.Mobile.Pages;
using Victoria.Mobile.Services;

namespace Victoria.Mobile;

public partial class AppShell : Shell
{
    private bool _initialized;

    public AppShell()
    {
        InitializeComponent();

        // Route do szczegółów (zakładam, że masz tę stronę)
        Routing.RegisterRoute("case-details", typeof(CaseDetailsPage));
        Routing.RegisterRoute("case-documents", typeof(CaseDocumentsPage));
        Routing.RegisterRoute("case-invoices", typeof(CaseInvoicesPage));
        Routing.RegisterRoute("invoice-details", typeof(InvoiceDetailsPage));

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_initialized) return;
        _initialized = true;

        var token = await TokenStore.GetAsync();
        SetLoggedInUI(!string.IsNullOrWhiteSpace(token));

        if (string.IsNullOrWhiteSpace(token))
            await GoToAsync("//login");
        else
            await GoToAsync("//app/home");
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
