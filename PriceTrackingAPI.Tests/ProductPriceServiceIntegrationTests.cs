using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PriceTrackingAPI.Data;
using PriceTrackingAPI.Models;
using PriceTrackingAPI.Services;
using Xunit;

namespace PriceTrackingAPI.Tests.Integration
{
    /// <summary>
    /// Integrationstester för ProductPriceService som använder in-memory DB och återanvändbar seed
    /// </summary>
    public class ProductPriceServiceIntegrationTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly ProductPriceService _service;

        public ProductPriceServiceIntegrationTests()
        {
            // Skapa in-memory databas med unikt namn
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            // Seeda databasen med fördefinierad testdata
            TestDataSeeder.Seed(_context);

            _service = new ProductPriceService(_context);
        }

        [Fact]
        public async Task GetLatestPriceAsync_ForProductA_Returns_HighestPrice()
        {
            // Act
            var latest = await _service.GetLatestPriceAsync("A");

            // Assert
            Assert.NotNull(latest);
            Assert.Equal(109.99m, latest.Price);
        }

        [Fact]
        public async Task CreateAsync_Adds_New_ProductPrice()
        {
            // Arrange
            var newPrice = new ProductPrice { ProductId = "E", Price = 123.45m, RecordedAt = DateTime.UtcNow };

            // Act
            var created = await _service.CreateAsync(newPrice);

            // Assert
            Assert.NotNull(created);
            Assert.Equal("E", created.ProductId);
            Assert.Equal(123.45m, created.Price);
            Assert.Contains(_context.ProductPrices, p => p.Id == created.Id);
        }

        [Fact]
        public async Task DeleteAsync_Removes_Existing_Record()
        {
            // Arrange: ta första post för produkt B
            var existing = _context.ProductPrices.First(p => p.ProductId == "B");

            // Act
            var deleted = await _service.DeleteAsync(existing.Id);

            // Assert
            Assert.True(deleted);
            Assert.DoesNotContain(_context.ProductPrices, p => p.Id == existing.Id);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
