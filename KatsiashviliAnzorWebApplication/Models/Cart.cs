using System.Text.Json.Serialization;

namespace KatsiashviliAnzorWebApplication.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [JsonIgnore]
        public User User { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public  ICollection<CartItem>? CartItems { get; set; }

    }
}
