using System.Threading;
using System.Threading.Tasks;


namespace PriceTrackingAPI.Services;

public interface IPriceUpdateService
{
    Task UpdatePricesAsync(CancellationToken cancellationToken);
}
