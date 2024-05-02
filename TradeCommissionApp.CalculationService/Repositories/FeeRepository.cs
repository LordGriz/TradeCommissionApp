using System.Net.Http.Json;
using System.Text;
using Domain.Contracts;
using Domain.Objects;
using Domain.Types;
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace TradeCommissionApp.CalculationService.Repositories;

public class FeeRepository(HttpClient httpClient) : IFeeRepository
{
    public async Task<Fee> Add(Fee fee)
    {
        throw new NotImplementedException($"The {nameof(CalculationService)} is not allowed to add fees");
    }

    public async Task<bool> Remove(Fee fee)
    {
        throw new NotImplementedException($"The {nameof(CalculationService)} is not allowed to remove fees");
    }

    public async Task<bool> Update(Fee fee) 
    {
        throw new NotImplementedException($"The {nameof(CalculationService)} is not allowed to Update fees");
    }

    public async IAsyncEnumerable<Fee> Get(string? securityType = default, TransactionType? transactionType = default)
    {
        StringBuilder urlBuilder = new("/fees?");

        if (securityType != null)
        {
            urlBuilder.Append($"securityType={securityType}");
        }

        if (transactionType != null)
        {
            urlBuilder.Append($"&transactionType={transactionType}");
        }

        var fees = await httpClient.GetFromJsonAsync<Fee[]>(urlBuilder.ToString()) ?? [];
        foreach (var fee in fees)
        {
            yield return fee;
        }
    }

    public async Task<Fee?> Get(Guid id)
    {
        throw new NotImplementedException($"The {nameof(CalculationService)} has no reason to get fees by id");
    }
}