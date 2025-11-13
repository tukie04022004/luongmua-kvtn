using KttvKvtnWeb.Data;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure; // ← CẦN THÊM DÒNG NÀY!

var builder = WebApplication.CreateBuilder(args);

// === CẤU HÌNH EPPLUS (XUẤT EXCEL) ===
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// === CẤU HÌNH DB VỚI RETRY + TIMEOUT + SSL ===
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection")),
        mysqlOptions =>
        {
            // BẬT RETRY KHI MẤT KẾT NỐI (Railway proxy hay chập chờn)
            mysqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null);

            // TĂNG TIMEOUT LÊN 60 GIÂY
            mysqlOptions.CommandTimeout(60);
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