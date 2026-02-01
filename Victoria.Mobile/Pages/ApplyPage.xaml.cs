using Victoria.Mobile.Models;
using Victoria.Mobile.Services;

namespace Victoria.Mobile.Pages;

public partial class ApplyPage : ContentPage
{
    private readonly LeadsService _service;

    public ApplyPage()
    {
        InitializeComponent();

        var api = new ApiClient();
        _service = new LeadsService(api);
    }

    private async void OnSendClicked(object sender, EventArgs e)
    {
        try
        {
            StatusLabel.Text = "";
            Loader.IsVisible = true;
            Loader.IsRunning = true;

            var fullName = FullNameEntry.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(fullName))
            {
                StatusLabel.Text = "Full name is required.";
                return;
            }

            var req = new LeadCreateRequest
            {
                FullName = fullName,
                Email = EmailEntry.Text?.Trim(),
                PhoneNumber = PhoneEntry.Text?.Trim(),
                Source = "MobileApp",
                InterestedCountry = CountryPicker.SelectedItem?.ToString(),
                InterestedService = ServicePicker.SelectedItem?.ToString()
            };

            await _service.CreateLeadAsync(req);

            await DisplayAlert("Sent ✅", "Thank you! Our staff will contact you soon.", "OK");

            FullNameEntry.Text = "";
            EmailEntry.Text = "";
            PhoneEntry.Text = "";
            CountryPicker.SelectedIndex = -1;
            ServicePicker.SelectedIndex = -1;
        }
        catch (Exception ex)
        {
            StatusLabel.Text = ex.Message;
        }
        finally
        {
            Loader.IsRunning = false;
            Loader.IsVisible = false;
        }
    }
}
