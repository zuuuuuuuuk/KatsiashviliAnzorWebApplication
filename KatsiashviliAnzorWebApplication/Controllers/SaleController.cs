using KatsiashviliAnzorWebApplication.Data;
using KatsiashviliAnzorWebApplication.Dto;
using KatsiashviliAnzorWebApplication.Models;
using KatsiashviliAnzorWebApplication.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KatsiashviliAnzorWebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaleController : ControllerBase
    {
        private readonly ISaleService _saleService;
        private readonly IProductService _productService;
        private readonly AppDbContext _context;
        public SaleController(ISaleService saleService, IProductService productService, AppDbContext context)
        {
            _saleService = saleService;
            _productService = productService;
            _context = context;

        }

        // Get All Sales

        [HttpGet]
        public IActionResult GetAllSales() 
        {
            var sales = _saleService.GetAllSales();
            return Ok(sales);
        }


        // Adding the Sale

        [HttpPost]
        public IActionResult AddSale(SaleDto saleDto)
        {
            if (saleDto == null)
            {
                return BadRequest("sale is null");
            }

            if (saleDto.DiscountValue <= 0 || saleDto.DiscountValue > 100)
            {
                return BadRequest("Discount value must be between 0 and 100 (0% to 100%)");
            }


            if (saleDto.StartsAt >= saleDto.EndsAt)
            {
                return BadRequest("Sale start date must be before end date");
            }

            

            Sale sale = new Sale()
            {
                Name = saleDto.Name,
                DiscountValue = saleDto.DiscountValue,
                Description = saleDto.Description,
                StartsAt = saleDto.StartsAt,
                EndsAt = saleDto.EndsAt,
                IsActive = saleDto.IsActive,
                ProductsOnThisSale = new List<Product>()
            };

            if (saleDto.ProductIdsOnThisSale != null)
            {
                foreach (var productId in saleDto.ProductIdsOnThisSale)
                {
                    var product = _productService.GetProductById(productId);
                    if (product != null)
                    {
                        sale.ProductsOnThisSale.Add(product);
                    }
                    
                }
           
            }

            if (sale.IsActive)
            {
                if (sale.StartsAt.HasValue && sale.EndsAt.HasValue)
                {
                    _saleService.ActivateSaleWithDefaultDates(sale.Id);
                }
                else
                {
                    return BadRequest("sale can not be added in active form without dates");
                }
            }


            _saleService.AddSale(sale);


            return Ok(sale);
                    
        }


        //Updating Sale

        [HttpPut("{id}")]
        public IActionResult UpdateSale(int id, SaleDto sale)
        {
            if (sale == null)
            {
                return BadRequest("sale is null");
            }

            var existingSale = _saleService.GetSaleById(id);

            if (existingSale == null)
            {
                return BadRequest("sale with that id was not found");
            }

            if (!string.IsNullOrEmpty(sale.Name) && sale.Name != "string")
                existingSale.Name = sale.Name;
            if(sale.DiscountValue != 0)
                existingSale.DiscountValue = sale.DiscountValue;
            if (!string.IsNullOrEmpty(sale.Description) && sale.Description != "string")
                existingSale.Description = sale.Description;
            if (sale.StartsAt.HasValue) 
                existingSale.StartsAt = sale.StartsAt.Value;
            if (sale.EndsAt.HasValue)
                existingSale.EndsAt = sale.EndsAt.Value;

            existingSale.IsActive = sale.IsActive;

            if (sale.ProductIdsOnThisSale != null)
            {
                var products = _productService.GetAllProducts()
              .Where(p => sale.ProductIdsOnThisSale.Contains(p.Id))
              .ToList();


                existingSale.ProductsOnThisSale.Clear();

                if (products != null)
                
                    foreach (var product in products)
                {
                        existingSale.ProductsOnThisSale.Add(product);
                        
                    }
                _saleService.ActivateSaleWithDefaultDates(existingSale.Id);
                _saleService.UpdateSale(existingSale);
                
            }

           
            return Ok($"sale with id {id} has been updated successfully"); 
        }


        // Adding products to sale

        [HttpPut("{id}/add-products")]
        public IActionResult AddProductsToSale(int id, List<int> productIds)
        {

            var sale = _saleService.GetSaleById(id);

            if (sale == null)
            {
                return NotFound($"sale with id {id} not found");
            }


            if (productIds == null || !productIds.Any())
            {
                return BadRequest("id list can not be empty");
            }

            var products = _productService.GetAllProducts()
               .Where(p => productIds.Contains(p.Id))
               .ToList();

            foreach (var product in products)
            {
                if (!sale.ProductsOnThisSale.Any(p => p.Id == product.Id))
                {
                    sale.ProductsOnThisSale.Add(product);
                    _saleService.UpdateSale(sale);
                    _saleService.UpdateProductDiscountedPrice(product);
                }
                
            }


            

            return Ok("products added successfully");
        }


        // Removing products from sale

        [HttpPut("{id}/remove-products")]
        public IActionResult DeleteProduct(int id, List<int> productIds)
        {

            var sale = _saleService.GetSaleById(id);
            
            if(sale == null)
            {
                return BadRequest("sale was not fount");
            }

            if (productIds == null || !productIds.Any())
            {
                return BadRequest("product id list cannot be empty");
            }

            var removedProducts = sale.ProductsOnThisSale
                .Where(p => productIds.Contains(p.Id))
                .ToList();

            _saleService.UpdateSale(sale);

            foreach (var product in removedProducts)
            {
                sale.ProductsOnThisSale.Remove(product);
                _saleService.UpdateSale(sale);
                _saleService.UpdateProductDiscountedPrice(product);
            }

           

            return Ok("products removed successfully");
        }



        // Deleting the Sale with ID and applying changes to products

        [HttpDelete("{id}")]
        public IActionResult DeleteSale(int id)
        {
            var sale = _saleService.GetSaleById(id);
            if (sale == null) 
            {
                return NotFound("sale with that ID was not found");
            }

            var affectedProducts = sale.ProductsOnThisSale.ToList();
            _saleService.DeleteSale(id);

            foreach (var product in affectedProducts)
            {
                {
                    var activeSales = _saleService.GetSalesByProductId(product.Id)
                                                  .Where(s => s.IsActive)
                                                  .ToList();
                    if (!activeSales.Any())
                    {
                        product.DiscountedPrice = product.OriginalPrice;
                    }
                    else
                    {
                        // Multiply the discounts of all active sales for this product
                        decimal discountMultiplier = activeSales
                            .Aggregate(1m, (total, s) => total * (1 - (s.DiscountValue / 100)));

                        product.DiscountedPrice = product.OriginalPrice * discountMultiplier;
                    }
                    _context.Products.Update(product);  // marks property of product modified
                }
            }
            return Ok($"sale with id {id} has been deleted");
        }

        // Getting Active Sales for Specific Product

        [HttpGet("product/{productId}")]
        public IActionResult GetActiveSalesForProduct(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
            {
                return NotFound("Product not found");
            }

            var activeSales = product.Sales?
                .Where(s => s.IsActive &&
                            s.StartsAt <= DateTime.UtcNow &&
                            s.EndsAt >= DateTime.UtcNow)
                .ToList();

            return Ok(activeSales);
        }

        // Activating Specific Sale with ID

        [HttpPost("{id}/{days}/activate")]
        public IActionResult ActivateSaleById(int id, int days)
        {

            try
            {
                _saleService.ActivateSale(id, days);
                return Ok($"sale with id {id} is active");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Deactivating specific Sale with ID

        [HttpPost("{id}/deactivate")]
        public  IActionResult DeactivateSaleById(int id)
        {
            try
            {
                _saleService.DeactivateSale(id);
                return Ok($"sale with id {id} has been deactivated");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Activating All the Sales

        [HttpPost("{days}/activate-all")]
        public IActionResult ActivateAllSales(int days)
        {
            var sales = _saleService.GetAllSales().ToList();
            try
            {
                foreach (var sale in sales)
                {
                    _saleService.ActivateSale(sale.Id, days);
                }
            }
            catch (Exception ex)
            {
                BadRequest(ex.Message);
            }
            return Ok("all the sales are active");
        }

        // Deactivating All the Sales

        [HttpPost("deactivate-all")]
        public IActionResult DeactivateAllSales()
        {
            var sales = _saleService.GetAllSales().ToList();
            try
            {
                foreach(var sale in sales)
                {
                    _saleService.DeactivateSale(sale.Id);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok("all the sales have been deactivated");
        }
    }
}
