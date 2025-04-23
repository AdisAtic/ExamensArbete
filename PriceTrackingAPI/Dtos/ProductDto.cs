using System.Text.Json.Serialization;

namespace PriceTrackingAPI.Dtos
{
    public class ProductDto
    {
        [JsonPropertyName("systemId")]
        public string SystemId { get; set; } = string.Empty;

        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("fields")]
        public Dictionary<string, Dictionary<string, string>> Fields { get; set; } = new();
    }
}