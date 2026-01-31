using System.Text.Json;
using Victoria.Mobile.Models;
using Victoria.Mobile.Services;

namespace Victoria.Mobile.Pages;

[QueryProperty(nameof(InvoiceId), "id")]
public partial class InvoiceDetailsPage : ContentPage
{
    private readonly ApiClient _api;
    private int _invoiceId;

    public string InvoiceId
    {
        set
        {
            if (int.TryParse(value, out var id))
                _invoiceId = id;
        }
    }

    public InvoiceDetailsPage()
    {
        InitializeComponent();
        _api = new ApiClient();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadBasicAsync();
    }

    // Tu bierzemy minimalne dane z listy (przez API invoices/by-case i tak masz),
    // wiêc robimy prosty fetch przez /api/invoices/{id} jeœli masz.
    // Jeœli nie masz takiego endpointu, to na razie poka¿emy sam invoiceId.
    private async Task LoadBasicAsync()
    {
        try
        {
            ErrorLabel.Text = "";
            ResultLabel.Text = "";

            Loader.IsVisible = true;
            Loader.IsRunning = true;

            // Spróbuj pobraæ szczegó³y faktury (jeœli endpoint istnieje)
            // Jeœli go nie masz, z³apie exception i poka¿e samo ID.
            var json = await _api.GetStringAsync($"api/invoices/{_invoiceId}");
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // Minimalny model lokalny (nie musisz tworzyæ osobnego pliku)
            var dto = JsonSerializer.Deserialize<InvoiceDetailsLocal>(json, opts);

            if (dto != null)
            {
                InvoiceNumberLabel.Text = $"Invoice #: {dto.InvoiceNumber}";
                IssueDateLabel.Text = $"Issue: {dto.IssueDate:yyyy-MM-dd}";
                TotalLabel.Text = $"Total: {dto.TotalAmount:0.00} {dto.Currency}";
                StatusLabel.Text = $"Status: {dto.Status}";
            }
            else
            {
                InvoiceNumberLabel.Text = $"Invoice ID: {_invoiceId}";
                IssueDateLabel.Text = "";
                TotalLabel.Text = "";
                StatusLabel.Text = "";
            }
        }
        catch
        {
            // jeœli nie masz api/invoices/{id}, nie blokujemy — nadal da siê pobraæ DOCX
            InvoiceNumberLabel.Text = $"Invoice ID: {_invoiceId}";
            IssueDateLabel.Text = "";
            TotalLabel.Text = "";
            StatusLabel.Text = "";
        }
        finally
        {
            Loader.IsRunning = false;
            Loader.IsVisible = false;
        }
    }

    private async void OnDownloadDocxClicked(object sender, EventArgs e)
    {
        try
        {
            ErrorLabel.Text = "";
            ResultLabel.Text = "";

            Loader.IsVisible = true;
            Loader.IsRunning = true;

            // Twój backend:
            // GET api/printouts/invoices/{invoiceId}/word
            var bytes = await _api.GetBytesAsync($"api/printouts/invoices/{_invoiceId}/word");

            var fileName = $"invoice_{_invoiceId}_{DateTime.UtcNow:yyyyMMdd_HHmm}.docx";
            var fullPath = Path.Combine(FileSystem.CacheDirectory, fileName);

            await File.WriteAllBytesAsync(fullPath, bytes);

            ResultLabel.Text = $"Saved: {fileName}";

            // Otwórz systemowo plik (Word / viewer)
            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(fullPath)
            });
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

    private class InvoiceDetailsLocal
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = "";
        public DateTime IssueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "";
        public string Status { get; set; } = "";
    }
}
