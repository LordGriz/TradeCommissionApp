
using Domain.Objects;
using Domain.Types;
using FluentAssertions;

namespace Domain.Tests;

public class FeeUnitTests
{
    [Fact]
    public void Fee_Calculate_WhenMinThresholdIsNotMet_ReturnsZero()
    {
        const int totalTrade = 10;
        const int minThreshold = 20;

        Fee sut = new("description", "security", TransactionType.Buy, 50, 100, minThreshold);

        var result = sut.Calculate(totalTrade);

        result.Should().Be(0, $"the {nameof(Fee.MinThreshold)} of {minThreshold} was not met");
    }

    [Theory]
    [InlineData(80000, 0.05, 0, 40)]
    [InlineData(52, 0, 500, 500)]
    [InlineData(10000, 0.05, 99, 104)]
    [InlineData(80000, 0.05, 500, 540)]
    public void Fee_Calculate_WhenThresholdsAreNull_ReturnsCorrectResult(double totalTrade, double percentageOfTotal, double flatFee, double expectedResult)
    {
        Fee sut = new("description", "security", TransactionType.Buy, percentageOfTotal, flatFee);

        var result = sut.Calculate(totalTrade);

        result.Should().Be(expectedResult, $"the {nameof(Fee.PercentageOfTotal)} = {percentageOfTotal}, {nameof(Fee.FlatFee)} = {flatFee}, and thresholds are null");
    }

    [Theory]
    [InlineData(3100, 2, 0, 4000, 0)]
    [InlineData(3100, 2, 0, 2000, 62)]
    [InlineData(3100, 0, 100, 4000, 0)]
    [InlineData(3100, 0, 100, 2000, 100)]
    public void Fee_Calculate_WhenMinThresholdIsSet_ReturnsCorrectResult(double totalTrade, double percentageOfTotal, double flatFee, double threshold, double expectedResult)
    {
        Fee sut = new("description", "security", TransactionType.Buy, percentageOfTotal, flatFee, MinThreshold: threshold);

        var result = sut.Calculate(totalTrade);

        result.Should().Be(expectedResult, $"the {nameof(Fee.PercentageOfTotal)} = {percentageOfTotal}, {nameof(Fee.FlatFee)} = {flatFee}, and {nameof(Fee.MinThreshold)} was {threshold}");
    }

    [Fact]
    public void Fee_Calculate_WhenMaxThresholdIsExceeded_ReturnsZero()
    {
        const int totalTrade = 200;
        const int maxThreshold = 10;

        Fee sut = new("description", "security", TransactionType.Buy, 50, 100, MaxThreshold: maxThreshold);

        var result = sut.Calculate(totalTrade);

        result.Should().Be(0, $"the {nameof(Fee.MaxThreshold)} of {maxThreshold} was not met");
    }

    [Theory]
    [InlineData(3100, 2, 4000, 5000, 0)]
    [InlineData(3100, 2, 2000, 5000, 62)]
    [InlineData(3100, 2, 2000, 3000, 0)]
    public void Fee_Calculate_WhenBetweenThresholdIsSet_ReturnsCorrectResult(double totalTrade, double percentageOfTotal, double minThreshold, double maxThreshold, double expectedResult)
    {
        Fee sut = new("description", "security", TransactionType.Buy, percentageOfTotal, 0, minThreshold, maxThreshold);

        var result = sut.Calculate(totalTrade);

        result.Should().Be(expectedResult, $"the {nameof(Fee.PercentageOfTotal)} = {percentageOfTotal} and {nameof(Fee.MinThreshold)} was {minThreshold}");
    }

}