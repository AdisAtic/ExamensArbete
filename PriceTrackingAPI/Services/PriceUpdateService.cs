using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Logging;
using PriceTrackingAPI.Configurations;
using PriceTrackingAPI.Data;
using PriceTrackingAPI.Models;

namespace PriceTrackingAPI.Services
{
    public class PriceUpdateService : IPriceUpdateService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<PriceUpdateService> _logger;
        private readonly HttpClient _httpClient;
        private readonly LitiumSettings _litiumSettings;

        public PriceUpdateService(
            AppDbContext dbContext,
            ILogger<PriceUpdateService> logger,
            HttpClient httpClient,
            LitiumSettings litiumSettings)
        {
            _dbContext = dbContext;
            _logger = logger;
            _httpClient = httpClient;
            _litiumSettings = litiumSettings;
        }

        public async Task UpdatePricesAsync(CancellationToken ct)
        {
            _logger.LogInformation("🔁 Startar prisuppdatering (endast Google Shopping)...");

            try
            {
                var googlePrices = await FetchGoogleShoppingAsync(ct);

                foreach (var kvp in googlePrices)
                {
                    string articleNumber = kvp.Key;
                    decimal price = kvp.Value.salePrice ?? kvp.Value.price;

                    var existing = _dbContext.ProductPrices
                        .FirstOrDefault(p => p.ProductId == articleNumber);

                    var pastPrices = _dbContext.ProductPrices
                        .Where(p => p.ProductId == articleNumber
                                    && p.RecordedAt >= DateTime.UtcNow.AddDays(-30))
                        .Select(p => p.LowestPriceLast30Days)
                        .ToList();

                    decimal lowest30 = pastPrices.Any() ? pastPrices.Min() : price;

                    if (existing == null)
                    {
                        _dbContext.ProductPrices.Add(new ProductPrice
                        {
                            ProductId = articleNumber,
                            LowestPriceLast30Days = lowest30,
                            RecordedAt = DateTime.UtcNow
                        });

                        _logger.LogInformation("🆕 Lagt till nytt pris: {Price} för {ArticleNumber}", price, articleNumber);
                    }
                    else
                    {
                        if (price < existing.LowestPriceLast30Days)
                        {
                            existing.LowestPriceLast30Days = price;
                            _logger.LogInformation("⬇️ Nytt lägsta pris registrerat för {ArticleNumber}: {Price}", articleNumber, price);
                        }
                        else if (lowest30 < existing.LowestPriceLast30Days)
                        {
                            existing.LowestPriceLast30Days = lowest30;
                            _logger.LogInformation("🔄 Uppdaterat lägsta pris de senaste 30 dagarna för {ArticleNumber}: {Lowest30}", articleNumber, lowest30);
                        }
                    }
                }

                await _dbContext.SaveChangesAsync(ct);
                _logger.LogInformation("✅ Prisuppdatering slutförd!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Fel vid uppdatering av priser: {ex.Message}");
            }
        }

        private async Task<Dictionary<string, (decimal price, decimal? salePrice)>> FetchGoogleShoppingAsync(CancellationToken ct)
        {
            var dict = new Dictionary<string, (decimal, decimal?)>();
            var url = "https://pk-produkter.localtest.me:5001/PriceAgent.axd/GoogleShopping?Refresh=True";

            var response = await _httpClient.GetAsync(url, ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Misslyckades hämta GoogleShopping feed: {response.StatusCode}");
                return dict;
            }

            var xml = await response.Content.ReadAsStringAsync();
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var nsm = new XmlNamespaceManager(doc.NameTable);
            nsm.AddNamespace("g", "http://base.google.com/ns/1.0");

            foreach (XmlNode item in doc.SelectNodes("//item", nsm))
            {
                var id = item.SelectSingleNode("g:id", nsm)?.InnerText;
                var priceText = item.SelectSingleNode("g:price", nsm)?.InnerText;
                if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(priceText))
                    continue;

                decimal price = ParseAmount(priceText);
                decimal? sale = null;
                var saleText = item.SelectSingleNode("g:sale_price", nsm)?.InnerText;
                if (!string.IsNullOrWhiteSpace(saleText))
                    sale = ParseAmount(saleText);

                dict[id] = (price, sale);
            }

            return dict;
        }

        private decimal ParseAmount(string text)
        {
            var segs = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return decimal.Parse(segs[0], CultureInfo.InvariantCulture);
        }
    }
}
