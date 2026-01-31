using Victoria.Mobile.Helpers;
using Victoria.Mobile.Models;
using Victoria.Mobile.Services;

namespace Victoria.Mobile.Pages;

[QueryProperty(nameof(CaseId), "id")]
public partial class CaseDocumentsPage : ContentPage
{
    private readonly DocumentsService _service;
    private int _caseId;

    public string CaseId
    {
        set
        {
            if (int.TryParse(value, out var id))
                _caseId = id;
        }
    }

    public CaseDocumentsPage()
    {
        InitializeComponent();
        var api = new ApiClient();
        _service = new DocumentsService(api);
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

            var list = await _service.GetForCaseAsync(_caseId);
            DocsList.ItemsSource = list;
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

    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selected = e.CurrentSelection?.FirstOrDefault();
        if (selected == null) return;

        var doc = (CaseDocumentModel)selected;
        ((CollectionView)sender).SelectedItem = null;

        // otwieramy link do pliku w przegl¹darce/systemowym viewerze
        var baseUrl = AppConfig.BaseUrl.TrimEnd('/');
        var path = doc.File.FilePath?.TrimStart('/') ?? "";
        var url = $"{baseUrl}/{path}";

        await Launcher.Default.OpenAsync(url);
    }
}
