using Domain.Objects;

namespace TradeCommissionApiTypes;

public sealed record CalculateCommissionRequest
{
    public CalculateCommissionRequest(TradeRequest[] Trades)
    {
        this.Trades = Trades;
    }

    public List<Trade> ToTradeList()
    {
        return Trades.Select(t => t.ToTrade()).ToList();
    }

    public TradeRequest[] Trades { get; init; }
}