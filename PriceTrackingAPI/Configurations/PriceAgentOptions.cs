namespace PriceTrackingAPI.Configurations
{
    /// <summary>
    /// Options för PriceAgent-API:t, bindas mot "ApiSettings:PriceAgent".
    /// </summary>
    public class PriceAgentOptions
    {
        /// <summary>
        /// Bas-URL, t.ex. "https://pk-produkter.localtest.me:5001"
        /// </summary>
        public string BaseUrl { get; set; } = null!;

        /// <summary>
        /// Relativ endpoint för feed, t.ex. "priceagent.axd/lowestprice30daysagent?refresh=true"
        /// </summary>
        public string FeedEndpoint { get; set; } = null!;

        /// <summary>
        /// Autentiseringskonfiguration (Basic, Bearer, etc).
        /// </summary>
        public ApiAuthenticationOptions Authentication { get; set; } = new ApiAuthenticationOptions();
    }
}
