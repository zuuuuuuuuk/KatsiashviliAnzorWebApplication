using KatsiashviliAnzorWebApplication.Models;

namespace KatsiashviliAnzorWebApplication.Services.Abstraction
{
    public interface IProductService
    {
        List<Product> GetAllProducts();    
        List<Product> GetProductsByCategoryId(int id);
        Product GetProductById(int id);
        void AddProduct(Product product);
        void DeleteProduct (int id);
        void UpdateProduct (Product product);

    }
}
