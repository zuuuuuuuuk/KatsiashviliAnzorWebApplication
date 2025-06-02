using KatsiashviliAnzorWebApplication.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection.Emit;
using System.Text.Json;

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
        public DbSet<PromoCode> PromoCodes { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Advertisement> Advertisements { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
      
        }

    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Defines the ValueComparer for List<int> 
            var intListComparer = new ValueComparer<List<int>>(
                (c1, c2) => c1.SequenceEqual(c2),  // Compare lists by element equality 
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v)),  // Generate a hash code for the list
                c => c.ToList()  // Clone the list to avoid reference issues
                );

                    modelBuilder.Entity<User>()
        .Property(u => u.FavoriteProductIds)
        .HasConversion(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
            v => JsonSerializer.Deserialize<List<int>>(v, (JsonSerializerOptions)null) ?? new List<int>()
        )
        .Metadata.SetValueComparer(intListComparer);  // Apply the ValueComparer intis listebistvis

            base.OnModelCreating(modelBuilder); 
        

        modelBuilder.Entity<Cart>()
                   .HasOne(c => c.User) 
                   .WithOne() 
                   .HasForeignKey<Cart>(c => c.UserId) 
                   .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                   .HasOne(ci => ci.Cart)
                   .WithMany(c => c.CartItems)
                   .HasForeignKey(ci => ci.CartId);

            modelBuilder.Entity<Category>()
                   .HasOne(c => c.ParentCategory)
                   .WithMany(c => c.SubCategories)
                   .HasForeignKey(c => c.ParentId)
                   .OnDelete(DeleteBehavior.Restrict);   // new


            modelBuilder.Entity<Order>()
                   .HasOne(o => o.User)
                   .WithMany(u => u.Orders)
                   .HasForeignKey(o => o.UserId);

            modelBuilder.Entity<Order>()
                   .HasMany(o => o.OrderItems)
                   .WithOne(oi => oi.Order)
                   .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<Product>()
                   .HasMany(p => p.Sales)
                   .WithMany(s => s.ProductsOnThisSale)
                   .UsingEntity(j => j.ToTable("ProductSales")); // junction table <3

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



            // decimal precision configurations 


            modelBuilder.Entity<PromoCode>()
                .Property(p => p.DiscountValue)
                .HasPrecision(18, 4);
            base.OnModelCreating(modelBuilder);



            modelBuilder.Entity<Product>()
                .Property(p => p.DiscountedPrice)
                .HasPrecision(18, 4);    

            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.DiscountedPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.OriginalPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Sale>()
                .Property(s => s.DiscountValue)
                .HasColumnType("decimal(18,2)");

            // Additional precision for TotalAmount in Order
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");
        }
    }
}
