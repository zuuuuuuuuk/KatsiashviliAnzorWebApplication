using KatsiashviliAnzorWebApplication.Data;
using KatsiashviliAnzorWebApplication.Models;
using KatsiashviliAnzorWebApplication.Services.Abstraction;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace KatsiashviliAnzorWebApplication.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }
        public List<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void DeleteUser(int id)
        {
            _context.Users.Remove(GetUserById(id));
            _context.SaveChanges();
        }

        public User GetUserById(int id)
        {
            return _context.Users
                           .Include(u => u.Orders)  // Eager load Orders
                           .FirstOrDefault(u => u.Id == id);
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public List<DeliveryAddress> GetDeliveryAddressesByUserId(int userId)
        {
            var user = _context.Users
         .Include(u => u.DeliveryAddresses)
         .FirstOrDefault(u => u.Id == userId);

            return user?.DeliveryAddresses.ToList() ?? new List<DeliveryAddress>();
        }

        public void AddDeliveryAddress(int userId, string address)
        {
            var user = _context.Users.Find(userId);
            if (user == null)
                throw new Exception("User not found");

            var newAddress = new DeliveryAddress
            {
                UserId = userId,
                Address = address
            };

            _context.DeliveryAddresses.Add(newAddress);
            _context.SaveChanges();
        }

        public void UpdateDeliveryAddress(int userId, int addressId, string newAddress)
        {
            var deliveryAddress = _context.DeliveryAddresses
          .FirstOrDefault(d => d.Id == addressId && d.UserId == userId);

            if (deliveryAddress == null)
                throw new Exception("Delivery address not found");

            deliveryAddress.Address = newAddress;
            _context.SaveChanges();
        }

        public void DeleteDeliveryAddress(int userId, int addressId)
        {
            var deliveryAddress = _context.DeliveryAddresses
    .FirstOrDefault(d => d.Id == addressId && d.UserId == userId);

            if (deliveryAddress == null)
                throw new Exception("Delivery address not found");

            _context.DeliveryAddresses.Remove(deliveryAddress);
            _context.SaveChanges();
        }
    }
}
