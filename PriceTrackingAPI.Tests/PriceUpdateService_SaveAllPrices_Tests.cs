using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using PriceTrackingAPI.Configurations;
using PriceTrackingAPI.Services;
using PriceTrackingAPI.Services.FeedParser;
using Xunit;

namespace PriceTrackingAPI.Tests
{
    /// <summary>
    /// Testklass för PriceUpdateService - enhets- och felvägstester
    /// </summary>
    public class PriceUpdateService_Tests
    {
        private readonly Mock<IFeedParser> _parserMock;
        private readonly Mock<IProductPriceService> _priceServiceMock;
        private readonly IOptions<PriceUpdateSettings> _options;
        private readonly PriceUpdateService _sut;
        private readonly CancellationToken _ct = CancellationToken.None;

        public PriceUpdateService_Tests()
        {
            _parserMock = new Mock<IFeedParser>();
            _priceServiceMock = new Mock<IProductPriceService>();
            _options = Options.Create(new PriceUpdateSettings { RetentionDays = 14 });
            _sut = new PriceUpdateService(
                _parserMock.Object,
                _priceServiceMock.Object,
                _options,
                NullLogger<PriceUpdateService>.Instance
            );
        }

        [Fact]
        public async Task UpdatePricesAsync_HappyPath_CallsDependencies()
        {
            // Arrange
            var feedData = new Dictionary<string, (decimal price, decimal? salePrice)>
            {
                ["X"] = (5m, null),
                ["Y"] = (10m, 8m)
            };
            _parserMock
                .Setup(p => p.ParseAsync(_ct))
                .ReturnsAsync(feedData);

            // Act
            await _sut.UpdatePricesAsync(_ct);

            // Assert
            _priceServiceMock.Verify(ps => ps.CleanupOldPricesAsync(14), Times.Once);
            _priceServiceMock.Verify(ps => ps.SavePrices(feedData), Times.Once);
            _priceServiceMock.Verify(ps => ps.CommitAsync(_ct), Times.Once);
        }

        [Fact]
        public void InterfaceContainsGetLowestPriceLast30DaysAsync()
        {
            // Verifiera att gränssnittet IProductPriceService deklarerar metoden
            var interfaceType = typeof(IProductPriceService);
            var method = interfaceType.GetMethod("GetLowestPriceLast30DaysAsync");
            Assert.NotNull(method);
            Assert.Equal(typeof(Task<decimal>), method.ReturnType);
        }
    }
}
