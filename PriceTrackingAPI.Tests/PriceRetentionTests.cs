using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PriceTrackingAPI.Data;
using PriceTrackingAPI.Models;
using PriceTrackingAPI.Services;
using Xunit;

namespace PriceTrackingAPI.Tests
{
    /// <summary>
    /// Testklass för att verifiera att ProductPriceService.GetLowestPriceLast30DaysAsync
    /// korrekt ignorerar poster äldre än 30 dagar och returnerar 0 om inga relevanta priser finns.
    /// </summary>
    public class PriceRetentionTests
    {
        // Hjälpmetod för att skapa en isolerad in-memory-databas
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task GetLowestPriceLast30DaysAsync_Ignores_OlderThan30Days()
        {
            // Arrange: lägg till tre poster, varav en äldre än 30 dagar
            var context = GetInMemoryDbContext();
            context.ProductPrices.AddRange(
                new ProductPrice { ProductId = "P1", Price = 100m, RecordedAt = DateTime.UtcNow.AddDays(-31) },
                new ProductPrice { ProductId = "P1", Price =  50m, RecordedAt = DateTime.UtcNow.AddDays(-10) },
                new ProductPrice { ProductId = "P1", Price =  75m, RecordedAt = DateTime.UtcNow.AddDays(-5) }
            );
            await context.SaveChangesAsync();
            var service = new ProductPriceService(context);

            // Act: hämta lägsta pris för P1
            var lowest = await service.GetLowestPriceLast30DaysAsync("P1");

            // Assert: 100m ignoreras (äldre än 30 dagar), lägsta av {50, 75} är 50
            Assert.Equal(50m, lowest);
        }

        [Fact]
        public async Task GetLowestPriceLast30DaysAsync_ReturnsZero_IfNoRecentPrices()
        {
            // Arrange: lägg till en post äldre än 30 dagar
            var context = GetInMemoryDbContext();
            context.ProductPrices.Add(
                new ProductPrice { ProductId = "P2", Price = 200m, RecordedAt = DateTime.UtcNow.AddDays(-40) }
            );
            await context.SaveChangesAsync();
            var service = new ProductPriceService(context);

            // Act: hämta lägsta pris för P2
            var lowest = await service.GetLowestPriceLast30DaysAsync("P2");

            // Assert: inga poster inom 30 dagar → returnerar 0m
            Assert.Equal(0m, lowest);
        }
    }
}