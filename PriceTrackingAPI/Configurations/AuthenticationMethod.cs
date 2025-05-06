namespace PriceTrackingAPI.Configurations
{
    /// <summary>
    /// Vilken autentiseringsmetod som ska användas mot ett API.
    /// </summary>
    public enum AuthenticationMethod
    {
        None,
        Basic,
        Bearer,
        ClientCredentials,
        ServiceAccount
    }
}