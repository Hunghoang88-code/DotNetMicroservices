using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace PricingService.Domain;

public class Tariff
{
    public string Code { get; }
    public string Name { get; }

    public RuleCollection BasePremiumRules { get; } = new();
    public RuleCollection DiscountMarkupRules { get; } = new();

    public Tariff(string code)
    {
        Code = code;
        Name = code;
    }

    public PriceResult CalculatePrice(Calculation calculation)
    {
        var coverPrices = new Dictionary<string, CoverPrice>();

        foreach (var cover in calculation.Covers)
        {
            var rule = BasePremiumRules.FindBasePriceRule(cover, calculation.Subject);
            if (rule is null)
            {
                throw new InvalidOperationException($"No base price rule matches cover '{cover}' for tariff '{Code}'.");
            }

            var price = RuleCollection.EvaluateDecimal(rule.Expression, calculation.Subject);
            foreach (var markup in DiscountMarkupRules.GetApplicableMarkups(calculation.Subject))
            {
                price *= markup;
            }

            coverPrices[cover] = new CoverPrice { Price = price };
        }

        return new PriceResult
        {
            TotalPremium = coverPrices.Values.Sum(cover => cover.Price),
            Covers = coverPrices
        };
    }
}

public class RuleCollection
{
    private readonly List<BasePriceRule> basePriceRules = new();
    private readonly List<MarkupRule> markupRules = new();

    public void AddBasePriceRule(string coverCode, string condition, string expression)
    {
        basePriceRules.Add(new BasePriceRule(coverCode, condition, expression));
    }

    public void AddPercentMarkup(string condition, decimal percent)
    {
        markupRules.Add(new MarkupRule(condition, percent));
    }

    internal BasePriceRule FindBasePriceRule(string coverCode, IDictionary<string, object> subject)
    {
        return basePriceRules.FirstOrDefault(rule =>
            rule.CoverCode == coverCode && (rule.Condition is null || EvaluateCondition(rule.Condition, subject)));
    }

    internal IEnumerable<decimal> GetApplicableMarkups(IDictionary<string, object> subject)
    {
        return markupRules
            .Where(rule => EvaluateCondition(rule.Condition, subject))
            .Select(rule => rule.Multiplier);
    }

    internal static decimal EvaluateDecimal(string expression, IDictionary<string, object> subject)
    {
        return Convert.ToDecimal(Evaluate(expression, subject), CultureInfo.InvariantCulture);
    }

    private static bool EvaluateCondition(string condition, IDictionary<string, object> subject)
    {
        return Convert.ToBoolean(Evaluate(condition, subject), CultureInfo.InvariantCulture);
    }

    private static object Evaluate(string expression, IDictionary<string, object> subject)
    {
        var normalized = Regex.Replace(
            expression,
            @"(?<condition>[^?:()]+)\?(?<whenTrue>[^?:()]+):(?<whenFalse>[^?:()]+)",
            match => $"IIF({match.Groups["condition"].Value},{match.Groups["whenTrue"].Value},{match.Groups["whenFalse"].Value})");
        normalized = Regex.Replace(normalized, "\"([^\"]*)\"", match => $"'{match.Groups[1].Value.Replace("'", "''")}'");
        normalized = Regex.Replace(normalized, @"(?<=\d)[mM]\b", string.Empty);
        normalized = normalized.Replace("==", "=").Replace("!=", "<>");
        normalized = Regex.Replace(normalized, @"'(?:''|[^'])*'|[A-Za-z_][A-Za-z0-9_]*", match =>
        {
            if (match.Value.StartsWith("'", StringComparison.Ordinal))
            {
                return match.Value;
            }

            if (match.Value.Equals("IIF", StringComparison.OrdinalIgnoreCase))
            {
                return match.Value;
            }

            if (!subject.TryGetValue(match.Value, out var value) || value is null)
            {
                throw new InvalidOperationException($"Pricing rule references missing answer '{match.Value}'.");
            }

            return value switch
            {
                string text => $"'{text.Replace("'", "''")}'",
                bool boolean => boolean ? "TRUE" : "FALSE",
                IFormattable number => number.ToString(null, CultureInfo.InvariantCulture),
                _ => throw new InvalidOperationException(
                    $"Pricing answer '{match.Value}' has unsupported type '{value.GetType().Name}'.")
            };
        });

        return new DataTable().Compute(normalized, string.Empty);
    }

    internal sealed record BasePriceRule(string CoverCode, string Condition, string Expression);
    private sealed record MarkupRule(string Condition, decimal Multiplier);
}

public class Calculation
{
    public string ProductCode { get; }
    public DateTimeOffset From { get; }
    public DateTimeOffset To { get; }
    public List<string> Covers { get; }
    public IDictionary<string, object> Subject { get; }

    public Calculation(
        string productCode,
        DateTimeOffset from,
        DateTimeOffset to,
        List<string> covers,
        IDictionary<string, object> subject)
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
    public Dictionary<string, CoverPrice> Covers { get; set; } = new();
}

public class CoverPrice
{
    public decimal Price { get; set; }
}
