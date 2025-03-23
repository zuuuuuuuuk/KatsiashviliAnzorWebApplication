using KatsiashviliAnzorWebApplication.Models;

namespace KatsiashviliAnzorWebApplication.Services.Abstraction
{
    public interface IProductService
    {
        List<Product> GetAllProducts();    
        Product GetProductById(int id);
        void AddProduct(Product product);
        void DeleteProduct (int id);
        

    }
}
