using FastCost.Core;
using Xunit;

namespace FastCostTests;

public class CostParserTests
{
    [Theory]
    [InlineData("10", 10)]
    [InlineData("10.5", 10.5)]
    [InlineData("10,5", 10.5)]
    [InlineData("1234.56", 1234.56)]
    [InlineData("1234,56", 1234.56)]
    [InlineData("1.234,56", 1234.56)]
    [InlineData("1,234.56", 1234.56)]
    [InlineData("0.99", 0.99)]
    [InlineData("0,99", 0.99)]
    public void Parse_ShouldHandleDecimalSeparators(string input, decimal expected)
    {
        Assert.Equal(expected, CostParser.Parse(input));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Parse_ShouldReturnZero_ForNullOrWhitespace(string? input)
    {
        Assert.Equal(0m, CostParser.Parse(input));
    }

    [Fact]
    public void Parse_ShouldReturnZero_ForInvalidInput()
    {
        Assert.Equal(0m, CostParser.Parse("abc"));
    }

    [Fact]
    public void Parse_ShouldTrimWhitespace()
    {
        Assert.Equal(10.5m, CostParser.Parse("  10.5  "));
    }

    [Theory]
    [InlineData("10.555", 10.56)]
    [InlineData("10.554", 10.55)]
    [InlineData("10.1234", 10.12)]
    public void Parse_ShouldRoundToTwoDecimalPlaces(string input, decimal expected)
    {
        Assert.Equal(expected, CostParser.Parse(input));
    }
}
