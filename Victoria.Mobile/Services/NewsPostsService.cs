using System.Text.Json;
using Victoria.Mobile.Models;

namespace Victoria.Mobile.Services;

public class NewsPostsService
{
    private readonly ApiClient _api;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public NewsPostsService(ApiClient api)
    {
        _api = api;
    }

    public async Task<List<NewsPostListModel>> GetAllAsync()
    {
        var json = await _api.GetStringAsync("api/newsposts");
        return JsonSerializer.Deserialize<List<NewsPostListModel>>(json, JsonOpts) ?? new();
    }

    public async Task<NewsPostDetailsModel> GetByIdAsync(int id)
    {
        var json = await _api.GetStringAsync($"api/newsposts/{id}");
        var dto = JsonSerializer.Deserialize<NewsPostDetailsModel>(json, JsonOpts);
        if (dto == null) throw new Exception("News post not found");
        return dto;
    }
}
