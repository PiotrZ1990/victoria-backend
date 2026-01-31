using System.Text.Json;
using Victoria.Mobile.Models;

namespace Victoria.Mobile.Services;

public class CourseGroupsService
{
    private readonly ApiClient _api;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public CourseGroupsService(ApiClient api)
    {
        _api = api;
    }

    public async Task<List<CourseGroupListModel>> GetByCourseAsync(int courseId)
    {
        var json = await _api.GetStringAsync($"api/coursegroups/by-course/{courseId}");
        return JsonSerializer.Deserialize<List<CourseGroupListModel>>(json, JsonOpts) ?? new();
    }

    public async Task<CourseGroupDetailsModel> GetByIdAsync(int id)
    {
        var json = await _api.GetStringAsync($"api/coursegroups/{id}");
        var dto = JsonSerializer.Deserialize<CourseGroupDetailsModel>(json, JsonOpts);
        if (dto == null) throw new Exception("Course group not found");
        return dto;
    }
}
