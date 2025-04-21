using KatsiashviliAnzorWebApplication.Dto;
using KatsiashviliAnzorWebApplication.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;
using KatsiashviliAnzorWebApplication.Models;

namespace KatsiashviliAnzorWebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // Getting all categories

        [HttpGet]
        public IActionResult GetAllCategories()
        {
            var categories = _categoryService.GetAllCategories();
            
            if (categories == null)
            {
                return BadRequest("Categories are null");
            }
            var categorySendDtos = categories.Select(c => new CategorySendDto
            {
                Id = c.Id,
                Name = c.Name,
                ParentId = c.ParentId,
                Description = c.Description,
                Image = c.Image,
                Subcategories = categories
        .Where(sub => sub.ParentId == c.Id)
        .Select(sub => sub.Id)
        .ToList()
            }).ToList();

            return Ok(categorySendDtos);
        }

        // Get specific category with ID

        [HttpGet("{id}")]
        public IActionResult GetCategoryById(int id)
        {
            var categ = _categoryService.GetCategoryById(id);
            return Ok(categ);
        }

        // Create new category 

        [HttpPost]
        public IActionResult AddCategory(CategoryDto category)
        {
            if (category == null)
            {
                return BadRequest("category is null");
            }

            var categ = new Category()
            {
                Name = category.Name,
                ParentId = category.ParentId,
                Description = category.Description,
                Image = category.Image,
                
            };

            _categoryService.AddCategory(categ);
            return Ok(category);
        }

        // Update category

        [HttpPut("{id}")]
        public IActionResult UpdateCategory(int id, CategoryDto category)
        {
            if (category == null)
            {
                return BadRequest("category is null");
            }

            var existingCategory = _categoryService.GetCategoryById(id);
            if (existingCategory == null)
            {
                return BadRequest($"category with id {id} does not exist");
            }

            if (!string.IsNullOrWhiteSpace(category.Name) && category.Name != "string")
                existingCategory.Name = category.Name;

            if (category.ParentId != 0)
                existingCategory.ParentId = category.ParentId;

            if (!string.IsNullOrWhiteSpace(category.Description) && category.Description != "string")
                existingCategory.Description = category.Description;

            if (!string.IsNullOrWhiteSpace(category.Image) && category.Image != "string")
                existingCategory.Image = category.Image;

            _categoryService.UpdateCategory(existingCategory);

            return Ok($"Category with id {id} has been updated successfully");
        }

        // Delete category

        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            var category = _categoryService.GetCategoryById(id);
            if (category == null)
            {
                return BadRequest("can not find category with that id");
            }
            _categoryService.DeleteCategory(id);
            return Ok($"category with id {id} has been deleted");
        }

    }
}
