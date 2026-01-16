using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Victoria.Infrastructure.Data;

namespace Victoria.Backend.Controllers.Printouts;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous] // docelowo Staff/Admin
public class PrintoutsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _cfg;

    public PrintoutsController(AppDbContext db, IConfiguration cfg)
    {
        _db = db;
        _cfg = cfg;
    }

    // GET: api/printouts/invoices/{invoiceId}/word
    [HttpGet("invoices/{invoiceId:int}/word")]
    public async Task<IActionResult> InvoiceWord(int invoiceId)
    {
        // 1) Dane faktury
        var invoice = await _db.Invoices.FirstOrDefaultAsync(x => x.Id == invoiceId);
        if (invoice == null) return NotFound("Invoice not found");

        // 2) Suma wpłat (masz już InvoicePayments)
        var totalPaid = await _db.InvoicePayments
            .Where(x => x.InvoiceId == invoiceId)
            .Include(x => x.Payment)
            .SumAsync(x => (decimal?)x.Payment.Amount) ?? 0m;

        var remaining = Math.Max(0m, invoice.TotalAmount - totalPaid);

        // 3) Ścieżka do szablonu z Settings (Twoje wymagania)
        //    Jeśli masz serwis Settings – użyj go. Poniżej najprościej bezpośrednio:
        var templateRel = await _db.Settings
            .Where(s => s.Key == "InvoiceTemplatePath")
            .Select(s => s.Value)
            .FirstOrDefaultAsync();

        if (string.IsNullOrWhiteSpace(templateRel))
            return BadRequest("Missing Settings key: InvoiceTemplatePath");

        var templateFullPath = Path.Combine(Directory.GetCurrentDirectory(), templateRel.Replace("/", Path.DirectorySeparatorChar.ToString()));
        if (!System.IO.File.Exists(templateFullPath))
            return BadRequest($"Template file not found: {templateRel}");

        // 4) Kopia szablonu do pamięci
        byte[] fileBytes;
        await using (var fs = System.IO.File.OpenRead(templateFullPath))
        {
            using var ms = new MemoryStream();
            await fs.CopyToAsync(ms);
            fileBytes = ms.ToArray();
        }

        // 5) Podmień placeholdery w DOCX (OpenXML)
        using (var ms = new MemoryStream())
        {
            ms.Write(fileBytes, 0, fileBytes.Length);
            ms.Position = 0;

            using (var doc = WordprocessingDocument.Open(ms, true))
            {
                ReplaceAllText(doc, "{{InvoiceNumber}}", invoice.InvoiceNumber);
                ReplaceAllText(doc, "{{IssueDate}}", invoice.IssueDate.ToString("yyyy-MM-dd"));
                ReplaceAllText(doc, "{{Currency}}", invoice.Currency);
                ReplaceAllText(doc, "{{Status}}", invoice.Status.ToString());

                ReplaceAllText(doc, "{{TotalAmount}}", invoice.TotalAmount.ToString("0.00"));
                ReplaceAllText(doc, "{{PaidAmount}}", totalPaid.ToString("0.00"));
                ReplaceAllText(doc, "{{RemainingAmount}}", remaining.ToString("0.00"));

                doc.MainDocumentPart!.Document.Save();
            }

            var outBytes = ms.ToArray();
            var outName = $"invoice_{invoice.InvoiceNumber}_{DateTime.UtcNow:yyyyMMdd_HHmm}.docx";

            return File(
                outBytes,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                outName
            );
        }
    }

    // --- helper: replace placeholders even if split across runs ---
    private static void ReplaceAllText(WordprocessingDocument doc, string find, string replace)
    {
        var body = doc.MainDocumentPart!.Document.Body;
        if (body == null) return;

        // 1) zbierz wszystkie Text w jednym stringu per paragraph
        foreach (var para in body.Descendants<Paragraph>())
        {
            var texts = para.Descendants<Text>().ToList();
            if (texts.Count == 0) continue;

            var full = new StringBuilder();
            foreach (var t in texts) full.Append(t.Text);

            var merged = full.ToString();
            if (!merged.Contains(find)) continue;

            merged = merged.Replace(find, replace);

            // 2) wyczyść i wpisz w pierwszy Text
            texts[0].Text = merged;
            for (int i = 1; i < texts.Count; i++)
                texts[i].Text = string.Empty;
        }
    }
}
