using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PriceTrackingAPI.Configurations;
using Microsoft.Extensions.Options;

namespace PriceTrackingAPI
{
    public static class OptionsServiceCollectionExtensions
    {
        public static IServiceCollection AddPriceUpdateOptions(this IServiceCollection services, IConfiguration config)
        {
            services.AddOptions<PriceUpdateSettings>()
                .Bind(config.GetSection("PriceUpdateSettings"))
                .ValidateOnStart();

            services.AddSingleton<IValidateOptions<PriceUpdateSettings>, PriceUpdateSettingsValidator>();

            return services;
        }
    }
}
