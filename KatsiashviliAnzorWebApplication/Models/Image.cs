using System.Text.Json.Serialization;

namespace KatsiashviliAnzorWebApplication.Models
{
    public class Image
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string? Description { get; set; }
        public string Url { get; set; }
    }
}
