using Domain.Types;

namespace Domain.Objects;

public sealed record Fee(string Description, string SecurityType, TransactionType TransactionType, double PercentageOfTotal = 0, double FlatFee = 0, double? MinThreshold = default, double? MaxThreshold = default)
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public double Calculate(double tradeTotal)
    {
        double result = 0;

        if (tradeTotal < MinThreshold || tradeTotal > MaxThreshold)
        {
            // The min threshold was not met or max threshold was exceeded. The fee does not apply.
            return 0;
        }

        if (PercentageOfTotal != 0)
        {
            result = tradeTotal * PercentageOfTotal / 100;
        }

        return result + FlatFee;
    }

    
}
