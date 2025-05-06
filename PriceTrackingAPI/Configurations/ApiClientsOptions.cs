namespace PriceTrackingAPI.Configurations
{
    /// <summary>
    /// Wrapper-klass som binder alla konfigurerade API-klienter.
    /// </summary>
    public class ApiClientsOptions
    {
        public ApiClientOptions PriceAgent  { get; set; } = new ApiClientOptions();
        public ApiClientOptions Litium      { get; set; } = new ApiClientOptions();
        public ApiClientOptions AnotherApi  { get; set; } = new ApiClientOptions();
    }
}
