using Domain.Contracts;
using Domain.Objects;
using Domain.Types;
using FluentAssertions;
using Moq;
using TradeCommissionApp.CalculationService;

namespace CalculationService.Tests;

public class CommissionCalculationServiceTests
{
    private readonly MockRepository _mockRepository;

    private readonly Mock<IFeeRepository> _fakeFeeRepository;

    public CommissionCalculationServiceTests()
    {
        _mockRepository = new MockRepository(MockBehavior.Default);

        _fakeFeeRepository = _mockRepository.Create<IFeeRepository>();
    }

    private void MockGetFees(IEnumerable<Fee> fees)
    {
        _fakeFeeRepository
            .Setup(r => r.Get(It.IsAny<string?>(), It.IsAny<TransactionType?>()))
            .Returns((string? securityType, TransactionType? transactionType) =>
                fees.Where(f => string.CompareOrdinal(f.SecurityType, securityType) == 0 && f.TransactionType == transactionType).ToAsyncEnumerable());
        //.Returns((string? _, TransactionType? _) =>
        //    fees.ToAsyncEnumerable());
    }

    public readonly Fee ComBuyCommissionFee = new("Standard COM Commission", "COM", TransactionType.Buy, 0.05);
    public readonly Fee ComSellCommissionFee = new("Standard COM Commission", "COM", TransactionType.Sell, 0.05);
    public readonly Fee ComSellAdvisoryFee = new("Advisory Fee", "COM", TransactionType.Sell, FlatFee: 500, MinThreshold: 100000);
    public readonly Fee FxBuyCommissionFee = new("Standard FX Commission", "FX", TransactionType.Buy, 0.01);
    public readonly Fee FxSellLowerCommissionFee = new("Standard FX Commission", "FX", TransactionType.Sell, FlatFee: 100, MinThreshold: 10000, MaxThreshold: 999999.99);
    public readonly Fee FxSellHigherCommissionFee = new("Standard Commission", "FX", TransactionType.Sell, FlatFee: 1000, MinThreshold: 1000000);


    /// <summary>
    /// This test verifies the example stated in the ProblemDescription.md
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CommissionCalculationService_WhenHasSingleFee_ReturnsCorrectCharge()
    {
        MockGetFees([
            ComBuyCommissionFee
        ]);

        Trade trade = new("COM", TransactionType.Buy, 1000, 12);
        List<Trade> trades = [trade];

        var expectedResult = trade.Quantity * trade.Price * ComBuyCommissionFee.PercentageOfTotal / 100;

        CommissionCalculationService sut = new(_fakeFeeRepository.Object);

        // Act
        var result = await sut.Calculate(trades);

        _mockRepository.VerifyAll();

        result.TradeCommissions.Should().NotBeEmpty();
        result.TradeCommissions.First().Commission.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(10000, 0)]
    [InlineData(100000, 500)]
    public async Task CommissionCalculationService_WhenHasMultipleFees_ReturnsCorrectCharge(double quantity, double expectedAdvisoryFee)
    {
        MockGetFees([
            ComSellCommissionFee,
            ComSellAdvisoryFee
        ]);

        Trade trade = new("COM", TransactionType.Sell, quantity, 2);
        List<Trade> trades = [trade];

        var expectedCommission = trade.Quantity * trade.Price * ComSellCommissionFee.PercentageOfTotal / 100;
        var expectedResult = expectedCommission + expectedAdvisoryFee;

        CommissionCalculationService sut = new(_fakeFeeRepository.Object);

        // Act
        var result = await sut.Calculate(trades);

        _mockRepository.VerifyAll();

        result.TradeCommissions.Should().NotBeEmpty();
        result.TradeCommissions.First().Commission.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(10000, 100)]
    [InlineData(999999, 100)]
    [InlineData(1000000, 1000)]
    public async Task CommissionCalculationService_WhenHasDifferentFeeThresholds_ReturnsCorrectCharge(double quantity, double expectedCommission)
    {
        MockGetFees([
            FxSellLowerCommissionFee,
            FxSellHigherCommissionFee
        ]);

        Trade trade = new("FX", TransactionType.Sell, quantity, 1);
        List<Trade> trades = [trade];

        CommissionCalculationService sut = new(_fakeFeeRepository.Object);

        // Act
        var result = await sut.Calculate(trades);

        _mockRepository.VerifyAll();

        result.TradeCommissions.Should().NotBeEmpty();
        result.TradeCommissions.First().Commission.Should().Be(expectedCommission);
    }

    [Fact]
    public async Task CommissionCalculationService_WhenMultipleTrades_ReturnsCorrectTotal()
    {
        MockGetFees([
            ComBuyCommissionFee,
            ComSellCommissionFee,
            ComSellAdvisoryFee,
            FxBuyCommissionFee,
            FxSellLowerCommissionFee,
            FxSellHigherCommissionFee
        ]);

        List<Trade> trades = [
            new Trade("COM", TransactionType.Sell, 100000, 1),
            new Trade("COM", TransactionType.Sell, 2000, 1),
            new Trade("COM", TransactionType.Buy, 20000, 10),
            new Trade("FX", TransactionType.Buy, 90900, 10),
            new Trade("FX", TransactionType.Sell, 20000, 10)
        ];

        const double expectedResult = 841.9;

        CommissionCalculationService sut = new(_fakeFeeRepository.Object);

        // Act
        var result = await sut.Calculate(trades);

        _mockRepository.VerifyAll();

        result.Total.Should().Be(expectedResult);
    }
}