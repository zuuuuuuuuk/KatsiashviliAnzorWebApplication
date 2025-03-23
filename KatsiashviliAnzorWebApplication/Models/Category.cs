namespace KatsiashviliAnzorWebApplication.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public ICollection<Product> Products { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }
}
