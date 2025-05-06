using System;
using Cronos;
using Xunit;

namespace PriceTrackingAPI.Tests
{
    /// <summary>
    /// Tester för att säkerställa att Cronos-parsning och beräkning av nästa körning fungerar korrekt.
    /// </summary>
    public class CronExpressionTests
    {
        /// <summary>
        /// Verifierar att ett giltigt cron-uttryck med sekunder/parsing med IncludeSeconds
        /// ger rätt nästa körningstid för olika tidpunkter "now".
        /// </summary>
        /// <param name="expression">Cron-uttryck (sekund minut timme dag månad veckodag)</param>
        /// <param name="nowIso">Nutid som ISO 8601-sträng</param>
        /// <param name="expectedIso">Förväntad nästa körning som ISO 8601-sträng</param>
        [Theory]
        [InlineData("0 0 10,22 * * *", "2025-05-06T09:00:00Z", "2025-05-06T10:00:00Z")]
        [InlineData("0 0 10,22 * * *", "2025-05-06T10:00:00Z", "2025-05-06T22:00:00Z")]
        [InlineData("0 0 10,22 * * *", "2025-05-06T23:00:00Z", "2025-05-07T10:00:00Z")]
        public void NextOccurrence_ShouldMatchExpected(
            string expression,
            string nowIso,
            string expectedIso)
        {
            // Arrange: parsar cron-uttrycket inklusive sekunder
            var cron = CronExpression.Parse(expression, CronFormat.IncludeSeconds);

            // Parsea ISO-strängar till DateTimeOffset
            var now      = DateTimeOffset.Parse(nowIso);
            var expected = DateTimeOffset.Parse(expectedIso);

            // Act: beräkna nästa förekomst utifrån 'now'
            var next = cron.GetNextOccurrence(now, TimeZoneInfo.Utc);

            // Assert: finns ett värde och det matchar förväntat
            Assert.True(next.HasValue, "CronExpression.GetNextOccurrence returnerade null.");
            Assert.Equal(expected, next.Value);
        }

        /// <summary>
        /// Verifierar att ett ogiltigt cron‐uttryck ger CronFormatException.
        /// </summary>
        [Fact]
        public void InvalidCronExpression_ShouldThrowCronFormatException()
        {
            // Arrange: en sträng som inte överensstämmer med cron‐syntax
            var badExpr = "invalid * cron";

            // Act & Assert: parsing ska kasta CronFormatException
            Assert.Throws<CronFormatException>(() =>
                CronExpression.Parse(badExpr, CronFormat.IncludeSeconds)
            );
        }
    }
}
