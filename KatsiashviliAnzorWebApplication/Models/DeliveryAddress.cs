using System.Text.Json.Serialization;

namespace KatsiashviliAnzorWebApplication.Models
{
    public class DeliveryAddress
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }
        public string Address { get; set; } = null!;
        public bool isDefault { get; set; } = false;
        
    }
}
