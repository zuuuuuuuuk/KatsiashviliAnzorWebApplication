using System.Text.Json.Serialization;

namespace KatsiashviliAnzorWebApplication.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        [JsonIgnore]
        public Product Product { get; set; }
        public int UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }
        public double Rating { get; set; } 
        public string? ReviewText { get; set; }
        public DateTime CreatedAt { get; set; } 
    }
}
