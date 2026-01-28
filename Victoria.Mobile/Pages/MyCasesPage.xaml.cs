using Victoria.Mobile.Services;
using Victoria.Mobile.ViewModels;

namespace Victoria.Mobile.Pages;

public partial class MyCasesPage : ContentPage
{
    private readonly MyCasesViewModel _vm;

    public MyCasesPage()
    {
        InitializeComponent();

        var api = new ApiClient();
        var service = new CaseFilesService(api);
        _vm = new MyCasesViewModel(service);

        BindingContext = _vm;
        CasesList.ItemsSource = _vm.Items;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Reload();
    }

    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        await Reload();
    }

    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selected = e.CurrentSelection?.FirstOrDefault();
        if (selected == null)
            return;

        var idProp = selected.GetType().GetProperty("Id");
        var idVal = idProp?.GetValue(selected);

        ((CollectionView)sender).SelectedItem = null;

        if (idVal == null || !int.TryParse(idVal.ToString(), out var caseId))
            return;

        await Shell.Current.GoToAsync("case-details", new Dictionary<string, object>
        {
            ["CaseFileId"] = caseId
        });
    }

    private async Task Reload()
    {
        await _vm.LoadAsync();
        ErrorLabel.Text = _vm.Error ?? "";
    }
}
