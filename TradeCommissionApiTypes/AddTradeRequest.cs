using System.Text.Json.Serialization;
using Domain.Objects;
using Domain.Types;

namespace TradeCommissionApiTypes;

public sealed class TradeRequest
{
    public TradeRequest()
    {
    }

    public TradeRequest(string securityType, TransactionType transactionType, double quantity, double price)
    {
        SecurityType = securityType;
        TransactionType = transactionType;
        Quantity = quantity;
        Price = price;
    }

    public Trade ToTrade()
    {
        return new Trade(SecurityType, TransactionType, Quantity, Price);
    }

    public string SecurityType { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TransactionType TransactionType { get; set; } = TransactionType.Unknown;

    public double Quantity { get; set; }

    public double Price { get; set; }
}