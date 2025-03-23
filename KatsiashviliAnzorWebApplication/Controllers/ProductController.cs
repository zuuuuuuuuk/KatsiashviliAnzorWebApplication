using Microsoft.AspNetCore.Mvc;
using KatsiashviliAnzorWebApplication.Models;
using KatsiashviliAnzorWebApplication.Enum;
using KatsiashviliAnzorWebApplication.Dto;
using KatsiashviliAnzorWebApplication.Services;
using KatsiashviliAnzorWebApplication.Services.Abstraction;
using Microsoft.EntityFrameworkCore;
using KatsiashviliAnzorWebApplication.Data;

namespace KatsiashviliAnzorWebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly AppDbContext _context;
        public ProductController(IProductService productService, AppDbContext context)
        {
            _productService = productService;
            _context = context;
        }



        [HttpGet]
        public IActionResult GetAllProducts()
        {
            var products = _productService.GetAllProducts();
            if (products == null)
            {
                return BadRequest("products are null");
            }
            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            var prod = _productService.GetProductById(id);
            return Ok(prod);
        }

        [HttpPost]
        public IActionResult AddProduct(ProductDto product) 
        {
           

            if (product == null)
            {
                return BadRequest("product is null");
            }

            // Check if the category exists
            var categoryExists = _context.Categories.Any(c => c.Id == product.CategoryId);
            if (!categoryExists)
            {
                throw new InvalidOperationException($"Category with ID {product.CategoryId} does not exist.");
            }

            ICollection<Image> images = new List<Image>();

            if (product.Images != null && product.Images.Any())
            {
               
                foreach (var image in product.Images)
                {
                  
                    Image img = new Image()
                    {
                        Url = image.Url,
                        Description = image.Description
                    };


                    images.Add(img);
                }

            }


                Product prod = new Product()
            {
                Name = product.Name,
                Description = product.Description,
                OriginalPrice = product.OriginalPrice,
                CategoryId = product.CategoryId,
                Stock = product.Stock,
                Images = images,
                ProductAvailability = ProductAvailability.Active,
                CreatedAt = DateTime.UtcNow
            };
            
            _productService.AddProduct(prod);
            return Ok(prod);

        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            _productService.DeleteProduct(id);
            return Ok($"product with id {id} has been deleted successfully");
        }
    }
}
