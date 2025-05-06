using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PriceTrackingAPI.Data;
using PriceTrackingAPI.Models;
using PriceTrackingAPI.Services;
using Xunit;

namespace PriceTrackingAPI.Tests
{
    // Testklass för CRUD-operationer i ProductPriceService
    public class ProductPriceService_CRUD_Tests
    {
        // Hjälpmetod som skapar en in-memory Databas för isolerade tester
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                // Använd ett unikt namn (GUID) för varje testdatabas
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        // Test för CreateAsync: ska lägga till en ny post
        public async Task CreateAsync_Adds_New_Record()
        {
            // Arrange: skapa databas, service och en ProductPrice-entity
            var context = GetInMemoryDbContext();
            var service = new ProductPriceService(context);
            var price = new ProductPrice { ProductId = "ProductOne", Price = 100m, RecordedAt = DateTime.UtcNow };

            //Act: kör metoden som ska spara entiten
            var result = await service.CreateAsync(price);

            //Assert: kontrollera att resultatet är korrekt och att databasen bara inntehåller en post
            Assert.NotNull(result);
            Assert.Equal(100m, result.Price);
            Assert.Single(context.ProductPrices);
        }

        [Fact]
        // Test för GetByIdAsync, ska hämta en befintlig post baserat på Id
        public async Task GetByIdAsync_Returns_Record_When_Exists()
        {
            // Arrange skapa databas med en ProductPrice post
            var context = GetInMemoryDbContext();
            var entity = new ProductPrice { ProductId = "ProductTwo", Price = 200m, RecordedAt = DateTime.UtcNow };
            context.ProductPrices.Add(entity);
            await context.SaveChangesAsync();
            var service = new ProductPriceService(context);

            //Act Hämta posten med rätt Id
            var found = await service.GetByIdAsync(entity.Id);

            //Assert metoden bör returnera ett icke null värde med korrekt pris
            Assert.NotNull(found);
            Assert.Equal(200m, found.Price);
        }

        [Fact]
        // Test för UpdateAsync, ska uppdatera ett befintligt pris
        public async Task UpdateAsync_Modifies_Existing_Price()
        {
            //Arrange: skapa databas med en ProductPrice-post
            var context = GetInMemoryDbContext();
            var entity = new ProductPrice { ProductId = "ProductThree", Price = 300m, RecordedAt = DateTime.UtcNow };
            context.ProductPrices.Add(entity);
            await context.SaveChangesAsync();
            var service = new ProductPriceService(context);

            // Act ändra priset och anropa UpdateAsync
            entity.Price = 350m;
            await service.UpdateAsync(entity);
            // Hämta den uppdaterade entiteten direkt från kontexten
            var updated = await context.ProductPrices.FindAsync(entity.Id);

            // Assert: konrtollera att priset har uppdaterats
            Assert.Equal(350m, updated.Price);
        }

        [Fact]
        // Test för DeleteAsync ska ta bort en befintlig post och returnera true
        public async Task DeleteAsync_Removes_Record_When_Exists()
        {
            // Arrange skapa databas med en ProductPrice-Post
            var context = GetInMemoryDbContext();
            var entity = new ProductPrice { ProductId = "ProductFour", Price = 400m, RecordedAt = DateTime.UtcNow };
            context.ProductPrices.Add(entity);
            await context.SaveChangesAsync();
            var service = new ProductPriceService(context);

            // Act: anropa DeleteAsync med entitetens Id
            var deleted = await service.DeleteAsync(entity.Id);

            // Assert metoden bör returnera true och databasen skall vara tom
            Assert.True(deleted);
            Assert.Empty(context.ProductPrices);
        }

        [Fact]
        // Test för DeleteAsync när posten inte finns och skall returnera false
        public async Task DeleteAsync_Returns_False_When_Not_Exists()
        {
            // Arrange: skapa service med tom databas
            var service = new ProductPriceService(GetInMemoryDbContext());

            // Act anropa DeleteAsync med ID som inte finns
            var result = await service.DeleteAsync(999);

            // Assert metoden skall returnera false
            Assert.False(result);
        }

        [Fact]
        // Test för GetLatestPriceAsync ska returnera nyaste prisposten 
        public async Task GetLatestPriceAsync_Returns_Most_Recent_Record()
        {
            // Arrange skapa databas med två poster för samma produkt vid olika tidpunkter
            var context = GetInMemoryDbContext();
            context.ProductPrices.AddRange(
                new ProductPrice { ProductId = "ProductFive", Price = 10m, RecordedAt = DateTime.UtcNow.AddHours(-2) },
                new ProductPrice { ProductId = "ProductFive", Price = 20m, RecordedAt = DateTime.UtcNow.AddHours(-1) }
            );
            await context.SaveChangesAsync();
            var service = new ProductPriceService(context);

            // Act hämta det senaste priset 
            var latest = await service.GetLatestPriceAsync("ProductFive");

            // Assert ska visa det högre priset på 20m då de är det senaste
            Assert.Equal(20m, latest.Price);
        }
    }
}
