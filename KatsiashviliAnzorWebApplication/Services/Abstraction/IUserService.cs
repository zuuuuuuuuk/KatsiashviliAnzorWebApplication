using KatsiashviliAnzorWebApplication.Models;

namespace KatsiashviliAnzorWebApplication.Services.Abstraction
{
    public interface IUserService
    {
        List<User> GetAllUsers();
        User GetUserById(int id);
        void AddUser(User user);
        void UpdateUser(User user);
        void DeleteUser(int id);

        List<DeliveryAddress>GetDeliveryAddressesByUserId(int userId);

        void AddDeliveryAddress(int userId, string address);

       void UpdateDeliveryAddress(int userId, int addressId, string newAddress, bool isDefault);

        void DeleteDeliveryAddress(int userId, int addressId);


    }
}
