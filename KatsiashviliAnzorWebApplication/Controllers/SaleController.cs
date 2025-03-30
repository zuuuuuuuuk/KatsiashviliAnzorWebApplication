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

            if (saleDto.DiscountValue < 0 || saleDto.DiscountValue > 1)
            {
                return BadRequest("Discount value must be between 0 and 1 (0% to 100%)");
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

            foreach(var productId in saleDto.ProductIdsOnThisSale)
            {
                var product = _productService.GetProductById(productId);
                if (product == null)
                {
                    return BadRequest("product with that productId was not found");
                }
                sale.ProductsOnThisSale.Add(product);
            }
            

            _saleService.AddSale(sale);
            return Ok(sale);
                    
        }

        // Deleting the Sale with ID and applying changes to products

        [HttpDelete("{id}")]
        public IActionResult DeleteSale(int id)
        {
            var sale = _saleService.GetSaleById(id);
            if (sale == null) 
            {
                return NotFound("sale not found");
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
                    _context.Entry(product).Property(p => p.DiscountedPrice).IsModified = true; // marks property of product modified
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

        [HttpPost("{id}/{days}")]
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
