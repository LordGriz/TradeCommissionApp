using Domain.Objects;

namespace Domain.Contracts;

public interface ITradeRepository
{
    Task<Trade> Add(Trade trade);
    Task<bool> Remove(Trade trade);
    Task<bool> Update(Trade trade);
    IAsyncEnumerable<Trade> Get();
    Task<Trade?> Get(Guid id);
}