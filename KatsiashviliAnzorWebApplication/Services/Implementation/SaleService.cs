using KatsiashviliAnzorWebApplication.Data;
using KatsiashviliAnzorWebApplication.Models;
using KatsiashviliAnzorWebApplication.Services.Abstraction;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace KatsiashviliAnzorWebApplication.Services.Implementation
{
    public class SaleService : ISaleService
    {
        private readonly AppDbContext _context;
        private readonly IProductService _productService;
        public SaleService(AppDbContext context, IProductService productService)
        {
            _context = context;   
            _productService = productService;
        }

        public void AddSale(Sale sale)
        {
            _context.Sales.Add(sale);
            _context.SaveChanges();
        }

        public void DeleteSale(int id)
        {
            _context.Sales.Remove(GetSaleById(id));
            _context.SaveChanges();
        }

        public List<Sale> GetAllSales()
        {
            return _context.Sales.ToList();
        }

        public Sale GetSaleById(int id)
        {
            return _context.Sales.Include(s => s.ProductsOnThisSale).FirstOrDefault(s => s.Id == id);
        }

        public List<Sale> GetSalesByProductId(int productId)
        {
            return _context.Sales
           .Include(s => s.ProductsOnThisSale)
           .Where(s => s.ProductsOnThisSale.Any(p => p.Id == productId))
           .ToList();
        }

        public void UpdateSale(Sale sale)
        {
            _context.Sales.Update(sale);
        }
    }
}
