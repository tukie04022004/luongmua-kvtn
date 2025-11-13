using KttvKvtnWeb.Data;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;  // ← THÊM DÒNG NÀY!

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(5, 5, 54))));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Đã có using nên OK

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();