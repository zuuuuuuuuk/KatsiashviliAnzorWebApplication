using System.Text.Json.Serialization;

namespace KatsiashviliAnzorWebApplication.Models
{
    public class CartItem
    {
        [JsonIgnore]
        public int Id { get; set; }

        [JsonIgnore] 
        public int CartId { get; set; }

        [JsonIgnore]
        public Cart? Cart { get; set; }

        public int ProductId { get; set; }
        public int Quantity { get; set; }

    }
}
