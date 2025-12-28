using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Victoria.Web.Models.Payments;

namespace Victoria.Web.Controllers.Payments;

public class InvoicesController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public InvoicesController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient CreateApiClientWithJwt()
    {
        var token = HttpContext.Session.GetString("JWT");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException();

        var client = _httpClientFactory.CreateClient("BackendApi");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    // =========================================
    // DETAILS: invoice + payments
    // GET: /Invoices/Details/{id}
    // =========================================
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var client = CreateApiClientWithJwt();

            // 1) Pobierz fakturę
            var invoiceResponse = await client.GetAsync($"api/invoices/{id}");
            if (!invoiceResponse.IsSuccessStatusCode)
                return NotFound();

            var invoiceJson = await invoiceResponse.Content.ReadAsStringAsync();
            var invoice = JsonSerializer.Deserialize<InvoiceDetailsViewModel>(
                invoiceJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (invoice == null)
                return NotFound();

            // 2) Pobierz płatności do faktury
            var paymentsResponse = await client.GetAsync($"api/payments/invoice/{id}");
            if (paymentsResponse.IsSuccessStatusCode)
            {
                var paymentsJson = await paymentsResponse.Content.ReadAsStringAsync();

                var payments = JsonSerializer.Deserialize<List<PaymentViewModel>>(
                    paymentsJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                invoice.Payments = payments ?? new();
            }

            // 3) Wyliczenia
            invoice.PaidAmount = invoice.Payments.Sum(x => x.Amount);
            invoice.RemainingAmount = invoice.TotalAmount - invoice.PaidAmount;

            // 4) Formularz dodania płatności
            ViewBag.PaymentCreate = new PaymentCreateViewModel
            {
                InvoiceId = invoice.Id,
                CaseFileId = invoice.CaseFileId,
                Currency = invoice.Currency
            };

            return View(invoice);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // =========================================
    // ADD PAYMENT
    // POST: /Invoices/AddPayment
    // =========================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddPayment(PaymentCreateViewModel vm)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(Details), new { id = vm.InvoiceId });

        try
        {
            var client = CreateApiClientWithJwt();

            var payload = new
            {
                invoiceId = vm.InvoiceId,
                caseFileId = vm.CaseFileId,
                amount = vm.Amount,
                currency = vm.Currency,
                paymentMethod = vm.PaymentMethod,
                serviceType = vm.ServiceType
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync("api/payments", content);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to add payment.";
                return RedirectToAction(nameof(Details), new { id = vm.InvoiceId });
            }

            TempData["Success"] = "Payment added.";
            return RedirectToAction(nameof(Details), new { id = vm.InvoiceId });
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }
}
