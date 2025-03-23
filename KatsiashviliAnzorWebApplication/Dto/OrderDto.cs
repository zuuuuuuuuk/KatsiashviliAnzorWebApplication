using KatsiashviliAnzorWebApplication.Enum;
using KatsiashviliAnzorWebApplication.Models;
using System.Text.Json.Serialization;

namespace KatsiashviliAnzorWebApplication.Dto
{
    public class OrderDto
    {
        public int UserId { get; set; }
        
        public ICollection<OrderItemDto>? OrderItems { get; set; }

        
        [JsonIgnore]
        public decimal? TotalAmount { get; set; }
        
        public string ShippingAddress { get; set; }
        public string PaymentMethod { get; set; }

        [JsonIgnore]
        public DateTime CreatedAt { get; set; }
    }
}
