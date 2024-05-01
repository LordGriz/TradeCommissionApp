using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddDbContext<TradeCommissionDbContext>(options =>
            options.UseSqlite("Data Source=development.db"));

        return services;
    }
}