using KatsiashviliAnzorWebApplication.Enum;


namespace KatsiashviliAnzorWebApplication.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserRole Role { get; set; }
        public ICollection<Order>? Orders { get; set; }
        public List<int>? FavoriteProductIds { get; set; } = new();
        public ICollection<DeliveryAddress> DeliveryAddresses { get; set; } = new List<DeliveryAddress>();
        public DateTime CreatedAt { get; set; } 
    }
}
