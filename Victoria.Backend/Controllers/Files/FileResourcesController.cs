using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Victoria.Backend.DTOs.Files;
using Victoria.Domain.Entities.Files;
using Victoria.Infrastructure.Data;

namespace Victoria.Backend.Controllers.Files;

[ApiController]
[Route("api/[controller]")]
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
    public async Task<IActionResult> UploadImage([FromForm] ImageUploadRequestDto req)
    {
        if (req.File == null || req.File.Length == 0)
            return BadRequest("File is required");

        var root = _config["FileStorage:UploadRoot"] ?? "uploads";
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), root);

        var subFolder = Path.Combine(DateTime.UtcNow.Year.ToString(), DateTime.UtcNow.Month.ToString("D2"));
        var targetFolder = Path.Combine(basePath, subFolder);

        if (!Directory.Exists(targetFolder))
            Directory.CreateDirectory(targetFolder);

        var safeOriginalName = Path.GetFileName(req.File.FileName);
        var ext = Path.GetExtension(safeOriginalName);
        var storedFileName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(targetFolder, storedFileName);

        await using (var stream = System.IO.File.Create(fullPath))
        {
            await req.File.CopyToAsync(stream);
        }

        var fileResource = new FileResource
        {
            FileName = safeOriginalName,
            ContentType = req.File.ContentType ?? "application/octet-stream",
            FileSize = req.File.Length,
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
