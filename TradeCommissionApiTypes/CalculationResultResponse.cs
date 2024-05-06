using Domain.Objects;

namespace TradeCommissionApiTypes;

public sealed class CalculationResultResponse(Charge[] tradeCommissions, double total)
{
    public Charge[] TradeCommissions { get; } = tradeCommissions;

    public double Total { get; } = total;
}
