using System.Globalization;

namespace FastCost.Core;

public static class CostParser
{
    public static decimal Parse(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;

        var trimmed = text.Trim();
        var indexOfDot = trimmed.IndexOf('.');
        var indexOfComma = trimmed.IndexOf(',');
        var numberFormat = new NumberFormatInfo
        {
            NumberDecimalSeparator = indexOfComma > indexOfDot ? "," : ".",
            NumberGroupSeparator = indexOfComma > indexOfDot ? "." : ","
        };

        decimal.TryParse(trimmed, NumberStyles.Number, numberFormat, out var result);
        return Math.Round(result, 2);
    }
}
