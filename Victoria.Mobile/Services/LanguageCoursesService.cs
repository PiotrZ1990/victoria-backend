using System.Text.Json;
using Victoria.Mobile.Models;

namespace Victoria.Mobile.Services;

public class LanguageCoursesService
{
    private readonly ApiClient _api;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public LanguageCoursesService(ApiClient api)
    {
        _api = api;
    }

    public async Task<List<LanguageCourseListModel>> GetAllAsync()
    {
        var json = await _api.GetStringAsync("api/languagecourses");
        return JsonSerializer.Deserialize<List<LanguageCourseListModel>>(json, JsonOpts) ?? new();
    }

    public async Task<LanguageCourseDetailsModel> GetByIdAsync(int id)
    {
        var json = await _api.GetStringAsync($"api/languagecourses/{id}");
        var dto = JsonSerializer.Deserialize<LanguageCourseDetailsModel>(json, JsonOpts);
        if (dto == null) throw new Exception("Course not found");
        return dto;
    }
}
