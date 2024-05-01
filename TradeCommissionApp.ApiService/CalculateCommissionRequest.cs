using Domain.Objects;

namespace TradeCommissionApp.ApiService;

internal sealed record CalculateCommissionRequest(TradeRequest[] Trades)
{
    public List<Trade> ToTradeList()
    {
        return Trades.Select(t => t.ToTrade()).ToList();
    }
}