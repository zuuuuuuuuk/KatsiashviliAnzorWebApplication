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


        [HttpGet]
        public IActionResult GetAllCategories()
        {
            var categories = _categoryService.GetAllCategories();
            if (categories == null)
            {
                return BadRequest("Categories are null");
            }
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public IActionResult GetCategoryById(int id)
        {
            var categ = _categoryService.GetCategoryById(id);
            return Ok(categ);
        }


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
                Image = category.Image
            };

            _categoryService.AddCategory(categ);
            return Ok(category);
        }

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

            if (!string.IsNullOrWhiteSpace(category.Name))
                existingCategory.Name = category.Name;

            if (category.ParentId != 0)
                existingCategory.ParentId = category.ParentId;

            if (!string.IsNullOrWhiteSpace(category.Description))
                existingCategory.Description = category.Description;

            if (!string.IsNullOrWhiteSpace(category.Image))
                existingCategory.Image = category.Image;

            _categoryService.UpdateCategory(existingCategory);

            return Ok($"Category with id {id} has been updated successfully");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            _categoryService.DeleteCategory(id);
            return Ok($"category with id {id} has been deleted");
        }

    }
}
