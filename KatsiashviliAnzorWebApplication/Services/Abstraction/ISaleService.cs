using KatsiashviliAnzorWebApplication.Models;

namespace KatsiashviliAnzorWebApplication.Services.Abstraction
{
    public interface ISaleService
    {
        void AddSale(Sale sale);
        void DeleteSale(int id);
        void UpdateSale(Sale sale);
        List<Sale> GetAllSales();
        Sale GetSaleById(int id);
        List<Sale> GetSalesByProductId(int productId);
        void ActivateSale(int saleId);
        void DeactivateSale(int saleId);

    }
}
