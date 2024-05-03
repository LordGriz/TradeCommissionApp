using Domain.Objects;
using TradeCommissionApiTypes;

namespace TradeCommissionApp.Web;

public class CalculationServiceClient(HttpClient httpClient)
{
    public async Task<CalculationResultResponse?> CalculateCommissionAsync(Trade[] trades)
    {
        var request = new CalculateCommissionRequest(
            trades.Select(t => new TradeRequest(t.SecurityType, t.TransactionType, t.Quantity, t.Price)).ToArray());

        var response = await httpClient.PostAsJsonAsync("commission/calculate", request);
        return await response.Content.ReadFromJsonAsync<CalculationResultResponse>();
    }   
}
