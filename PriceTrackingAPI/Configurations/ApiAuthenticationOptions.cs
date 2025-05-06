namespace PriceTrackingAPI.Configurations
{
    /// <summary>
    /// Inställningar för autentisering mot ett API.
    /// </summary>
    public class ApiAuthenticationOptions
    {
        /// <summary>
        /// Väljer vilken metod som används (Basic, Bearer, etc.).
        /// </summary>
        public AuthenticationMethod Method { get; set; } = AuthenticationMethod.None;

        // Basic
        public string? Username { get; set; }
        public string? Password { get; set; }

        // Bearer / ServiceAccount
        public string? Token { get; set; }

        // OAuth2 Client Credentials
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? TokenEndpoint { get; set; }
    }
}
