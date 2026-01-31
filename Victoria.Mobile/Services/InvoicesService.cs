using System.Text.Json;
using Victoria.Mobile.Models;

namespace Victoria.Mobile.Services;

public class InvoicesService
{
    private readonly ApiClient _api;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public InvoicesService(ApiClient api) => _api = api;

    public async Task<List<InvoiceListItemModel>> GetByCaseAsync(int caseFileId)
    {
        var json = await _api.GetStringAsync($"api/invoices/by-case/{caseFileId}");
        return JsonSerializer.Deserialize<List<InvoiceListItemModel>>(json, JsonOpts) ?? new();
    }
}
