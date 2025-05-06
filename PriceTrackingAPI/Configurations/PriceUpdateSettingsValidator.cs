using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using PriceTrackingAPI.Configurations;

namespace PriceTrackingAPI.Configurations
{
    /// <summary>
    /// Validerar att alla nödvändiga inställningar i PriceUpdateSettings är satta.
    /// </summary>
    public class PriceUpdateSettingsValidator : IValidateOptions<PriceUpdateSettings>
    {
        public ValidateOptionsResult Validate(string? name, PriceUpdateSettings settings)
        {
            var failures = new List<string>();

            // RetentionDays måste vara > 0
            if (settings.RetentionDays <= 0)
                failures.Add($"{nameof(settings.RetentionDays)} måste vara större än 0.");

            // Parser.Type måste vara satt
            var parser = settings.Parser;
            if (string.IsNullOrWhiteSpace(parser.Type))
                failures.Add($"{nameof(parser.Type)} måste sättas (\"Xml\" eller \"Json\").");

            // Om Xml, kontrollera XPath-värden
            if (parser.Type.Equals("Xml", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(parser.Xml.ProductXPath))
                    failures.Add($"{nameof(parser.Xml.ProductXPath)} måste vara satt.");
                if (string.IsNullOrWhiteSpace(parser.Xml.IdXPath))
                    failures.Add($"{nameof(parser.Xml.IdXPath)} måste vara satt.");
                if (string.IsNullOrWhiteSpace(parser.Xml.PriceXPath))
                    failures.Add($"{nameof(parser.Xml.PriceXPath)} måste vara satt.");
            }
            // Om Json, kontrollera JSON-path för produkter
            else if (parser.Type.Equals("Json", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(parser.Json.ProductsPath))
                    failures.Add($"{nameof(parser.Json.ProductsPath)} måste vara satt.");
            }
            else
            {
                failures.Add($"Okänt parser-format: {parser.Type}. Använd \"Xml\" eller \"Json\".");
            }

            return failures.Count == 0
                ? ValidateOptionsResult.Success
                : ValidateOptionsResult.Fail(failures);
        }
    }
}
