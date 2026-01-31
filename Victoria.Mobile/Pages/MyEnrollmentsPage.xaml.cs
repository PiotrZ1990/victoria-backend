using Victoria.Mobile.Services;

namespace Victoria.Mobile.Pages;

public partial class MyEnrollmentsPage : ContentPage
{
    private readonly EnrollmentsService _service;

    public MyEnrollmentsPage()
    {
        InitializeComponent();

        var api = new ApiClient();
        _service = new EnrollmentsService(api);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAsync();
    }

    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        try
        {
            ErrorLabel.Text = "";
            var list = await _service.GetMyAsync();
            EnrollmentsList.ItemsSource = list;
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = ex.Message;
        }
    }
}
