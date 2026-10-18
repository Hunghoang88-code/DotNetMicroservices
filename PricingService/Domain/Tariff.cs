namespace PricingService.Domain
{
    public class Tariff
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public RuleCollection BasePremiumRules { get; } = new RuleCollection();
        public RuleCollection DiscountMarkupRules { get; } = new RuleCollection();

        public Tariff(string code)
        {
            Code = code;
            Name = code;
        }

        public PriceResult CalculatePrice(Calculation calculation)
        {
            // Placeholder implementation: return zeroed result to satisfy compilation.
            return new PriceResult
            {
                TotalPremium = 0M,
                Covers = new System.Collections.Generic.Dictionary<string, CoverPrice>()
            };
        }
    }

    public class RuleCollection
    {
        public void AddBasePriceRule(string coverCode, string condition, string expression)
        {
            // no-op placeholder
        }

        public void AddPercentMarkup(string condition, decimal percent)
        {
            // no-op placeholder
        }
    }

    public class Calculation
    {
        public string ProductCode { get; }
        public System.DateTimeOffset From { get; }
        public System.DateTimeOffset To { get; }
        public System.Collections.Generic.List<string> Covers { get; }
        public System.Collections.Generic.IDictionary<string, object> Subject { get; }

        public Calculation(string productCode, System.DateTimeOffset from, System.DateTimeOffset to, System.Collections.Generic.List<string> covers, System.Collections.Generic.IDictionary<string, object> subject)
        {
            ProductCode = productCode;
            From = from;
            To = to;
            Covers = covers;
            Subject = subject;
        }
    }

    public class PriceResult
    {
        public decimal TotalPremium { get; set; }
        public System.Collections.Generic.Dictionary<string, CoverPrice> Covers { get; set; } = new();
    }

    public class CoverPrice
    {
        public decimal Price { get; set; }
    }
}
