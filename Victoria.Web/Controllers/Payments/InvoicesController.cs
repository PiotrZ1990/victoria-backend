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
    // LIST
    // GET: /Invoices
    // =========================================
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var client = CreateApiClientWithJwt();

            var response = await client.GetAsync("api/invoices");
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to load invoices. Status={(int)response.StatusCode}. Body={body}");
            }

            var json = await response.Content.ReadAsStringAsync();

            var invoices = JsonSerializer.Deserialize<List<InvoiceListViewModel>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(invoices ?? new());
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // =========================================
    // CREATE (GET)
    // GET: /Invoices/Create?caseFileId=1
    // =========================================
    [HttpGet]
    public IActionResult Create(int caseFileId)
    {
        try
        {
            _ = CreateApiClientWithJwt();

            var vm = new InvoiceCreateViewModel
            {
                CaseFileId = caseFileId,
                Currency = "GBP",
                IssueDate = DateTime.UtcNow.Date
            };

            return View(vm);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // =========================================
    // CREATE (POST)
    // POST: /Invoices/Create
    // =========================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(InvoiceCreateViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        try
        {
            var client = CreateApiClientWithJwt();

            var payload = new
            {
                caseFileId = vm.CaseFileId,
                invoiceNumber = vm.InvoiceNumber,
                issueDate = vm.IssueDate,
                totalAmount = vm.TotalAmount,
                currency = vm.Currency
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync("api/invoices", content);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Create invoice failed. Status={(int)response.StatusCode}. Body={body}";
                return View(vm);
            }

            TempData["Success"] = "Invoice created";
            return RedirectToAction(nameof(Index));
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // =========================================
    // EDIT (GET)
    // GET: /Invoices/Edit/5
    // =========================================
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var client = CreateApiClientWithJwt();

            var response = await client.GetAsync($"api/invoices/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Load invoice failed. Status={(int)response.StatusCode}. Body={body}";
                return RedirectToAction(nameof(Index));
            }

            var json = await response.Content.ReadAsStringAsync();
            var invoice = JsonSerializer.Deserialize<InvoiceDetailsViewModel>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (invoice == null)
                return NotFound();

            var vm = new InvoiceEditViewModel
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                IssueDate = invoice.IssueDate.Date,
                TotalAmount = invoice.TotalAmount,
                Currency = invoice.Currency,

                // ważne: status ma być string typu "Issued"/"Paid"/"PartiallyPaid"/"Overdue"
                Status = (invoice.Status ?? "Issued").Trim()
            };

            return View(vm);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // =========================================
    // EDIT (POST)
    // POST: /Invoices/Edit/5
    // =========================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, InvoiceEditViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        try
        {
            var client = CreateApiClientWithJwt();

            var payload = new
            {
                invoiceNumber = vm.InvoiceNumber,
                issueDate = vm.IssueDate,
                totalAmount = vm.TotalAmount,
                currency = vm.Currency,
                status = (vm.Status ?? "Issued").Trim()
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            var response = await client.PutAsync($"api/invoices/{id}", content);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Update invoice failed. Status={(int)response.StatusCode}. Body={body}";
                return View(vm);
            }

            TempData["Success"] = "Invoice updated";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // =========================================
    // DELETE (POST)
    // POST: /Invoices/Delete/5
    // =========================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var client = CreateApiClientWithJwt();

            var response = await client.DeleteAsync($"api/invoices/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Delete failed. Status={(int)response.StatusCode}. Body={body}";
            }
            else
            {
                TempData["Success"] = "Invoice deleted";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
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

            // 1) invoice
            var invoiceResponse = await client.GetAsync($"api/invoices/{id}");
            if (!invoiceResponse.IsSuccessStatusCode)
            {
                var body = await invoiceResponse.Content.ReadAsStringAsync();
                TempData["Error"] = $"Load invoice failed. Status={(int)invoiceResponse.StatusCode}. Body={body}";
                return RedirectToAction(nameof(Index));
            }

            var invoiceJson = await invoiceResponse.Content.ReadAsStringAsync();
            var invoice = JsonSerializer.Deserialize<InvoiceDetailsViewModel>(
                invoiceJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (invoice == null)
                return NotFound();

            // 2) payments
            var paymentsResponse = await client.GetAsync($"api/payments/invoice/{id}");
            if (paymentsResponse.IsSuccessStatusCode)
            {
                var paymentsJson = await paymentsResponse.Content.ReadAsStringAsync();

                var payments = JsonSerializer.Deserialize<List<PaymentViewModel>>(
                    paymentsJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                invoice.Payments = payments ?? new();
            }
            else
            {
                invoice.Payments = new();
            }

            // 3) calc
            invoice.PaidAmount = invoice.Payments.Sum(x => x.Amount);
            invoice.RemainingAmount = invoice.TotalAmount - invoice.PaidAmount;

            // 4) add payment form
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
                var body = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Failed to add payment. Status={(int)response.StatusCode}. Body={body}";
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
