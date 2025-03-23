using KatsiashviliAnzorWebApplication.Data;
using KatsiashviliAnzorWebApplication.Models;
using KatsiashviliAnzorWebApplication.Services.Abstraction;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace KatsiashviliAnzorWebApplication.Services.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;
        public OrderService(AppDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public List<Order> GetOrdersByUserId(int id)
        {
            // Query the orders directly and include the OrderItems
            return _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == id)
                .ToList();
        }




        public List<Order> GetAllOrders()
        {
            return _context.Orders.Include(o => o.OrderItems).ToList();
        }


        public void AddOrder(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public void UpdateOrder(Order order)
        {
            _context.Orders.Update(order);
            _context.SaveChanges();
        }

        public Order GetOrderById(int id)
        {
            return _context.Orders.Include(o =>  o.OrderItems).FirstOrDefault(o => o.Id == id);
        }

        public void DeleteOrder(int id)
        {
            _context.Orders.Remove(GetOrderById(id));
            _context.SaveChanges();
           
        }
    }
}
