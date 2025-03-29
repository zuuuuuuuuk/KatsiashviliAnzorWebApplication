using KatsiashviliAnzorWebApplication.Models;

namespace KatsiashviliAnzorWebApplication.Services.Abstraction
{
    public interface IAdvertisementService
    {
        void CreateAdvertisement(Advertisement Advertisement);
        Advertisement GetAdvertisementById(int id);
        List<Advertisement> GetAllAdvertisements();
        void UpdateAdvertisement(Advertisement advertisement);
        void DeleteAdvertisement(int id);
    }
}
