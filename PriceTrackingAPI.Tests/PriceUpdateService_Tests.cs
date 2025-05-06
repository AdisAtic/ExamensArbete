// using System;
// using System.Net;
// using System.Net.Http;
// using System.Threading;
// using System.Threading.Tasks;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Logging.Abstractions;
// using Microsoft.Extensions.Options;
// using Moq;
// using Moq.Protected;
// using PriceTrackingAPI.Configurations;
// using PriceTrackingAPI.Data;
// using PriceTrackingAPI.Models;
// using PriceTrackingAPI.Services;
// using Xunit;

// namespace PriceTrackingAPI.Tests
// {
//     // Testklass för PriceUpdateService: testar både uppdatering från XML-feed och prislogik
//     public class PriceUpdateService_Tests
//     {
//         // Skapar en ny isolerad in-memory-databas för varje test
//         private AppDbContext GetInMemoryDbContext()
//         {
//             var options = new DbContextOptionsBuilder<AppDbContext>()
//                 // Guid ger unikt namn så att tester inte krockar med varandra
//                 .UseInMemoryDatabase(Guid.NewGuid().ToString())
//                 .Options;
//             return new AppDbContext(options);
//         }

//         [Fact]
//         // Testar att UpdatePricesAsync läser in XML, extraherar produktdata och sparar i databasen
//         public async Task UpdatePricesAsync_Should_Insert_Prices_From_Feed()
//         {
//             // Arrange:
//             // 1) Definiera en enkel XML-feed-sträng med två produkter
//             var xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
// <feed>
//   <product><id>A1</id><price>100</price></product>
//   <product><id>B2</id><price>200</price></product>
// </feed>";

//             // 2) Mocka ett HttpMessageHandler så HttpClient returnerar vår xml-sträng
//             var handlerMock = new Mock<HttpMessageHandler>();
//             handlerMock
//                .Protected()
//                .Setup<Task<HttpResponseMessage>>(                
//                   // Interceptar SendAsync för godtycklig HttpRequestMessage
//                   "SendAsync",
//                   ItExpr.IsAny<HttpRequestMessage>(),
//                   ItExpr.IsAny<CancellationToken>())
//                .ReturnsAsync(new HttpResponseMessage
//                {
//                    StatusCode = HttpStatusCode.OK,
//                    Content = new StringContent(xml)
//                });

//             // 3) Skapa HttpClient som använder vår mockade handler
//             var httpClient = new HttpClient(handlerMock.Object);

//             // 4) Konfigurera PriceUpdateSettings som guidar feed-parsern
//             var settings = Options.Create(new PriceUpdateSettings
//             {
//                 RetentionDays  = 30,                         // Hur många dagar bakåt vi behåller historik
//                 FeedUrl        = "http://fake/feed",       // URL vi mockar
//                 ProductXPath   = "//*[local-name()='product']",  // XPath för produkt-noder
//                 IdXPath        = "*[local-name()='id']",         // XPath för id-interna
//                 PriceXPath     = "*[local-name()='price']",      // XPath för pris
//                 SalePriceXPath = "*[local-name()='sale_price']"  // XPath för ev. reapris (ej i denna test)
//             });

//             // 5) Skapa kontext och service-objekt
//             var context = GetInMemoryDbContext();
//             var service = new PriceUpdateService(
//                 context,
//                 NullLogger<PriceUpdateService>.Instance,
//                 httpClient,
//                 settings
//             );

//             // Act: kör metoden som hämtar, parser och sparar produktpriser
//             await service.UpdatePricesAsync(CancellationToken.None);

//             // Assert: verifiera att två rader lagts till med förväntade värden
//             var list = await context.ProductPrices.ToListAsync();
//             Assert.Equal(2, list.Count);
//             Assert.Contains(list, p => p.ProductId == "A1" && p.Price == 100m);
//             Assert.Contains(list, p => p.ProductId == "B2" && p.Price == 200m);
//         }

//         [Fact]
//         // Testar loggerik för att hämta lägsta pris inom retention-perioden
//         public void GetLowestPriceLast30Days_Should_Return_Min_Price_After_Update()
//         {
//             // Arrange: seedar in-memory db med tre poster, en utanför 30 dagars fönster
//             var context = GetInMemoryDbContext();
//             context.ProductPrices.AddRange(new[]
//             {
//                 new ProductPrice { ProductId = "C1", Price = 300m, RecordedAt = DateTime.UtcNow.AddDays(-10) },
//                 new ProductPrice { ProductId = "C1", Price = 150m, RecordedAt = DateTime.UtcNow.AddDays(-5) },
//                 new ProductPrice { ProductId = "C1", Price = 50m, RecordedAt = DateTime.UtcNow.AddDays(-45) },
//                 new ProductPrice { ProductId = "C1", Price = 400m, RecordedAt = DateTime.UtcNow }
//             });
//             context.SaveChanges();  // Spara innan service-instansiering

//             // Settings behöver endast RetentionDays för denna test
//             var settings = Options.Create(new PriceUpdateSettings
//             {
//                 RetentionDays = 30
//             });

//             // Skapa service utan behov av HttpClient eller logger i denna test
//             var service = new PriceUpdateService(
//                 context,
//                 NullLogger<PriceUpdateService>.Instance,
//                 new HttpClient(),  // Ej använd i denna test
//                 settings
//             );

//             // Act: anropa synkront GetLowestPriceLast30Days
//             var lowest = service.GetLowestPriceLast30Days("C1");

//             // Assert: det minsta priset inom 30 dagar är 150m (ej 50m)
//             Assert.Equal(150m, lowest);
//         }
//     }
// }
