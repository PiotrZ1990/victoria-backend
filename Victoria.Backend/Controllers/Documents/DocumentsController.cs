using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Victoria.Backend.DTOs.Documents;
using Victoria.Domain.Entities.Documents;
using Victoria.Domain.Entities.Files;
using Victoria.Infrastructure.Data;

namespace Victoria.Backend.Controllers.Documents;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
//[Authorize(Roles = "Staff,Admin,Student")]
public class DocumentsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public DocumentsController(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(25_000_000)]
    public async Task<IActionResult> Upload([FromForm] DocumentUploadRequest req)
    {
        if (req.File == null || req.File.Length == 0)
            return BadRequest("File is required");

        if (string.IsNullOrWhiteSpace(req.Title))
            return BadRequest("Title is required");

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

        var document = new Document
        {
            DocumentType = req.Title,
            Description = req.Description,
            FileResourceId = fileResource.Id,
            Status = "Uploaded",   // ← KLUCZOWA LINIA
            CreatedAt = DateTime.UtcNow
        };




        _db.Documents.Add(document);
        await _db.SaveChangesAsync();

        var result = new DocumentUploadResultDto
        {
            DocumentId = document.Id,
            FileResourceId = fileResource.Id,
            Title = document.DocumentType,
            FileName = fileResource.FileName,
            ContentType = fileResource.ContentType,
            SizeBytes = fileResource.FileSize,
            FilePath = fileResource.FilePath,
            CreatedAt = document.CreatedAt
        };

        return Ok(result);
    }
}
