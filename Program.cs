using KttvKvtnWeb.Data;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);

// === CẤU HÌNH EPPLUS (XUẤT EXCEL) ===
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// === CẤU HÌNH DB: FIX TIMEOUT + RETRY + LOẠI BỎ ConnectionStringBuilder ===
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 36)), // DÙNG PHIÊN BẢN CỐ ĐỊNH
        mysqlOptions =>
        {
            // BẬT RETRY KHI MẤT KẾT NỐI
            mysqlOptions.EnableRetryOnFailure(
                maxRetryCount: 10,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);

            // TĂNG COMMAND TIMEOUT LÊN 5 PHÚT
            mysqlOptions.CommandTimeout(300);
        }));

// === CÁC SERVICE KHÁC ===
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

var app = builder.Build();

// === MIDDLEWARE ===
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