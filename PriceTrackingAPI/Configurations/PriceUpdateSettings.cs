namespace PriceTrackingAPI.Configurations
{
    /// <summary>
    /// Övergripande inställningar för prisuppdateringen.
    /// </summary>
    public class PriceUpdateSettings
    {
        /// <summary>
        /// Antal dagar som gamla priser ska behållas.
        /// </summary>
        public int RetentionDays { get; set; }

        /// <summary>
        /// Parser‐konfiguration (Xml/Json etc).
        /// </summary>
        public ParserSettings Parser { get; set; } = new();
    }
}
