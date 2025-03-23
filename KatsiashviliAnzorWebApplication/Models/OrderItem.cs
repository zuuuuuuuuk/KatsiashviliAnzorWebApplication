using System.Text.Json.Serialization;

namespace KatsiashviliAnzorWebApplication.Models
{
    public class OrderItem
    {
        [JsonIgnore]
        public int Id { get; set; }
        
        [JsonIgnore]
        public int OrderId { get; set; }

        [JsonIgnore]
        public Order Order { get; set; }

        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
