using Domain.Contracts;
using Domain.Objects;
using Domain.Types;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class FeeRepository : IFeeRepository
{
    private readonly TradeCommissionDbContextFactory _dbContextFactory;

    public FeeRepository(TradeCommissionDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<Fee> Add(Fee fee)
    {
        var dbContext = _dbContextFactory.CreateTradeCommissionDbContext();
        var added = await dbContext.Fees.AddAsync(fee);
        await dbContext.SaveChangesAsync();

        return added.Entity;
    }

    public async Task<bool> Remove(Fee fee)
    {
        var dbContext = _dbContextFactory.CreateTradeCommissionDbContext();
        dbContext.Fees.Remove(fee);
        var changes = await dbContext.SaveChangesAsync();

        return changes > 0;
    }

    public async Task<bool> Update(Fee fee)
    {
        var dbContext = _dbContextFactory.CreateTradeCommissionDbContext();
        dbContext.Fees.Update(fee);
        var changes = await dbContext.SaveChangesAsync();

        return changes > 0;
    }

    public IAsyncEnumerable<Fee> Get(string? securityType = default, TransactionType? transactionType = default)
    {
        var dbContext = _dbContextFactory.CreateTradeCommissionDbContext();
        var fees = dbContext.Fees.AsQueryable();

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
        var dbContext = _dbContextFactory.CreateTradeCommissionDbContext();
        return await dbContext.Fees.FirstOrDefaultAsync(f => f.Id == id);
    }
}