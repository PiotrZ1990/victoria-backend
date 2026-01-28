using Victoria.Mobile.Models;
using Victoria.Mobile.Services;

namespace Victoria.Mobile.Pages;

[QueryProperty(nameof(CaseId), "id")]
public partial class CaseDetailsPage : ContentPage
{
    private readonly CaseFilesService _service;
    private int _caseId;

    public string CaseId
    {
        set
        {
            if (int.TryParse(value, out var id))
            {
                _caseId = id;
            }
        }
    }

    public CaseDetailsPage()
    {
        InitializeComponent();

        var api = new ApiClient();
        _service = new CaseFilesService(api);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        try
        {
            ErrorLabel.Text = "";
            Loader.IsVisible = true;
            Loader.IsRunning = true;

            var dto = await _service.GetByIdAsync(_caseId);
            Render(dto);
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = ex.Message;
        }
        finally
        {
            Loader.IsRunning = false;
            Loader.IsVisible = false;
        }
    }

    private void Render(CaseDetailsModel m)
    {
        ClientNameLabel.Text = string.IsNullOrWhiteSpace(m.ClientName) ? "(no client name)" : m.ClientName;
        StageLabel.Text = $"Stage: {m.Stage ?? "-"}";
        StatusLabel.Text = $"Status: {m.Status ?? "-"}";
        CreatedAtLabel.Text = $"Created: {m.CreatedAt:yyyy-MM-dd HH:mm}";
        CaseIdLabel.Text = $"Case ID: {m.Id}";
    }
}
