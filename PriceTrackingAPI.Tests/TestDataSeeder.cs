using System;
using PriceTrackingAPI.Data;
using PriceTrackingAPI.Models;

namespace PriceTrackingAPI.Tests.Integration
{
    /// <summary>
    /// Hjälpklass för att seeda in-memory databasen med testdata
    /// </summary>
    public static class TestDataSeeder
    {
        public static void Seed(AppDbContext context)
        {
            // Rensa eventuella befintliga data
            context.ProductPrices.RemoveRange(context.ProductPrices);

            // Lägg till grundläggande priser för flera produkter
            context.ProductPrices.AddRange(
                new ProductPrice { ProductId = "A", Price = 99.99m, RecordedAt = DateTime.UtcNow.AddDays(-2) },
                new ProductPrice { ProductId = "A", Price = 109.99m, RecordedAt = DateTime.UtcNow.AddDays(-1) },
                new ProductPrice { ProductId = "B", Price = 49.50m, RecordedAt = DateTime.UtcNow }
            );

            // Kantfall: nollpris och framtida datum
            context.ProductPrices.Add(new ProductPrice { ProductId = "C", Price = 0m, RecordedAt = DateTime.UtcNow });
            context.ProductPrices.Add(new ProductPrice { ProductId = "D", Price = 9999m, RecordedAt = DateTime.UtcNow.AddYears(1) });

            context.SaveChanges();
        }
    }
}