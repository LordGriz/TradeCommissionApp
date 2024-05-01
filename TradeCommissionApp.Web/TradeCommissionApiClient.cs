using Domain.Objects;
using TradeCommissionApiTypes;

namespace TradeCommissionApp.Web;

public class TradeCommissionApiClient(HttpClient httpClient)
{
    public async Task<Fee[]> GetFeesAsync()
    {
        return await httpClient.GetFromJsonAsync<Fee[]>("/fees") ?? [];
    }

    public async Task DeleteFeeAsync(Guid id)
    {
         await httpClient.DeleteAsync($"/fees/{id}");
    }

    public async Task ResetToDefaultAsync()
    {
        await httpClient.PostAsync("/fees/reset", null);
    }

    public async Task<Trade[]> GetTradesAsync()
    {
        return await httpClient.GetFromJsonAsync<Trade[]>("/trades") ?? [];
    }

    public async Task DeleteTradeAsync(Guid id)
    {
        await httpClient.DeleteAsync($"/trades/{id}");
    }

    public async Task AddTradeAsync(TradeRequest trade)
    {
        await httpClient.PostAsJsonAsync("/trades", trade);
    }

    public async Task<CalculationResultResponse?> CalculateCommissionAsync(Trade[] trades)
    {
        var request = new CalculateCommissionRequest(
            trades.Select(t => new TradeRequest(t.SecurityType, t.TransactionType, t.Quantity, t.Price)).ToArray());

        var response = await httpClient.PostAsJsonAsync("commission/calculate", request);
        return await response.Content.ReadFromJsonAsync<CalculationResultResponse>();
    }   
}
