using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PriceTrackingAPI.Configurations;
using PriceTrackingAPI.Services.FeedParser;

namespace PriceTrackingAPI.Services
{
    public class PriceUpdateService : IPriceUpdateService
    {
        private readonly IFeedParser _parser;
        private readonly IProductPriceService _priceService;
        private readonly PriceUpdateSettings _settings;
        private readonly ILogger<PriceUpdateService> _logger;

        public PriceUpdateService(
            IFeedParser parser,
            IProductPriceService priceService,
            IOptions<PriceUpdateSettings> options,
            ILogger<PriceUpdateService> logger)
        {
            _parser = parser;
            _priceService = priceService;
            _settings = options.Value;
            _logger = logger;
        }

        public async Task UpdatePricesAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Startar prisuppdatering");

            try
            {
                var prices = await _parser.ParseAsync(cancellationToken);

                await _priceService.CleanupOldPricesAsync(_settings.RetentionDays);
                _priceService.SavePrices(prices);
                await _priceService.CommitAsync(cancellationToken);

                _logger.LogInformation("Prisuppdatering slutförd!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fel vid uppdatering av priser");
            }
        }
    }
}
