using System.Text.Json.Serialization;

namespace KatsiashviliAnzorWebApplication.Dto
{
    public class OrderItemDto
    {
        [JsonIgnore]
        public int OrderId { get; set; }

        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
