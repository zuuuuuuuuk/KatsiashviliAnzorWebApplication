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

        public Cart GetCartByUserId(int userId)
        {
            return _context.Carts.Include(c => c.CartItems).FirstOrDefault(c => c.UserId == userId);
        }

        public void RemoveExpiredCarts()
        {
            var expiredCarts = _context.Carts
       .Where(c => c.ExpiresAt < DateTime.UtcNow)
       .ToList();

            foreach (var cart in expiredCarts)
            {
                if (cart != null && cart.CartItems != null)
                {
                    // Restore stock for each item before deleting the cart
                    foreach (var item in cart.CartItems)
                    {
                        var product = _context.Products.FirstOrDefault(p => p.Id == item.ProductId);
                        if (product != null)
                        {
                            product.Stock += item.Quantity; // Restock products
                            _context.Products.Update(product);
                        }
                    }

                    _context.Carts.Remove(cart);
                }
            }

            _context.SaveChanges();
        }

        public void UpdateCart(Cart cart)
        {
            _context.Carts.Update(cart);
            _context.SaveChanges();
        }
    }
}
