using System.Text.Json;
using Victoria.Mobile.Models;

namespace Victoria.Mobile.Services;

public class CaseFilesService
{
    private readonly ApiClient _api;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public CaseFilesService(ApiClient api)
    {
        _api = api;
    }

    public async Task<List<CaseFileDetailsDto>> GetAllAsync()
    {
        var json = await _api.GetStringAsync("api/casefiles");
        return JsonSerializer.Deserialize<List<CaseFileDetailsDto>>(json, JsonOpts) ?? new();
    }

    public async Task<CaseFileDetailsDto> GetByIdAsync(int id)
    {
        // Backend ma CaseFilesController -> GET api/casefiles/{id}
        return await _api.GetJsonAsync<CaseFileDetailsDto>($"api/casefiles/{id}");
    }
}
