using System.Threading;
using System.Threading.Tasks;

namespace PriceTrackingAPI.Services
{
    public interface IPriceUpdateService
    {
        /// <summary>
        /// Kör prisuppdateringen asynkront.
        /// </summary>
        Task UpdatePricesAsync(CancellationToken cancellationToken);
    }
}
