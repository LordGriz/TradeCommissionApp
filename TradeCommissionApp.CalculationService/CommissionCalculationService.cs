using Domain.Contracts;
using Domain.Objects;
using TradeCommissionApiTypes;

namespace TradeCommissionApp.CalculationService;

public sealed class CommissionCalculationService
{
    // ToDo: This should be set via the application configuration
    public const int MaxDegreeOfParallelism = 10;

    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly ParallelOptions _options;

    private readonly IFeeRepository _feeRepository;

    public CommissionCalculationService(IFeeRepository feeRepository)
    {
        _feeRepository = feeRepository;

        _options = new ParallelOptions
        { 
            MaxDegreeOfParallelism = MaxDegreeOfParallelism,
            CancellationToken = _cancellationTokenSource.Token
        };
    }

    public async Task<CalculationResultResponse> Calculate(IEnumerable<Trade> trades)
    {
        List<Charge> tradeCommissions = [];
        double totalCommission = 0;

        // Operate on the IEnumerable using multiple threads
        await Parallel.ForEachAsync(trades, _options, async (trade, cancellationToken) =>
        {
            // Do not perform any calculations if the execution was cancelled
            if (cancellationToken.IsCancellationRequested) return;

            var totalTrade = trade.Quantity * trade.Price;
            double commission = 0;

            await foreach (var fee in _feeRepository.Get(trade.SecurityType, trade.TransactionType).WithCancellation(cancellationToken))
            {
                // Calculate the commission based on the given fee
                commission += fee.Calculate(totalTrade);
            }

            // Add the calculation to the results
            tradeCommissions.Add(
                new Charge(trade.Id, commission));

            // Keep a running total using thread-safe double addition
            double initialValue, computedValue;
            do
            {
                // Save the current running total in a local variable.
                initialValue = totalCommission;

                // Add the new value to the running total.
                computedValue = initialValue + commission;
            }
            while (Math.Abs(initialValue - Interlocked.CompareExchange(ref totalCommission, computedValue, initialValue)) > 0.001);
        });
        
        return new CalculationResultResponse(tradeCommissions, totalCommission);
    }
}
