using KatsiashviliAnzorWebApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace KatsiashviliAnzorWebApplication.Data
{
    public class AppDbContext : DbContext
    {
      public DbSet<Product> Products { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
          
        }
    }
}
