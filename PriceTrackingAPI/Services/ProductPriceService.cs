using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PriceTrackingAPI.Data;
using PriceTrackingAPI.Models;

namespace PriceTrackingAPI.Services
{
    public class ProductPriceService : IProductPriceService
    {
        private readonly AppDbContext _context;

        public ProductPriceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductPrice>> GetAllAsync()
        {
            return await _context.ProductPrices.ToListAsync();
        }

        public async Task<ProductPrice?> GetByIdAsync(int id)
        {
            return await _context.ProductPrices.FindAsync(id);
        }

        public async Task<decimal> GetLowestPriceLast30DaysAsync(string productId)
        {
            var cutoff = DateTime.UtcNow.AddDays(-30);
            return await _context.ProductPrices
                .Where(p => p.ProductId == productId && p.RecordedAt >= cutoff)
                .MinAsync(p => (decimal?)p.Price) ?? 0m;
        }

        public async Task<ProductPrice?> GetLatestPriceAsync(string productId)
        {
            return await _context.ProductPrices
                .Where(p => p.ProductId == productId)
                .OrderByDescending(p => p.RecordedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<ProductPrice> CreateAsync(ProductPrice productPrice)
        {
            _context.ProductPrices.Add(productPrice);
            await _context.SaveChangesAsync();
            return productPrice;
        }

        public async Task UpdateAsync(ProductPrice productPrice)
        {
            _context.Entry(productPrice).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.ProductPrices.FindAsync(id);
            if (entity == null) return false;
            _context.ProductPrices.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task CleanupOldPricesAsync(int retentionDays)
        {
            var cutoff = DateTime.UtcNow.AddDays(-retentionDays);
            var stalePrices = await _context.ProductPrices
                .Where(p => p.RecordedAt < cutoff)
                .ToListAsync();

            if (!stalePrices.Any()) return;

            _context.ProductPrices.RemoveRange(stalePrices);
        }

        public void SavePrices(Dictionary<string, (decimal price, decimal? salePrice)> prices)
        {
            foreach (var kvp in prices)
            {
                var articleNumber = kvp.Key;
                var basePrice = kvp.Value.salePrice ?? kvp.Value.price;

                var newPrice = new ProductPrice
                {
                    ProductId = articleNumber,
                    Price = basePrice,
                    RecordedAt = DateTime.UtcNow
                };

                _context.ProductPrices.Add(newPrice);
            }
        }

        public async Task CommitAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
