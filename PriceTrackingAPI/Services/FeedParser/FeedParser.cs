using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PriceTrackingAPI.Configurations;

namespace PriceTrackingAPI.Services.FeedParser
{
    public class FeedParser : IFeedParser
    {
        private readonly HttpClient _http;
        private readonly PriceAgentOptions _agent;
        private readonly PriceUpdateSettings _settings;
        private readonly ILogger<FeedParser> _logger;

        public FeedParser(
            HttpClient httpClient,
            IOptions<PriceAgentOptions> agentOpts,
            IOptions<PriceUpdateSettings> updateOpts,
            ILogger<FeedParser> logger)
        {
            _http      = httpClient;
            _agent     = agentOpts.Value;
            _settings  = updateOpts.Value;
            _logger    = logger;
        }

        public async Task<Dictionary<string, (decimal price, decimal? salePrice)>> ParseAsync(CancellationToken ct)
        {
            var result = new Dictionary<string, (decimal, decimal?)>();

            // Anropa rätt endpoint från ApiSettings:PriceAgent
            var resp = await _http.GetAsync(_agent.FeedEndpoint, ct);
            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogError("Misslyckades hämta feed: {Status}", resp.StatusCode);
                return result;
            }

            var raw = await resp.Content.ReadAsStringAsync(ct);
            var parserType = _settings.Parser.Type;

            if (parserType.Equals("Xml", StringComparison.OrdinalIgnoreCase))
            {
                var doc = new XmlDocument();
                doc.LoadXml(raw);
                var cfg = _settings.Parser.Xml;
                var products = doc.SelectNodes(cfg.ProductXPath);
                if (products != null)
                {
                    foreach (XmlNode product in products)
                    {
                        var idNode    = product.SelectSingleNode(cfg.IdXPath);
                        var priceNode = product.SelectSingleNode(cfg.PriceXPath);
                        var saleNode  = product.SelectSingleNode(cfg.SalePriceXPath);
                        if (idNode == null || priceNode == null) continue;

                        var id = idNode.InnerText.Trim();
                        if (!decimal.TryParse(priceNode.InnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out var price))
                            continue;

                        decimal? salePrice = null;
                        if (saleNode != null && !string.IsNullOrWhiteSpace(saleNode.InnerText))
                            salePrice = decimal.Parse(saleNode.InnerText.Split(' ')[0], CultureInfo.InvariantCulture);

                        result[id] = (price, salePrice);
                    }
                }
            }
            else if (parserType.Equals("Json", StringComparison.OrdinalIgnoreCase))
            {
                using var doc = JsonDocument.Parse(raw);
                var config = _settings.Parser.Json;
                // Exempel: antag att $.items är en array
                if (doc.RootElement.TryGetProperty("items", out var items))
                {
                    foreach (var item in items.EnumerateArray())
                    {
                        if (!item.TryGetProperty(config.IdProperty, out var idProp)) continue;
                        if (!item.TryGetProperty(config.PriceProperty, out var priceProp)) continue;

                        var id = idProp.GetString();
                        var price = priceProp.GetDecimal();
                        decimal? salePrice = null;
                        if (item.TryGetProperty(config.SalePriceProperty, out var saleProp)
                            && saleProp.ValueKind == JsonValueKind.Number)
                        {
                            salePrice = saleProp.GetDecimal();
                        }

                        if (!string.IsNullOrEmpty(id))
                            result[id] = (price, salePrice);
                    }
                }
            }
            else
            {
                _logger.LogError("Okänt parser‐format: {Type}", parserType);
            }

            return result;
        }
    }
}
