using KatsiashviliAnzorWebApplication.Models;

namespace KatsiashviliAnzorWebApplication.Services.Abstraction
{
    public interface ISaleService
    {
        void AddSale(Sale sale);
        List<Sale> GetAllSales();
        List<Sale> GetSalesByProductId(int productId);
        Sale GetSaleById(int id);
        void UpdateSale(Sale sale);
        void DeleteSale(int id);
    }
}
