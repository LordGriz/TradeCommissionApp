using Domain.Objects;

namespace TradeCommissionApiTypes;

public sealed class CalculationResultResponse(List<Charge> tradeCommissions, double total)
{
    public List<Charge> TradeCommissions { get; } = tradeCommissions;

    public double Total { get; } = total;
}
