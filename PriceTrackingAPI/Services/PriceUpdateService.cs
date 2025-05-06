using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PriceTrackingAPI.Configurations;
using PriceTrackingAPI.Services.FeedParser;

namespace PriceTrackingAPI.Services
{
    public class PriceUpdateService : IPriceUpdateService
    {
        private readonly IFeedParser _parser;
        private readonly IProductPriceService _priceSvc;
        private readonly PriceUpdateSettings _settings;
        private readonly ILogger<PriceUpdateService> _logger;

        public PriceUpdateService(
            IFeedParser parser,
            IProductPriceService priceService,
            IOptions<PriceUpdateSettings> opts,
            ILogger<PriceUpdateService> logger)
        {
            _parser   = parser;
            _priceSvc = priceService;
            _settings = opts.Value;
            _logger   = logger;
        }

        public async Task UpdatePricesAsync(CancellationToken ct)
        {
            _logger.LogInformation("Startar prisuppdatering");
            try
            {
                var data = await _parser.ParseAsync(ct);
                await _priceSvc.CleanupOldPricesAsync(_settings.RetentionDays);
                _priceSvc.SavePrices(data);
                await _priceSvc.CommitAsync(ct);
                _logger.LogInformation("Prisuppdatering slutförd!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fel vid uppdatering av priser");
            }
        }
    }
}
