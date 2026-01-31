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

    public async Task UploadForCaseAsync(int caseFileId, string title, string? description, string filePath)
    {
        // multipart/form-data
        using var content = new MultipartFormDataContent();

        content.Add(new StringContent(caseFileId.ToString()), "CaseFileId");
        content.Add(new StringContent(title), "Title");
        if (!string.IsNullOrWhiteSpace(description))
            content.Add(new StringContent(description), "Description");

        var bytes = await File.ReadAllBytesAsync(filePath);
        var fileContent = new ByteArrayContent(bytes);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

        content.Add(fileContent, "File", Path.GetFileName(filePath));

        await _api.PostMultipartAsync("api/documents/upload-by-case", content);
    }

}
