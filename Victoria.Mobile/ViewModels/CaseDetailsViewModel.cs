using System.ComponentModel;
using System.Runtime.CompilerServices;
using Victoria.Mobile.Models;
using Victoria.Mobile.Services;

namespace Victoria.Mobile.ViewModels;

public class CaseDetailsViewModel : INotifyPropertyChanged
{
    private readonly CaseFilesService _service;

    public event PropertyChangedEventHandler? PropertyChanged;

    private CaseDetailsModel? _item;
    public CaseDetailsModel? Item
    {
        get => _item;
        private set { _item = value; OnPropertyChanged(); }
    }

    private string? _error;
    public string? Error
    {
        get => _error;
        private set { _error = value; OnPropertyChanged(); }
    }

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        private set { _isBusy = value; OnPropertyChanged(); }
    }

    public CaseDetailsViewModel(CaseFilesService service)
    {
        _service = service;
    }

    public async Task LoadAsync(int caseFileId)
    {
        try
        {
            IsBusy = true;
            Error = null;
            Item = await _service.GetByIdAsync(caseFileId);
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

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
