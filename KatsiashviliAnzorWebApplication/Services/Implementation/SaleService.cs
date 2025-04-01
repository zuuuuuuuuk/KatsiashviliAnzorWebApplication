using KatsiashviliAnzorWebApplication.Data;
using KatsiashviliAnzorWebApplication.Dto;
using KatsiashviliAnzorWebApplication.Models;
using KatsiashviliAnzorWebApplication.Services.Abstraction;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.Xml;

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



        // Activation of specific sale
        public void ActivateSale(int saleId, int days)
        {
            var sale = _context.Sales.Include(s => s.ProductsOnThisSale).FirstOrDefault(s => s.Id == saleId);

            if (sale == null && !sale.IsActive)
            {
                throw new Exception("Sale not found or is already active");
            }

            if (!sale.StartsAt.HasValue && !sale.EndsAt.HasValue) // checking if sale got no dates yet
            {
                sale.StartsAt = DateTime.UtcNow;
                sale.EndsAt = DateTime.UtcNow.AddDays(days);
            }
            else // for checking...
            {
                sale.StartsAt = DateTime.UtcNow;

                if (sale.EndsAt <= DateTime.UtcNow)
                {
                    sale.EndsAt = DateTime.UtcNow.AddDays(days);
                }
            }


            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (sale.ProductsOnThisSale != null)
                    {
                        foreach (var product in sale.ProductsOnThisSale)
                        {
                            decimal discountedPrice = product.OriginalPrice - (product.OriginalPrice * (sale.DiscountValue / 100));
                            product.DiscountedPrice = discountedPrice;

                            _context.Entry(product).Property(p => p.DiscountedPrice).IsModified = true;
                        }
                    }
                    sale.IsActive = true;
                    _context.Sales.Update(sale);
                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("error during sale activation", ex);
                }
            }


        }


        // Deactivation of specific sale 

        public void DeactivateSale(int saleId)
        {
            var sale = _context.Sales.Include(s => s.ProductsOnThisSale).FirstOrDefault(s => s.Id == saleId);

            if (sale == null)
            {
                throw new Exception("Sale not found");
            }
            var prods = new List<Product>();
            
            using (var transaction = _context.Database.BeginTransaction()) // ert operaciad rom moxdes da naxevrad ar sheicvalos baza
            {
                try
                {
                    foreach (var product in sale.ProductsOnThisSale)
                    {
                        // checking if there are other active sales for this product
                        var otherActiveSales = _context.Sales
                            .Where(s => s.IsActive && s.Id != saleId && s.ProductsOnThisSale.Contains(product))
                            .ToList();

                        if (!otherActiveSales.Any())
                        {
                            // if there are no other active sales, reset to originalPrice
                            product.DiscountedPrice = product.OriginalPrice;
                        }
                        else
                        {
                            // applying remaining active sales
                            decimal discountMultiplier = otherActiveSales
                                .Aggregate(1m, (total, s) => total * (1 - (s.DiscountValue / 100)));

                            product.DiscountedPrice = product.OriginalPrice * discountMultiplier;
                            
                        }
                        prods.Add(product);
                        _context.Products.Update(product);
                        
                    }
                    sale.ProductsOnThisSale = prods;
                    sale.IsActive = false;
                    _context.Update(sale);
                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("error during sale deactivation", ex);
                }
            } 

           
        }

        // Getting All Sales

        public List<Sale> GetAllSales()
        {
            return _context.Sales.Include(s => s.ProductsOnThisSale)
                .ThenInclude(p => p.Images)  // This prevents potential performance issues gets rid of duplicating data 
                    .AsSplitQuery()
                    .ToList();
        }

        // Get sale by id

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
        
        // Update sale

        public void UpdateSale(Sale sale)
        {
            _context.Sales.Update(sale);
            _context.SaveChanges();
        }

        // Sale activation for background process of checking sale dates automatically

        public void ActivateSaleWithDefaultDates(int saleId) // for background service use
        {
            var sale = _context.Sales.Include(s => s.ProductsOnThisSale).FirstOrDefault(s => s.Id == saleId);

            if (sale == null)
            {
                throw new Exception("Sale not found");
            }

            sale.IsActive = true;
            using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        if (sale.ProductsOnThisSale != null)
                        {
                      

                        foreach (var product in sale.ProductsOnThisSale)
                            {
                            UpdateProductDiscountedPrice(product);
                            }
                        }
                       
                        UpdateSale(sale);
                        _context.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("error during sale activation", ex);
                    }
                }
            

        }

        public void UpdateProductDiscountedPrice(Product product)
        {
            var activeSales = GetSalesByProductId(product.Id)
                                          .Where(s => s.IsActive)
                                          .ToList();

            var prod = product;
            if (prod != null) 
            if (!activeSales.Any())
            {
                prod.DiscountedPrice = prod.OriginalPrice;
            }
            else
            {
                decimal discountMultiplier = activeSales
                    .Aggregate(1m, (total, s) => total * (1 - (s.DiscountValue / 100)));

                prod.DiscountedPrice = prod.OriginalPrice * discountMultiplier;
            }

            _context.Products.Update(prod);
            _context.SaveChanges();
        }
    }
}
