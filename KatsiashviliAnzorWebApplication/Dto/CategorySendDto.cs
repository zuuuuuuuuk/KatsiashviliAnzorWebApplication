namespace KatsiashviliAnzorWebApplication.Dto
{
    public class CategorySendDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int? ParentId { get; set; }
        public List<int>? Subcategories { get; set; }
    }
}
