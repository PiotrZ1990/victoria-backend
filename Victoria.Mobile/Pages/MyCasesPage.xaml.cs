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
        await _vm.LoadAsync();
        ErrorLabel.Text = _vm.Error ?? "";
    }

    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        await _vm.LoadAsync();
        ErrorLabel.Text = _vm.Error ?? "";
    }

    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selected = e.CurrentSelection?.FirstOrDefault();
        if (selected == null) return;

        // u Ciebie element listy ma Id
        dynamic item = selected;
        int id = (int)item.Id;

        ((CollectionView)sender).SelectedItem = null;

        await Shell.Current.GoToAsync($"{nameof(CaseDetailsPage)}?id={id}");
    }
}
