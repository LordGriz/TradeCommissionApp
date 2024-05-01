using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Objects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class TradeCommissionDbContext(DbContextOptions<TradeCommissionDbContext> options) : DbContext(options)
    {
        public DbSet<Fee> Fees { get; set; } = null!;

    }
}
