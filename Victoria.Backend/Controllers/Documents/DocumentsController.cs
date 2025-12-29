using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Victoria.Backend.DTOs.Documents;
using Victoria.Domain.Entities.Documents;
using Victoria.Domain.Entities.Files;
using Victoria.Domain.Enums;
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
            Status = Domain.Enums.DocumentStatus.Uploaded,   
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
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var doc = await _db.Documents
            .Include(x => x.FileResource)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (doc == null)
            return NotFound();

        return Ok(new
        {
            doc.Id,
            doc.DocumentType,
            doc.Description,
            doc.Status,
            doc.CreatedAt,
            File = new
            {
                doc.FileResourceId,
                doc.FileResource.FileName,
                doc.FileResource.ContentType,
                doc.FileResource.FileSize,
                doc.FileResource.FilePath
            }
        });
    }
    
    [HttpPost("{id:int}/approve")]
    //[Authorize(Roles = "Staff,Admin")]
    [AllowAnonymous]
    public async Task<IActionResult> Approve(int id)
    {
        var doc = await _db.Documents.FirstOrDefaultAsync(x => x.Id == id);
        if (doc == null)
            return NotFound("Document not found");

        doc.Status = Domain.Enums.DocumentStatus.Approved;
        await _db.SaveChangesAsync();

        return Ok(new { doc.Id, doc.Status });
    }
    
    [HttpPost("{id:int}/reject")]
    //[Authorize(Roles = "Staff,Admin")]
    [AllowAnonymous]
    public async Task<IActionResult> Reject(int id, [FromBody] DocumentRejectDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Reason))
            return BadRequest("Reason is required");

        var doc = await _db.Documents.FirstOrDefaultAsync(x => x.Id == id);
        if (doc == null)
            return NotFound("Document not found");

        doc.Status = Domain.Enums.DocumentStatus.Rejected;

        // zapisujemy powód w Description (masz już to pole)
        doc.Description = dto.Reason;

        await _db.SaveChangesAsync();

        return Ok(new { doc.Id, doc.Status, doc.Description });
    }

    [HttpPut("{documentId:int}/status")]
    public async Task<IActionResult> UpdateStatus(int documentId, [FromBody] DocumentStatusUpdateDto dto)
    {
        var doc = await _db.Documents
            .FirstOrDefaultAsync(x => x.Id == documentId);

        if (doc == null)
            return NotFound("Document not found");

        if (!Enum.TryParse<DocumentStatus>(dto.Status, true, out var newStatus))
            return BadRequest("Invalid document status");

        doc.Status = newStatus;

        // Jeżeli zatwierdzony => domykamy checklist item (Application albo Visa)
        // jeśli Approved => domykamy checklist
        // jeśli Rejected => cofamy checklist
        if (newStatus == DocumentStatus.Approved || newStatus == DocumentStatus.Rejected)
        {
            var markCompleted = newStatus == DocumentStatus.Approved;

            bool anyLinked = false;
            bool anyChecklistUpdated = false;

            // ==========================
            // APPLICATION PATH
            // ApplicationDocument -> StudyApplication -> CaseFileId
            // ==========================
            var appLink = await _db.ApplicationDocuments
                .Include(x => x.StudyApplication)
                .FirstOrDefaultAsync(x => x.DocumentId == documentId);

            if (appLink != null)
            {
                anyLinked = true;

                var caseFileId = appLink.StudyApplication.CaseFileId;

                var item = await _db.CaseApplicationChecklistItems
                    .Include(x => x.Checklist)
                    .FirstOrDefaultAsync(x =>
                        x.CaseFileId == caseFileId &&
                        x.Checklist.DocumentType == doc.DocumentType);

                if (item != null)
                {
                    item.IsCompleted = markCompleted;
                    anyChecklistUpdated = true;
                }
            }

            // ==========================
            // VISA PATH
            // VisaDocument -> VisaApplication -> CaseFileId
            // ==========================
            var visaLink = await _db.VisaDocuments
                .Include(x => x.VisaApplication)
                .FirstOrDefaultAsync(x => x.DocumentId == documentId);

            if (visaLink != null)
            {
                anyLinked = true;

                var caseFileId = visaLink.VisaApplication.CaseFileId;

                var item = await _db.CaseVisaChecklistItems
                    .Include(x => x.Checklist)
                    .FirstOrDefaultAsync(x =>
                        x.CaseFileId == caseFileId &&
                        x.Checklist.DocumentType == doc.DocumentType);

                if (item != null)
                {
                    item.IsCompleted = markCompleted;
                    anyChecklistUpdated = true;
                }
            }

            // Czytelne info jak coś nie gra
            if (!anyLinked)
                return BadRequest("Document is not linked to StudyApplication or VisaApplication.");

            if (!anyChecklistUpdated)
                return BadRequest("No matching checklist item found for this document type. Check DocumentType vs Checklist DocumentType.");
        }


        await _db.SaveChangesAsync();

        return Ok(new
        {
            doc.Id,
            doc.DocumentType,
            Status = doc.Status.ToString()
        });
    }
    [HttpPost("attach-to-application")]
    public async Task<IActionResult> AttachToApplication([FromBody] AttachDocumentDto dto)
    {
        var doc = await _db.Documents.FirstOrDefaultAsync(x => x.Id == dto.DocumentId);
        if (doc == null) return NotFound("Document not found");

        var app = await _db.StudyApplications.FirstOrDefaultAsync(x => x.Id == dto.StudyApplicationId);
        if (app == null) return NotFound("StudyApplication not found");

        var exists = await _db.ApplicationDocuments
            .AnyAsync(x => x.DocumentId == dto.DocumentId && x.StudyApplicationId == dto.StudyApplicationId);

        if (!exists)
        {
            _db.ApplicationDocuments.Add(new ApplicationDocument
            {
                DocumentId = dto.DocumentId,
                StudyApplicationId = dto.StudyApplicationId
            });

            await _db.SaveChangesAsync();
        }

        return Ok(new { message = "Attached", dto.DocumentId, dto.StudyApplicationId });
    }


}
