using KatsiashviliAnzorWebApplication.Models;

namespace KatsiashviliAnzorWebApplication.Services.Abstraction
{
    public interface ICartService
    {
        void CreateNewCart(Cart cart);
        void DeleteCart(int id);
        void UpdateCart(Cart cart);
        List<Cart> GetAllCarts();
        Cart GetCartById(int id);

    }
}
