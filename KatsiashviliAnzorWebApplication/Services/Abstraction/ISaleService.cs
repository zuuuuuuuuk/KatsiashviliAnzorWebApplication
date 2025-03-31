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
        void ActivateSale(int saleId, int days);
        void ActivateSaleWithDefaultDates(int saleId); // for background service use
        void DeactivateSale(int saleId);
        void UpdateProductDiscountedPrice(Product product);
    }
}
