using Domain.Contracts;
using Domain.Objects;
using Domain.Types;
using TradeCommissionApiTypes;

namespace TradeCommissionApp.ApiService;

public static class FeesRouteConfiguration
{
    public static void AddFeesRoutes(this WebApplication app)
    {
        var feesGroup = app.MapGroup("/fees").WithTags("Fees");

        // Add a new fee 
        //
        feesGroup.MapPost("/", async (IFeeRepository repository, FeeRequest feeRequest) =>
        {
            var fee = feeRequest.ToFee();
            fee = await repository.Add(fee);
            return Results.Accepted($"/fees/{fee.Id}", fee);

        });

        // Get all fees with matching security and transaction type when applicable
        //
        feesGroup.MapGet("/", async (IFeeRepository repository, string? securityType = default, TransactionType? transactionType = default) =>
                await repository.Get(securityType, transactionType).ToListAsync());

        // Get a specific fee
        //
        feesGroup.MapGet("/{id:guid}", async (IFeeRepository repository, Guid id) =>
        {
            var fee = await repository.Get(id);

            return fee is null 
                ? Results.NotFound(id)
                : Results.Ok(fee);

        });

        // Delete a fee
        //
        feesGroup.MapDelete("/{id:guid}", async (IFeeRepository repository, Guid id) =>
        {
            var fee = await repository.Get(id);
            if (fee is null)
            {
                return Results.NotFound(id);
            }

            await repository.Remove(fee);
            return Results.Ok(fee);

        });

        // Delete all the fees and re-add them. This is meant for development purposes.
        //
        feesGroup.MapPost("/reset", async (IFeeRepository repository) =>
        {
            // Delete everything
            await foreach (var fee in repository.Get())
            {
                await repository.Remove(fee);
            }

            // ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
            List<Fee> fees = [
                new("Standard COM Commission", "COM", TransactionType.Buy, 0.05),
                new("Standard COM Commission", "COM", TransactionType.Sell, 0.05),
                new("Advisory Fee", "COM", TransactionType.Sell, FlatFee: 500, MinThreshold: 100000),
                new("Standard CB Commission", "CB", TransactionType.Buy, 0.02),
                new("Standard CB Commission", "CB", TransactionType.Sell, 0.01),
                new("Standard FX Commission", "FX", TransactionType.Buy, 0.01),
                new("Standard FX Commission", "FX", TransactionType.Sell, FlatFee: 100, MinThreshold: 10000, MaxThreshold: 999999.99),
                new("Standard Commission", "FX", TransactionType.Sell, FlatFee: 1000, MinThreshold: 1000000)
            ];
            // ReSharper restore ArrangeObjectCreationWhenTypeNotEvident

            // Add everything
            foreach (var fee in fees)
            {
                await repository.Add(fee);
            }

        });
    }
}