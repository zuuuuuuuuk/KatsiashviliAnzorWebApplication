using KatsiashviliAnzorWebApplication.Enum;
using System.Text.Json.Serialization;

namespace KatsiashviliAnzorWebApplication.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        [JsonIgnore]
        public ICollection<Sale>? Sales { get; set; } //navigation property
        public int CategoryId { get; set; }

        [JsonIgnore]
        public Category? Category { get; set; } //navigation property
        public int Stock {  get; set; }
        public ICollection<Image>? Images { get; set; }
        public ProductAvailability ProductAvailability { get; set; } // is active - 0(default)   or not active - 1
        public DateTime CreatedAt { get; set; }
        [JsonIgnore]
        public ICollection<Review>? Reviews { get; set; }
        public double? AverageRating { get; set; }
        
    }
}
