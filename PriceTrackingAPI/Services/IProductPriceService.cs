using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PriceTrackingAPI.Models;

namespace PriceTrackingAPI.Services
{
    public interface IProductPriceService
    {
        Task<IEnumerable<ProductPrice>> GetAllAsync();
        Task<ProductPrice?> GetByIdAsync(int id);
        Task<decimal> GetLowestPriceLast30DaysAsync(string productId);
        Task<ProductPrice?> GetLatestPriceAsync(string productId);
        Task<ProductPrice> CreateAsync(ProductPrice productPrice);
        Task UpdateAsync(ProductPrice productPrice);
        Task<bool> DeleteAsync(int id);

        Task CleanupOldPricesAsync(int retentionDays);
        void SavePrices(Dictionary<string, (decimal price, decimal? salePrice)> prices);
        Task CommitAsync(CancellationToken cancellationToken);
    }
}
