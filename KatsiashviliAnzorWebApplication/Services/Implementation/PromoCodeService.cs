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
    }
}
