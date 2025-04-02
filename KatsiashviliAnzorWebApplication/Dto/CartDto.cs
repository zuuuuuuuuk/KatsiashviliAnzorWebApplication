using KatsiashviliAnzorWebApplication.Models;

namespace KatsiashviliAnzorWebApplication.Dto
{
    public class CartDto
    {
        public int UserId { get; set; }
        public ICollection<CartItem>? CartItems { get; set; }
    }
}
