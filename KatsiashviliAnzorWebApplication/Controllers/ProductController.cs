using Microsoft.AspNetCore.Mvc;
using KatsiashviliAnzorWebApplication.Models;
using KatsiashviliAnzorWebApplication.Enum;
using KatsiashviliAnzorWebApplication.Dto;
using KatsiashviliAnzorWebApplication.Services;
using KatsiashviliAnzorWebApplication.Services.Abstraction;
using Microsoft.EntityFrameworkCore;
using KatsiashviliAnzorWebApplication.Data;
using Microsoft.AspNetCore.Authorization;

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


        [HttpGet("category/{categoryId}")]
        public IActionResult GetProductsByCategoryId(int categoryId)
        {
            var prods = _productService.GetProductsByCategoryId(categoryId).ToList();
            return Ok(prods);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public IActionResult AddProduct(ProductDto product)
        {


            if (product == null)
            {
                return BadRequest("product is null");
            }

            // this checks if the category exists
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
            return Ok("product was added successfully");

        }
        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, ProductDto product)
        {
            var existingProduct = _productService.GetProductById(id);


            if (product == null)
            {
                return BadRequest("product is null");
            }
            if (string.IsNullOrWhiteSpace(product.Name) || product.Name == "string")
            {
                product.Name = existingProduct.Name;
            }
            existingProduct.Name = product.Name;

            if (!string.IsNullOrWhiteSpace(product.Description) && product.Description != "string")
                existingProduct.Description = product.Description;
            if (product.OriginalPrice > 0)
                existingProduct.OriginalPrice = product.OriginalPrice;
            if (product.Stock > 0)
                existingProduct.Stock = product.Stock;
            if (product.CategoryId > 0)
                existingProduct.CategoryId = product.CategoryId;

            if (product.Images != null && product.Images.Any())

            {
                // This clears EF's current tracked collection so it can save the new one
                existingProduct.Images?.Clear();

                existingProduct.Images = product.Images.Select(img => new Image
                {
                    Url = img.Url,
                    Description = img.Description
                }).ToList();
            }
            _productService.UpdateProduct(existingProduct);

            return Ok(new { message = "product updated successfully" });
        }
        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
            {
                return BadRequest("can not find product with that id");
            }
            _productService.DeleteProduct(id);
            return Ok($"product with id {id} has been deleted successfully");
        }
    }
}
