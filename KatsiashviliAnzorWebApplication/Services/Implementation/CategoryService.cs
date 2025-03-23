using KatsiashviliAnzorWebApplication.Data;
using KatsiashviliAnzorWebApplication.Models;
using KatsiashviliAnzorWebApplication.Services.Abstraction;

namespace KatsiashviliAnzorWebApplication.Services.Implementation
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;
        private readonly ICategoryService _categoryService;

        public CategoryService(AppDbContext context)
        {
            _context = context;
            
        }

        public void AddCategory(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        public void DeleteCategory(int id)
        {
            _context.Categories.Remove(GetCategoryById(id));
            _context.SaveChanges();
        }

        public List<Category> GetAllCategories()
        {
            return _context.Categories.ToList();
        }

        public Category GetCategoryById(int id)
        {
            return _context.Categories.FirstOrDefault(c => c.Id == id);
        }

        public void UpdateCategory(Category category)
        {
            _context.Categories.Update(category);
            _context.SaveChanges();
        }
    }
}
