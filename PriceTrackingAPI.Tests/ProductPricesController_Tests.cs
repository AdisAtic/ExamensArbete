using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PriceTrackingAPI.Controllers;
using PriceTrackingAPI.Services;
using Xunit;

namespace PriceTrackingAPI.Tests
{
    // Testklass för ProductPricesController: testar kontrollerns GetLowestPrice-endpoint
    public class ProductPricesController_Tests
    {
        [Fact]
        // Testar att GetLowestPrice returnerar 200 OK med korrekt payload när ett pris finns
        public async Task GetLowestPrice_Returns_Ok_When_Price_Exists()
        {
            // Arrange
            // 1) Mocka IProductPriceService så att GetLowestPriceLast30DaysAsync("X1") returnerar 99m
            var productService = new Mock<IProductPriceService>();
            productService
                .Setup(s => s.GetLowestPriceLast30DaysAsync("X1"))
                .ReturnsAsync(99m);
            
            // 2) Mocka IPriceUpdateService 
            var priceUpdater = new Mock<IPriceUpdateService>();

            // 3) Skapa en controller instans med mockade tjänster
            var controller = new ProductPricesController(productService.Object, priceUpdater.Object);

            // Act
            // Anropa GetLowestPrice("X1") på kontrollern
            var actionResult = await controller.GetLowestPrice("X1");

            // Assert
            // a) Resultatet är en av typen OkObjectResult (HTTP 200)
            var okResult = Assert.IsType<OkObjectResult>(actionResult);

            // b) Extrahera den anonyma Value-egenskapen
            var value = okResult.Value;
            var type = value.GetType();

            // c) Hämta properties productId och lowestPrice via reflection
            var propertyProductId = type.GetProperty("productId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            var propertyLowest    = type.GetProperty("lowestPrice", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            // Säkerställ att båda properties finns
            Assert.NotNull(propertyProductId);
            Assert.NotNull(propertyLowest);

            // d) Läs av värdena
            var productId = propertyProductId.GetValue(value) as string;
            var lowest = Convert.ToDecimal(propertyLowest.GetValue(value));

            // e) Jämför med förväntade värden
            Assert.Equal("X1", productId);
            Assert.Equal(99m, lowest);
        }

        [Fact]
        // Testar att GetLowestPrice returnerar 404 NotFound när inger pris finns
        public async Task GetLowestPrice_Returns_NotFound_When_No_Price()
        {
            // Arrange:
            // Mocka IProductPriceService så att GetLowestPriceLast30Days Async("Y1") returnerar 0 (ingen post)
            var productService = new Mock <IProductPriceService>();
            productService
                .Setup(s => s.GetLowestPriceLast30DaysAsync("Y1"))
                .ReturnsAsync(0m);
            
            // skapa controller med en mockad produkt tjänst och dummu priceUpdater
            var controller = new ProductPricesController(productService.Object, Mock.Of<IPriceUpdateService>());

            // Act anropa GetLowestPrice("Y1")
            var actionResult = await controller.GetLowestPrice("Y1");

            // Assert
            // a) Resultatet är NotFoundObjectResult (HTTP 404)
            var notFound = Assert.IsType<NotFoundObjectResult>(actionResult);

            // b) Meddelandet (Value) innehåller produkt-id "Y1"
            Assert.Contains("Y1", notFound.Value?.ToString());
        }
    }
}
