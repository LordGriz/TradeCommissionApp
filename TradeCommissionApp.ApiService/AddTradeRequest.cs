using System.Text.Json.Serialization;
using Domain.Objects;
using Domain.Types;

namespace TradeCommissionApp.ApiService;

internal sealed record TradeRequest(
    string SecurityType,
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    TransactionType TransactionType,
    double Quantity = 0,
    double Price = 0)
{
    public Trade ToTrade()
    {
        return new Trade(SecurityType, TransactionType, Quantity, Price);
    }
}