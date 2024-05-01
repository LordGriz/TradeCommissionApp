using Domain.Contracts;
using Domain.Objects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class TradeRepository : ITradeRepository
{
    private readonly TradeCommissionDbContext _dbContext;

    public TradeRepository(TradeCommissionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Trade> Add(Trade trade)
    {
        var added = await _dbContext.Trades.AddAsync(trade);
        await _dbContext.SaveChangesAsync();

        return added.Entity;
    }

    public async Task<bool> Remove(Trade trade)
    {
        _dbContext.Trades.Remove(trade);
        var changes = await _dbContext.SaveChangesAsync();

        return changes > 0;
    }

    public async Task<bool> Update(Trade trade)
    {
        _dbContext.Trades.Update(trade);
        var changes = await _dbContext.SaveChangesAsync();

        return changes > 0;
    }

    public IAsyncEnumerable<Trade> Get()
    {
        var trades = _dbContext.Trades.AsQueryable();
        return trades.AsAsyncEnumerable();
    }

    public async Task<Trade?> Get(Guid id)
    {
        return await _dbContext.Trades.FirstOrDefaultAsync(f => f.Id == id);
    }
}