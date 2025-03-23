using KatsiashviliAnzorWebApplication.Data;
using KatsiashviliAnzorWebApplication.Models;
using KatsiashviliAnzorWebApplication.Services.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace KatsiashviliAnzorWebApplication.Services.Implementation
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public void AddProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void DeleteProduct(int id)
        {
            _context.Products.Remove(GetProductById(id));
            _context.SaveChanges();
        }

        public List<Product> GetAllProducts()
        {
            return _context.Products.Include(p => p.Images).ToList();
        }

        public Product GetProductById(int id)
        {
            return _context.Products.Include(p => p.Images)
                 .FirstOrDefault(p => p.Id == id);

        }
    }
}
