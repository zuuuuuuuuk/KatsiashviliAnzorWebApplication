using KatsiashviliAnzorWebApplication.Data;
using KatsiashviliAnzorWebApplication.Models;
using KatsiashviliAnzorWebApplication.Services.Abstraction;
using Microsoft.AspNetCore.Http.HttpResults;

namespace KatsiashviliAnzorWebApplication.Services.Implementation
{
    public class AdvertisementService : IAdvertisementService
    {
        private readonly AppDbContext _context;
        public AdvertisementService(AppDbContext context)
        {
            _context = context;
        }
        public void CreateAdvertisement(Advertisement advertisement)
        {
            _context.Add(advertisement);
            _context.SaveChanges();
        }

        public void DeleteAdvertisement(int id)
        {
            _context.Advertisements.Remove(GetAdvertisementById(id));
            _context.SaveChanges();
        }

        public Advertisement GetAdvertisementById(int id)
        {
            return _context.Advertisements.FirstOrDefault(a => a.Id == id);
            
        }

        public List<Advertisement> GetAllAdvertisements()
        {
            return _context.Advertisements.ToList();
        }

        public void UpdateAdvertisement(Advertisement advertisement)
        {
            _context.Advertisements.Update(advertisement);
            _context.SaveChanges();
        }
    }
}
