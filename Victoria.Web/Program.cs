var builder = WebApplication.CreateBuilder(args);

// ===============================
// MVC
// ===============================
builder.Services.AddControllersWithViews();

// ===============================
// HTTP CLIENT → BACKEND API
// ===============================
builder.Services.AddHttpClient("BackendApi", client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["Api:BaseUrl"]!);
});

// ===============================
// SESSION (JWT storage)
// ===============================
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ===============================
// BUILD APP
// ===============================
var app = builder.Build();

// ===============================
// PIPELINE
// ===============================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// 🔴 MUSI BYĆ PRZED Authorization
app.UseSession();

app.UseAuthorization();

// ===============================
// ROUTING
// ===============================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
