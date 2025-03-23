using KatsiashviliAnzorWebApplication.Models;

namespace KatsiashviliAnzorWebApplication.Services.Abstraction
{
    public interface ICategoryService
    {
        void AddCategory(Category category);
        List<Category> GetAllCategories();
        Category GetCategoryById(int id);
        void DeleteCategory(int id);
        void UpdateCategory(Category category);
    }
}
