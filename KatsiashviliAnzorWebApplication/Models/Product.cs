namespace KatsiashviliAnzorWebApplication.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public int Stock {  get; set; }
        public List<string> Images { get; set; }
        public enum Status
        {
            ACTIVE,
            INACTIVE
        }
        public DateTime CreatedAt { get; set; }
    }
}
