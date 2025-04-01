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
        private readonly IDateTimeParser _dateTimeParser;
        public SaleController(ISaleService saleService, IProductService productService, AppDbContext context, IDateTimeParser dateTimeParser)
        {
            _saleService = saleService;
            _productService = productService;
            _context = context;
            _dateTimeParser = dateTimeParser;

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
                return BadRequest("Sale is null");
            }

            if (saleDto.DiscountValue <= 0 || saleDto.DiscountValue > 100)
            {
                return BadRequest("Discount value must be between 0 and 100 (0% to 100%)");
            }

            if (saleDto.StartsAt == null && saleDto.EndsAt == null && saleDto.IsActive == true)
            {
                return BadRequest("Sale cannot be active without dates");
            }

            // Parse dates - will be converted to UTC internally
            DateTime? parsedStartsAt = null;
            DateTime? parsedEndsAt = null;

            if (!string.IsNullOrEmpty(saleDto.StartsAt))
            {
                parsedStartsAt = _dateTimeParser.Parse(saleDto.StartsAt);
            }

            if (!string.IsNullOrEmpty(saleDto.EndsAt))
            {
                parsedEndsAt = _dateTimeParser.Parse(saleDto.EndsAt);
            }

            // Check date constraints
            if (parsedStartsAt.HasValue && parsedEndsAt.HasValue && parsedStartsAt >= parsedEndsAt)
            {
                return BadRequest("Sale start date must be before end date");
            }

            Sale sale = new Sale()
            {
                Name = saleDto.Name,
                DiscountValue = saleDto.DiscountValue,
                Description = saleDto.Description,
                StartsAt = parsedStartsAt,
                EndsAt = parsedEndsAt,
                IsActive = saleDto.IsActive,
                ProductsOnThisSale = new List<Product>()
            };

            // Add products to sale
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

            _saleService.AddSale(sale);

            // Activate if needed
            if (sale.IsActive && sale.StartsAt.HasValue && sale.EndsAt.HasValue)
            {
                _saleService.ActivateSaleWithDefaultDates(sale.Id);
            }
            else if (sale.IsActive)
            {
                return BadRequest("Sale cannot be added in active form without dates");
            }

            return Ok(sale);
        }


        //Updating Sale

        [HttpPut("{id}")]
        public IActionResult UpdateSale(int id, SaleDto sale)
        {
            if (sale == null)
            {
                return BadRequest("Sale is null");
            }

            var existingSale = _saleService.GetSaleById(id);

            if (existingSale == null)
            {
                return BadRequest("Sale with that id was not found");
            }

            if (!string.IsNullOrEmpty(sale.Name) && sale.Name != "string")
                existingSale.Name = sale.Name;
            if (sale.DiscountValue != 0)
                existingSale.DiscountValue = sale.DiscountValue;
            if (!string.IsNullOrEmpty(sale.Description) && sale.Description != "string")
                existingSale.Description = sale.Description;

            // Parse and update dates
            if (!string.IsNullOrEmpty(sale.StartsAt) && sale.StartsAt != "string")
            {
                var parsedStartsAt = _dateTimeParser.Parse(sale.StartsAt);
                if (parsedStartsAt.HasValue)
                    existingSale.StartsAt = parsedStartsAt.Value;
            }

            if (!string.IsNullOrEmpty(sale.EndsAt) && sale.EndsAt != "string")
            {
                var parsedEndsAt = _dateTimeParser.Parse(sale.EndsAt);
                if (parsedEndsAt.HasValue)
                    existingSale.EndsAt = parsedEndsAt.Value;
            }

            // Validate dates if both are present
            if (existingSale.StartsAt.HasValue && existingSale.EndsAt.HasValue &&
                existingSale.StartsAt.Value >= existingSale.EndsAt.Value)
            {
                return BadRequest("Sale start date must be before end date");
            }

            existingSale.IsActive = sale.IsActive;

            // Update products
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
            }

            _saleService.UpdateSale(existingSale);

            if (existingSale.IsActive)
            {
                _saleService.ActivateSaleWithDefaultDates(existingSale.Id);
            }

            return Ok($"Sale with id {id} has been updated successfully");
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
