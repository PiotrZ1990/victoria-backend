using Victoria.Mobile.Services;
using Victoria.Mobile.ViewModels;

namespace Victoria.Mobile.Pages;

[QueryProperty(nameof(CaseFileId), "CaseFileId")]
public partial class CaseDetailsPage : ContentPage
{
    private readonly CaseDetailsViewModel _vm;

    public int CaseFileId { get; set; }

    public CaseDetailsPage()
    {
        InitializeComponent();

        var api = new ApiClient();
        var service = new CaseFilesService(api);
        _vm = new CaseDetailsViewModel(service);

        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync(CaseFileId);
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
