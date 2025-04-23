using System.Text.Json.Serialization;

namespace PriceTrackingAPI.Dtos
{
    public class ProductPriceDto
    {
        [JsonPropertyName("articleNumber")]
        public string ArticleNumber { get; set; } = null!;

        [JsonPropertyName("systemId")]
        public string SystemId { get; set; } = null!; // Måste finnas!

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("minimumQuantity")]
        public decimal MinimumQuantity { get; set; }
    }

    public class ProductPriceResponseDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        [JsonPropertyName("items")]
        public List<ProductPriceDto> Items { get; set; } = new List<ProductPriceDto>();
    }
}
