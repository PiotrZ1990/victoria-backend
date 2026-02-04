using Victoria.Mobile.Helpers;
using Victoria.Mobile.Models;
using Victoria.Mobile.Services;

namespace Victoria.Mobile.Pages;

[QueryProperty(nameof(CaseId), "id")]
public partial class CaseDocumentsPage : ContentPage
{
    private readonly DocumentsService _service;
    private int _caseId;
    private string? _pickedPath;
    private readonly ChecklistService _checklists;


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
        _checklists = new ChecklistService(api);

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
        var checklist = await _checklists.GetMyChecklistAsync(_caseId);
        var missing = checklist.Items.Count(x => x.IsRequired && !x.IsCompleted);
        ChecklistSummaryLabel.Text = $"Missing required: {missing}";

        ChecklistList.ItemsSource = checklist.Items
            .OrderBy(x => x.Flow)
            .ThenByDescending(x => x.IsRequired)
            .ThenBy(x => x.DocumentType)
            .ToList();

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
    private async void OnPickFileClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Pick a document"
            });

            if (result == null) return;

            _pickedPath = result.FullPath;
            PickedFileLabel.Text = Path.GetFileName(_pickedPath);
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = ex.Message;
        }
    }

    private async void OnUploadClicked(object sender, EventArgs e)
    {
        try
        {
            ErrorLabel.Text = "";

            if (string.IsNullOrWhiteSpace(_pickedPath))
            {
                ErrorLabel.Text = "Pick a file first.";
                return;
            }

            var title = TitleEntry.Text?.Trim();
            if (string.IsNullOrWhiteSpace(title))
            {
                ErrorLabel.Text = "Title is required.";
                return;
            }

            Loader.IsVisible = true;
            Loader.IsRunning = true;

            await _service.UploadForCaseAsync(_caseId, title!, DescEntry.Text, _pickedPath);

            // reset
            _pickedPath = null;
            PickedFileLabel.Text = "No file selected";
            TitleEntry.Text = "";
            DescEntry.Text = "";

            await LoadAsync(); // reload list
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
    private void OnToggleChecklistClicked(object sender, EventArgs e)
    {
        ChecklistPanel.IsVisible = !ChecklistPanel.IsVisible;
        ToggleChecklistBtn.Text = ChecklistPanel.IsVisible ? "Hide checklist" : "Show checklist";
    }

}
