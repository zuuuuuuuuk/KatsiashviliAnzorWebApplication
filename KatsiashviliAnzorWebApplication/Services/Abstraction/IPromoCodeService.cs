using KatsiashviliAnzorWebApplication.Models;


namespace KatsiashviliAnzorWebApplication.Services.Abstraction
{
    public interface IPromoCodeService
    {
        void AddPromoCode(PromoCode promoCode);
        List<PromoCode> GetPromoCodeList();
        PromoCode GetPromoCodeById(int id);
        PromoCode getPromoCodeByCode(string code);
        void UpdatePromoCode(PromoCode promoCode);
        void DeletePromoCode(int id);
    }
}
