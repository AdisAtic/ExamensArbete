using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PriceTrackingAPI.Data;
using PriceTrackingAPI.Models;
using PriceTrackingAPI.Services;
using Xunit;

namespace PriceTrackingAPI.Tests
{
    // Testklass för ProductPriceService: Innehåller enhetstester för metoderna i tjänsten
    public class ProductPriceServiceTests
    {
        // Hjälpmetod som skapar en ny tom in-memory databas för varje test
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                // Använd en unik databas per test via ett guid-namn
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task CreateAsync_Should_Add_New_Record()
        {
            // Arrange: sätt upp kontext och service samt en produktpris-DTO att skapa
            var context = GetInMemoryDbContext();
            var service = new ProductPriceService(context);
            var dto = new ProductPrice
            {
                ProductId = "TestProduct1",
                Price = 123.45m,
                RecordedAt = DateTime.UtcNow
            };

            // Act: anropa CreateAsync för att lägga till pris
            var created = await service.CreateAsync(dto);

            // Assert: verifiera att returvärdet är korrekt och att posten sparats
            Assert.NotNull(created);
            Assert.Equal("TestProduct1", created.ProductId);
            Assert.Equal(123.45m, created.Price);

            // Kontrollera att posten verkligen finns i databasen
            var all = await context.ProductPrices.ToListAsync();
            Assert.Single(all);
        }

        [Fact]
        public async Task GetLowestPriceLast30DaysAsync_Should_Return_Min_Price()
        {
            // Arrange: skapa kontext och fyll med flera prisvärden på samma produkt
            var context = GetInMemoryDbContext();
            context.ProductPrices.AddRange(new[]
            {
                new ProductPrice { ProductId = "One", Price = 500m, RecordedAt = DateTime.UtcNow.AddDays(-20) },
                new ProductPrice { ProductId = "One", Price = 200m, RecordedAt = DateTime.UtcNow.AddDays(-5) },
                new ProductPrice { ProductId = "One", Price = 400m, RecordedAt = DateTime.UtcNow.AddDays(-10) },
                new ProductPrice { ProductId = "One", Price = 300m, RecordedAt = DateTime.UtcNow.AddDays(-1) },
                // Detta pris skall inte räknas med då det är äldre än 30 dagar
                new ProductPrice { ProductId = "One", Price = 100m, RecordedAt = DateTime.UtcNow.AddDays(-40) },
            });
            await context.SaveChangesAsync();
            var service = new ProductPriceService(context);

            // Act: hämta det lägsta priset under de senaste 30 dagarna
            var lowest = await service.GetLowestPriceLast30DaysAsync("One");

            // Assert: det lägsta priset inom 30-dagarsfönstret är 200m
            Assert.Equal(200m, lowest);
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Record()
        {
            // Arrange: skapa kontext, lägg till en post och spara
            var context = GetInMemoryDbContext();
            var entity = new ProductPrice { ProductId = "X", Price = 1m, RecordedAt = DateTime.UtcNow };
            context.ProductPrices.Add(entity);
            await context.SaveChangesAsync();
            var service = new ProductPriceService(context);

            // Act: anropa DeleteAsync med entitetens Id
            var result = await service.DeleteAsync(entity.Id);

            // Assert: DeleteAsync bör returnera true och databasen ska vara tom
            Assert.True(result);
            Assert.Empty(context.ProductPrices);
        }
    }
}
