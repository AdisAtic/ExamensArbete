namespace PriceTrackingAPI.Configurations
{
    public class PriceUpdateSettings
    {
        /// <summary>
        /// Antal dagar tillbaka som gamla priser ska behållas.
        /// </summary>
        public int RetentionDays { get; set; } = 180;
        /// <summary>
        /// Url för feed vart priser skall hämtas.
        /// </summary>
        public string FeedUrl { get; set; } = null!;

        // ===================================================================
        // Noderna i XML-feeden (standardvärden)
        public string ProductXPath       { get; set; } = "//*[local-name()='product']";
        public string IdXPath            { get; set; } = "*[local-name()='id']";
        public string PriceXPath         { get; set; } = "*[local-name()='price']";
        public string SalePriceXPath     { get; set; } = "*[local-name()='sale_price']";
        // ===================================================================
    }
}
