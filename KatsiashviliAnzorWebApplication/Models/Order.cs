using KatsiashviliAnzorWebApplication.Enum;
using System.Text.Json.Serialization;

namespace KatsiashviliAnzorWebApplication.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        
        [JsonIgnore]
        public User User { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }
        public OrderStatus Status { get; set; }
        public decimal? TotalAmount { get; set; }
        public string ShippingAddress { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
