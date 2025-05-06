// File: PriceUpdateService_HappyPath_Tests.cs

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
    /// Testklass för att verifiera att PriceUpdateService 
    /// vid lyckat scenario anropar rätt metoder i IProductPriceService.
    /// </summary>
    public class PriceUpdateService_HappyPath_Tests
    {
        private readonly Mock<IFeedParser> _parserMock;
        private readonly Mock<IProductPriceService> _priceServiceMock;
        private readonly IOptions<PriceUpdateSettings> _options;
        private readonly PriceUpdateService _sut;
        private readonly CancellationToken _ct = CancellationToken.None;

        public PriceUpdateService_HappyPath_Tests()
        {
            // Mocka parser och price-service
            _parserMock       = new Mock<IFeedParser>();
            _priceServiceMock = new Mock<IProductPriceService>();

            // Skapa settings med 14 dagars retention
            _options = Options.Create(new PriceUpdateSettings
            {
                RetentionDays = 14
            });

            // Bygg servicen
            _sut = new PriceUpdateService(
                _parserMock.Object,
                _priceServiceMock.Object,
                _options,
                NullLogger<PriceUpdateService>.Instance
            );
        }

        [Fact]
        public async Task UpdatePricesAsync_CallsCleanupSaveAndCommit()
        {
            // Arrange: parsern returnerar en test-dictionary
            var feedData = new Dictionary<string, (decimal price, decimal? salePrice)>
            {
                ["P1"] = (100m, null),
                ["P2"] = (200m, 180m)
            };
            _parserMock
                .Setup(p => p.ParseAsync(_ct))
                .ReturnsAsync(feedData);

            // Act
            await _sut.UpdatePricesAsync(_ct);

            // Assert: verifiera anrop i rätt ordning och antal
            _priceServiceMock.Verify(ps => ps.CleanupOldPricesAsync(14), Times.Once);
            _priceServiceMock.Verify(ps => ps.SavePrices(feedData),        Times.Once);
            _priceServiceMock.Verify(ps => ps.CommitAsync(_ct),           Times.Once);
        }
    }
}
