using System.Text.Json.Serialization;
using Domain.Objects;
using Domain.Types;

namespace TradeCommissionApp.ApiService;

internal sealed record AddFeeRequest(
    string Description,
    string SecurityType,
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    TransactionType TransactionType,
    double PercentageOfTotal = 0,
    double FlatFee = 0,
    double? MinThreshold = default,
    double? MaxThreshold = default)
{
    public Fee ToFee()
    {
        return new Fee(Description, SecurityType, TransactionType, PercentageOfTotal, FlatFee, MinThreshold, MaxThreshold);
    }
}