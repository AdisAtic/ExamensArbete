namespace PriceTrackingAPI.Configurations
{
    public class LitiumSettings
    {
        /// <summary>
        /// Service account–token (Litium).
        /// </summary>
        public string ServiceAccountToken { get; set; } = string.Empty;

        /// <summary>
        /// Cron-uttryck för hur ofta updateringen ska köras.
        /// Format: sekund minut timme dag månad veckodag.
        /// </summary>
        public string CronExpression { get; set; } = null!;

        /// <summary>
        /// Tidszons-ID, t.ex. "Europe/Stockholm".
        /// </summary>
        public string TimeZone { get; set; } = null!;
    }
}
