namespace PriceTrackingAPI.Configurations
{
    /// <summary>
    /// Inställningar för en enskild API-klient.
    /// </summary>
    public class ApiClientOptions
    {
        /// <summary>
        /// Bas-URL för API:t (t.ex. https://api.example.com).
        /// </summary>
        public string BaseUrl { get; set; } = null!;

        /// <summary>
        /// Valfri endpoint för feed eller liknande.
        /// </summary>
        public string? FeedEndpoint { get; set; }

        /// <summary>
        /// Autentiseringsinställningar för denna klient.
        /// </summary>
        public ApiAuthenticationOptions Authentication { get; set; } = new ApiAuthenticationOptions();
    }
}
