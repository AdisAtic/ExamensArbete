using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection; // <-- viktigt!

namespace PriceTrackingAPI.Services
{
    public class LitiumPriceUpdaterService : BackgroundService
    {
        private readonly ILogger<LitiumPriceUpdaterService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public LitiumPriceUpdaterService(
            ILogger<LitiumPriceUpdaterService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var priceUpdateService = scope.ServiceProvider.GetRequiredService<IPriceUpdateService>();
                    await priceUpdateService.UpdatePricesAsync(stoppingToken);
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}
