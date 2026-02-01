using Victoria.Mobile.Models;
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
    private async void OnUnenrollClicked(object sender, EventArgs e)
    {
        try
        {
            if (sender is not Button btn)
                return;

            if (btn.BindingContext is not EnrollmentListItemModel item)
            {
                await DisplayAlert("Error", "Cannot resolve enrollment from UI context.", "Close");
                return;
            }

            var enrollmentId = item.Id;

            var ok = await DisplayAlert("Confirm", "Unenroll from this course group?", "Yes", "No");
            if (!ok) return;

            await _service.UnenrollMyAsync(enrollmentId);

            await DisplayAlert("OK", "Unenrolled ✅", "Close");
            await LoadAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "Close");
        }
    }
}
