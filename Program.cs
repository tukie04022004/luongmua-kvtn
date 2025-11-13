using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Lấy connection string từ Environment Variable trước, fallback về appsettings.json
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION") 
                       ?? builder.Configuration.GetConnectionString("DefaultConnection");

// Kiểm tra connection string
if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("Connection string not found. Set DB_CONNECTION environment variable.");
}

// Thêm DbContext (EF Core) nếu dùng MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// Thêm dịch vụ MVC / Razor Pages / API
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage(); // Hiển thị lỗi chi tiết khi Development
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Map routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();

// ===== DbContext mẫu =====
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Ví dụ table Products
    public DbSet<Product> Products { get; set; } = null!;
}

// ===== Entity mẫu =====
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
