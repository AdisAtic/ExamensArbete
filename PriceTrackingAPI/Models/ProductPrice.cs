namespace PriceTrackingAPI.Models
{
    public class ProductPrice
    {
        public int Id { get; set; }
        public string? ProductId { get; set; }
        public decimal LowestPriceLast30Days { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}
