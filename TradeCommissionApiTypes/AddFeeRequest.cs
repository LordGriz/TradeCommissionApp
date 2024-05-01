using System.Text.Json.Serialization;
using Domain.Objects;
using Domain.Types;

namespace TradeCommissionApiTypes;

public sealed class AddFeeRequest
{
    public AddFeeRequest()
    {
    }

    public AddFeeRequest(string description, string securityType, TransactionType transactionType, double percentageOfTotal = 0, double flatFee = 0,
        double? minThreshold = default, double? maxThreshold = default)
    {
        Description = description;
        SecurityType = securityType;
        TransactionType = transactionType;
        PercentageOfTotal = percentageOfTotal;
        FlatFee = flatFee;
        MinThreshold = minThreshold;
        MaxThreshold = maxThreshold;
    }

    public Fee ToFee()
    {
        return new Fee(Description, SecurityType, TransactionType, PercentageOfTotal, FlatFee, MinThreshold, MaxThreshold);
    }


    public string Description { get; set; } = string.Empty;
    public string SecurityType { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TransactionType TransactionType { get; set; } = TransactionType.Unknown;

    public double PercentageOfTotal { get; set; }
    public double FlatFee { get; set; }
    public double? MinThreshold { get; set; }
    public double? MaxThreshold { get; set; }
}