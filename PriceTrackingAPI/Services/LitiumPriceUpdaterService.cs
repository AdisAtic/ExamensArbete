using System;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PriceTrackingAPI.Configurations;

namespace PriceTrackingAPI.Services
{
    public class LitiumPriceUpdaterService : BackgroundService
    {
        private readonly ILogger<LitiumPriceUpdaterService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly CronExpression _cron;
        private readonly TimeZoneInfo _timeZone;

        public LitiumPriceUpdaterService(
            ILogger<LitiumPriceUpdaterService> logger,
            IServiceProvider serviceProvider,
            IOptions<LitiumSettings> options)
        {
            _logger          = logger;
            _serviceProvider = serviceProvider;

            var cronOptions = options.Value;
            _cron = CronExpression.Parse(
                    cronOptions.CronExpression,
                    CronFormat.IncludeSeconds
                );
            try
            {
                _timeZone = TimeZoneInfo.FindSystemTimeZoneById(cronOptions.TimeZone);
            }
            catch
            {
                _logger.LogWarning("Tidszon '{Zone}' ej funnen, använder UTC.", cronOptions.TimeZone);
                _timeZone = TimeZoneInfo.Utc;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("LitiumPriceUpdaterService startad med cron-schedule.");

             try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var updater = scope.ServiceProvider.GetRequiredService<IPriceUpdateService>();
                    _logger.LogInformation("Initial prisuppdatering vid start ({Time}).", DateTimeOffset.Now);
                    await updater.UpdatePricesAsync(stoppingToken);
                    _logger.LogInformation("Initial prisuppdatering klar ({Time}).", DateTimeOffset.Now);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fel vid initial prisuppdatering.");
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                // Hitta nästa gång cron-uttrycket skall köras
                var next = _cron.GetNextOccurrence(DateTimeOffset.Now, _timeZone);
                if (!next.HasValue)
                {
                    _logger.LogWarning("Ingen nästa körning kunde beräknas från cron-uttrycket.");
                    break;
                }

                var delay = next.Value - DateTimeOffset.Now;
                _logger.LogInformation("Nästa körning om {Delay}.", delay);

                // Vänta fram till dess
                await Task.Delay(delay, stoppingToken);

                // Utför uppdatering i ett scope
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var updater = scope.ServiceProvider.GetRequiredService<IPriceUpdateService>();

                    _logger.LogInformation("Startar schemalagd prisuppdatering ({Time}).", DateTimeOffset.Now);
                    await updater.UpdatePricesAsync(stoppingToken);
                    _logger.LogInformation("Prisuppdatering klar ({Time}).", DateTimeOffset.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Fel vid schemalagd prisuppdatering.");
                }
            }
        }
    }
}
