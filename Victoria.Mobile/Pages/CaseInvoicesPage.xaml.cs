using System.Collections.ObjectModel;
using Victoria.Mobile.Models;
using Victoria.Mobile.Services;

namespace Victoria.Mobile.Pages;

[QueryProperty(nameof(CaseId), "id")]
public partial class CaseInvoicesPage : ContentPage
{
    private readonly InvoicesService _service;
    private int _caseId;

    public ObservableCollection<InvoiceListItemModel> Items { get; } = new();

    public string CaseId
    {
        set
        {
            if (int.TryParse(value, out var id))
                _caseId = id;
        }
    }

    public CaseInvoicesPage()
    {
        InitializeComponent();
        _service = new InvoicesService(new ApiClient());
        InvoicesList.ItemsSource = Items;
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

            Items.Clear();
            var list = await _service.GetByCaseAsync(_caseId);
            foreach (var x in list.OrderByDescending(x => x.IssueDate))
                Items.Add(x);
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
        var selected = e.CurrentSelection?.FirstOrDefault() as InvoiceListItemModel;
        if (selected == null) return;

        ((CollectionView)sender).SelectedItem = null;

        // na razie tylko poka¿emy alert - details zrobimy w nastêpnym kroku
        await DisplayAlert("Invoice", $"InvoiceId: {selected.Id}\n{selected.InvoiceNumber}", "OK");
    }
}
