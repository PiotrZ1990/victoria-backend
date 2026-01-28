using System.Collections.ObjectModel;
using System.Windows.Input;
using Victoria.Mobile.Models;
using Victoria.Mobile.Services;

namespace Victoria.Mobile.ViewModels;

public class MyCasesViewModel
{
    private readonly CaseFilesService _service;

    public ObservableCollection<CaseDetailsModel> Items { get; } = new();

    public bool IsBusy { get; private set; }
    public string? Error { get; private set; }

    public ICommand RefreshCommand { get; }

    public MyCasesViewModel(CaseFilesService service)
    {
        _service = service;
        RefreshCommand = new Command(async () => await LoadAsync());
    }

    public async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        Error = null;

        try
        {
            Items.Clear();
            var list = await _service.GetAllAsync();
            foreach (var x in list.OrderByDescending(x => x.CreatedAt))
                Items.Add(x);
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
