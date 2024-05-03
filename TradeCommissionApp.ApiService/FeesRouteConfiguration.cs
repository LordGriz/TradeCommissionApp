using Domain.Contracts;
using Domain.Objects;
using Domain.Types;
using TradeCommissionApiTypes;

namespace TradeCommissionApp.ApiService;

public static class FeesRouteConfiguration
{
    public static void AddFeesRoutes(this WebApplication app)
    {
        // Add a new fee 
        //
        app.MapPost("/fees", async (IFeeRepository repository, FeeRequest feeRequest) =>
        {
            var fee = feeRequest.ToFee();
            fee = await repository.Add(fee);
            return Results.Accepted($"/fees/{fee.Id}", fee);

        }).WithTags("Fees");

        // Get all fees with matching security and transaction type when applicable
        //
        app.MapGet("/fees", async (IFeeRepository repository, string? securityType = default, TransactionType? transactionType = default) =>
                await repository.Get(securityType, transactionType).ToListAsync())
            .WithTags("Fees");

        // Delete a fee
        //
        app.MapDelete("/fees/{id:guid}", async (IFeeRepository repository, Guid id) =>
        {
            var fee = await repository.Get(id);
            if (fee is null)
            {
                return Results.NotFound(id);
            }

            await repository.Remove(fee);
            return Results.Ok(fee);

        }).WithTags("Fees");

        // Delete all the fees and re-add them. This is meant for development purposes.
        //
        app.MapPost("/fees/reset", async (IFeeRepository repository) =>
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

        }).WithTags("Fees");
    }
}