using Domain.Objects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class TradeCommissionDbContext(DbContextOptions<TradeCommissionDbContext> options) : DbContext(options)
{
    public DbSet<Fee> Fees { get; set; } = null!;
    public DbSet<Trade> Trades { get; set; } = null!;
}