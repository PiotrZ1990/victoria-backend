using System.Text.Json;
using Victoria.Mobile.Models;

namespace Victoria.Mobile.Services;

public class DocumentsService
{
    private readonly ApiClient _api;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public DocumentsService(ApiClient api)
    {
        _api = api;
    }

    public async Task<List<CaseDocumentModel>> GetForCaseAsync(int caseFileId)
    {
        var json = await _api.GetStringAsync($"api/documents/case/{caseFileId}");
        return JsonSerializer.Deserialize<List<CaseDocumentModel>>(json, JsonOpts) ?? new();
    }
}
