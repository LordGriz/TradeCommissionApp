using Domain.Contracts;
using Domain.Objects;
using Domain.Types;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class FeeRepository : IFeeRepository
{
    private readonly TradeCommissionDbContext _dbContext;

    public FeeRepository(TradeCommissionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fee> Add(Fee fee)
    {
        var added = await _dbContext.Fees.AddAsync(fee);
        await _dbContext.SaveChangesAsync();

        return added.Entity;
    }

    public async Task<bool> Remove(Fee fee)
    {
        _dbContext.Fees.Remove(fee);
        var changes = await _dbContext.SaveChangesAsync();

        return changes > 0;
    }

    public async Task<bool> Update(Fee fee)
    {
        _dbContext.Fees.Update(fee);
        var changes = await _dbContext.SaveChangesAsync();

        return changes > 0;
    }

    public IAsyncEnumerable<Fee> Get(string? securityType = default, TransactionType? transactionType = default)
    {
        var fees = _dbContext.Fees.AsQueryable();

        if (!string.IsNullOrWhiteSpace(securityType))
        {
            fees = fees.Where(f => f.SecurityType == securityType);
        }

        if (transactionType != null)
        {
            fees = fees.Where(f => f.TransactionType == transactionType);
        }

        return fees.AsAsyncEnumerable();
    }

    public async Task<Fee?> Get(Guid id)
    {
        return await _dbContext.Fees.FirstOrDefaultAsync(f => f.Id == id);
    }
}