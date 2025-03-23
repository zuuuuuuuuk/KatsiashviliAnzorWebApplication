using KatsiashviliAnzorWebApplication.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace KatsiashviliAnzorWebApplication.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Review> Reviews { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
      
        }

    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        
            modelBuilder.Entity<Cart>()
                   .HasOne(c => c.User) 
                   .WithOne() 
                   .HasForeignKey<Cart>(c => c.UserId) 
                   .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                   .HasOne(ci => ci.Cart)
                   .WithMany(c => c.CartItems)
                   .HasForeignKey(ci => ci.CartId);

            modelBuilder.Entity<CartItem>()
                   .HasOne(ci => ci.Product)
                   .WithMany()
                   .HasForeignKey(ci => ci.ProductId);

            modelBuilder.Entity<Order>()
                   .HasOne(o => o.User)
                   .WithMany(u => u.Orders)
                   .HasForeignKey(o => o.UserId);

            modelBuilder.Entity<Order>()
                   .HasMany(o => o.OrderItems)
                   .WithOne(oi => oi.Order)
                   .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<Product>()
                   .HasOne(p => p.Sale)
                   .WithMany(s => s.ProductsOnThisSale)
                   .HasForeignKey(p => p.SaleId);

            modelBuilder.Entity<Product>()
                   .HasOne(p => p.Category)
                   .WithMany(c => c.Products)
                   .HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<Product>()
                   .HasMany(p => p.Reviews)
                   .WithOne(r => r.Product)  
                   .HasForeignKey(r => r.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                   .HasOne(r => r.User) 
                   .WithMany()  
                   .HasForeignKey(r => r.UserId) 
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
