using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Victoria.Domain.Entities.Files;
using Victoria.Infrastructure.Data;

namespace Victoria.Backend.Controllers.Files;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous] // docelowo Staff/Admin
public class FileResourcesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public FileResourcesController(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    // POST: api/fileresources/upload-image
    [HttpPost("upload-image")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(25_000_000)]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is required");

        var root = _config["FileStorage:UploadRoot"] ?? "uploads";
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), root);

        var subFolder = Path.Combine(DateTime.UtcNow.Year.ToString(), DateTime.UtcNow.Month.ToString("D2"));
        var targetFolder = Path.Combine(basePath, subFolder);

        if (!Directory.Exists(targetFolder))
            Directory.CreateDirectory(targetFolder);

        var safeOriginalName = Path.GetFileName(file.FileName);
        var ext = Path.GetExtension(safeOriginalName);
        var storedFileName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(targetFolder, storedFileName);

        await using (var stream = System.IO.File.Create(fullPath))
        {
            await file.CopyToAsync(stream);
        }

        var fileResource = new FileResource
        {
            FileName = safeOriginalName,
            ContentType = file.ContentType ?? "application/octet-stream",
            FileSize = file.Length,
            FilePath = Path.Combine(root, subFolder, storedFileName).Replace("\\", "/"),
            UploadedAt = DateTime.UtcNow
        };

        _db.FileResources.Add(fileResource);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            fileResourceId = fileResource.Id,
            fileName = fileResource.FileName,
            contentType = fileResource.ContentType,
            sizeBytes = fileResource.FileSize,
            filePath = fileResource.FilePath
        });
    }
}
