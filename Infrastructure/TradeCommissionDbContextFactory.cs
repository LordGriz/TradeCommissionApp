using Microsoft.EntityFrameworkCore;


namespace Infrastructure
{
    public sealed class TradeCommissionDbContextFactory(DbContextOptions<TradeCommissionDbContext> options)
    {
        public TradeCommissionDbContext CreateTradeCommissionDbContext()
        {
            return new TradeCommissionDbContext(options);
        }
    }
}
