using System.Globalization;
using System.Xml;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PriceTrackingAPI.Configurations;

namespace PriceTrackingAPI.Services.FeedParser
{
    public class FeedParser : IFeedParser
    {
        private readonly HttpClient _httpClient;
        private readonly PriceUpdateSettings _settings;
        private readonly ILogger<FeedParser> _logger;

        public FeedParser(HttpClient httpClient, IOptions<PriceUpdateSettings> options, ILogger<FeedParser> logger)
        {
            _httpClient = httpClient;
            _settings = options.Value;
            _logger = logger;
        }

        public async Task<Dictionary<string, (decimal price, decimal? salePrice)>> ParseAsync(CancellationToken ct)
        {
            var dict = new Dictionary<string, (decimal price, decimal? salePrice)>();

            var response = await _httpClient.GetAsync(_settings.FeedUrl, ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Misslyckades hämta feed: {Status}", response.StatusCode);
                return dict;
            }

            var xml = await response.Content.ReadAsStringAsync();
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            var products = doc.SelectNodes(_settings.ProductXPath);
            if (products == null) return dict;

            foreach (XmlNode product in products)
            {
                var idNode = product.SelectSingleNode(_settings.IdXPath);
                var priceNode = product.SelectSingleNode(_settings.PriceXPath);
                var saleNode = product.SelectSingleNode(_settings.SalePriceXPath);

                if (idNode == null || priceNode == null) continue;

                var id = idNode.InnerText.Trim();
                if (!decimal.TryParse(priceNode.InnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out var price))
                    continue;

                decimal? salePrice = null;
                if (saleNode != null && !string.IsNullOrWhiteSpace(saleNode.InnerText))
                    salePrice = ParseAmount(saleNode.InnerText);

                dict[id] = (price, salePrice);
            }

            return dict;
        }

        private decimal ParseAmount(string text)
        {
            var segments = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return decimal.Parse(segments[0], CultureInfo.InvariantCulture);
        }
    }
}
