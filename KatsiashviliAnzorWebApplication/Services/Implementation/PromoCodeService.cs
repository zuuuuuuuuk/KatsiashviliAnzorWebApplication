using KatsiashviliAnzorWebApplication.Data;
using KatsiashviliAnzorWebApplication.Models;
using KatsiashviliAnzorWebApplication.Services.Abstraction;

namespace KatsiashviliAnzorWebApplication.Services.Implementation
{
    public class PromoCodeService : IPromoCodeService
    {
        private readonly AppDbContext _context;

        public PromoCodeService(AppDbContext context)
        {
            _context = context;
        }

        public List<PromoCode> GetPromoCodeList()
        {
            return _context.PromoCodes.ToList();
        }

        public void AddPromoCode(PromoCode promoCode)
        {
            _context.PromoCodes.Add(promoCode);
            _context.SaveChanges();
        }

        public void DeletePromoCode(int id)
        { 
            _context.PromoCodes.Remove(GetPromoCodeById(id));
            _context.SaveChanges();
        }

        public PromoCode getPromoCodeByCode(string code)
        {
            return _context.PromoCodes.FirstOrDefault(p => p.Code == code);
        }

        public PromoCode GetPromoCodeById(int id)
        {
            return _context.PromoCodes.FirstOrDefault(p => p.Id == id);
        }

        public void UpdatePromoCode(PromoCode promoCode)
        {
            _context.PromoCodes.Update(promoCode);
            _context.SaveChanges();
        }



        public PromoCode BuyPromoCode(int promoId, int userId)
        {
            var templatePromo = _context.PromoCodes.FirstOrDefault(p => p.Id == promoId);
            if (templatePromo == null)
                return null;

            if (templatePromo.IsGlobal)
                throw new InvalidOperationException("Promo code is not buyable (it's global).");


            var userPromo = new PromoCode
            {
                Name = templatePromo.Name,
                Description = templatePromo.Description,
                DiscountValue = templatePromo.DiscountValue,
                Code = GenerateRandomCode(8),
                IsGlobal = false,
                OwnerUserId = userId,
                SourcePromoId = promoId,
            };

            _context.PromoCodes.Add(userPromo);
            _context.SaveChanges();

            return userPromo;
        }

        private string GenerateRandomCode(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}
