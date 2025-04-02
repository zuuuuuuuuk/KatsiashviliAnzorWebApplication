using KatsiashviliAnzorWebApplication.Data;
using KatsiashviliAnzorWebApplication.Models;
using KatsiashviliAnzorWebApplication.Services.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace KatsiashviliAnzorWebApplication.Services.Implementation
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;
        public CartService(AppDbContext context) 
        {
          _context = context;
        }

        public void CreateNewCart(Cart cart)
        {
            _context.Carts.Add(cart);
            _context.SaveChanges();
        }

        public void DeleteCart(int id)
        {
            _context.Carts.Remove(GetCartById(id));
            _context.SaveChanges();
        }

        public List<Cart> GetAllCarts()
        {
            return _context.Carts.Include(c => c.CartItems).ToList();   
        }

        public Cart GetCartById(int id)
        {
           return _context.Carts.Include(c => c.CartItems).FirstOrDefault(c => c.Id == id);
        }

        public void UpdateCart(Cart cart)
        {
            _context.Carts.Update(cart);
            _context.SaveChanges();
        }
    }
}
