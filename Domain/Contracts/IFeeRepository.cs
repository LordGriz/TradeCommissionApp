using Domain.Types;
using Domain.Objects;

namespace Domain.Contracts;

public interface IFeeRepository
{
    Task<Fee> Add(Fee fee);
    Task<bool> Delete(Fee fee);
    Task<bool> Update(Fee fee);

    IAsyncEnumerable<Fee> Get(string? securityType = default, TransactionType? transactionType = default);
}