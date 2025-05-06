namespace PriceTrackingAPI.Configurations
{
    public class ParserSettings
    {
        public string Type { get; set; } = null!;      // Fälttyp, t.ex. "Xml" eller "Json"
        public XmlParserSettings Xml   { get; set; } = null!;
        public JsonParserSettings Json { get; set; } = null!;
    }

    public class XmlParserSettings
    {
        /// <summary>
        /// XPath till produkt-noder.
        /// </summary>
        public string ProductXPath   { get; set; } = null!;

        /// <summary>
        /// XPath till id-element.
        /// </summary>
        public string IdXPath        { get; set; } = null!;

        /// <summary>
        /// XPath till pris-element.
        /// </summary>
        public string PriceXPath     { get; set; } = null!;

        /// <summary>
        /// XPath till försäljningspris-element.
        /// </summary>
        public string SalePriceXPath { get; set; } = null!;
    }

    public class JsonParserSettings
    {
        /// <summary>
        /// JSONPath eller liknande för produktlista.
        /// </summary>
        public string ProductsPath      { get; set; } = null!;

        /// <summary>
        /// Namn på egenskap som innehåller id.
        /// </summary>
        public string IdProperty        { get; set; } = null!;

        /// <summary>
        /// Namn på egenskap som innehåller pris.
        /// </summary>
        public string PriceProperty     { get; set; } = null!;

        /// <summary>
        /// Namn på egenskap som innehåller kampanjpris.
        /// </summary>
        public string SalePriceProperty { get; set; } = null!;
    }
}
