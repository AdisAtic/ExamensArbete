namespace PriceTrackingAPI.Models
{
    public class ProductPrice
    {
        public int Id { get; set; }
        public string? ProductId { get; set; }
        public decimal Price { get; set; } 
        public DateTime RecordedAt { get; set; }
    }
}
