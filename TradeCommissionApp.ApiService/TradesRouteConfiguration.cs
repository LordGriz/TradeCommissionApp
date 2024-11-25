using Domain.Contracts;
using Domain.Objects;
using TradeCommissionApiTypes;

namespace TradeCommissionApp.ApiService;

public static class TradesRouteConfiguration
{
    public static void AddTradesRoutes(this WebApplication app)
    {
        // Add a new trade 
        //
        app.MapPost("/trades", async (ITradeRepository repository, TradeRequest tradeRequest) =>
        {
            var trade = tradeRequest.ToTrade();
            trade = await repository.Add(trade);
            return Results.Accepted($"/trades/{trade.Id}", trade);

        }).WithTags("Trades");

        // Get all trades
        //
        app.MapGet("/trades", async (ITradeRepository repository) =>
                await repository.Get().ToListAsync())
            .WithTags("Trades");

        // Get a specific trade
        //
        app.MapGet("/trades/{id:guid}", async (ITradeRepository repository, Guid id) =>
        {
            var trade = await repository.Get(id);

            return trade is null
                ? Results.NotFound(id)
                : Results.Ok(trade);

        }).WithTags("Trades");

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