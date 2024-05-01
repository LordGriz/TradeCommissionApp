using Domain.Contracts;
using TradeCommissionApiTypes;

namespace TradeCommissionApp.ApiService;

public static class TradesRouteConfiguration
{
    public static void AddTradesRoutes(this WebApplication app)
    {
        // Add a new fee 
        //
        app.MapPost("/trades", async (ITradeRepository repository, TradeRequest tradeRequest) =>
        {
            var trade = tradeRequest.ToTrade();
            trade = await repository.Add(trade);
            return Results.Accepted($"/fees/{trade.Id}", trade);

        }).WithTags("Trades");

        // Get all fees with matching security and transaction type when applicable
        //
        app.MapGet("/trades", async (ITradeRepository repository) =>
                await repository.Get().ToListAsync())
            .WithTags("Trades");

        // Delete a fee
        //
        app.MapDelete("/trades/{id:guid}", async (ITradeRepository repository, Guid id) =>
        {
            var trade = await repository.Get(id);
            if (trade is null)
            {
                return Results.NotFound(id);
            }

            await repository.Remove(trade);
            return Results.Ok(trade);

        }).WithTags("Trades");
    }
}