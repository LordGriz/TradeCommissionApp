using Domain.Objects;

namespace CalculationService;

public sealed class CalculationResult(List<Charge> tradeCommissions, double total)
{
    public List<Charge> TradeCommissions { get; } = tradeCommissions;

    public double Total { get; } = total;
}
