using Microsoft.EntityFrameworkCore;
using KttvKvtnWeb.Models;

namespace KttvKvtnWeb.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<CwtDomua> CwtDomua { get; set; } = null!;
        public DbSet<TramKttvtn> TramKttvtn { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
    }
}