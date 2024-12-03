using Domain.Contracts;
using TradeCommissionApiTypes;

namespace TradeCommissionApp.ApiService;

public static class TradesRouteConfiguration
{
    public static void AddTradesRoutes(this WebApplication app)
    {
        var tradesGroup = app.MapGroup("/trades").WithTags("Trades");

        // Add a new trade 
        //
        tradesGroup.MapPost("/", async (ITradeRepository repository, TradeRequest tradeRequest) =>
        {
            var trade = tradeRequest.ToTrade();
            trade = await repository.Add(trade);
            return Results.Accepted($"/trades/{trade.Id}", trade);

        });

        // Get all trades
        //
        tradesGroup.MapGet("/", async (ITradeRepository repository) =>
                await repository.Get().ToListAsync());

        // Get a specific trade
        //
        tradesGroup.MapGet("/{id:guid}", async (ITradeRepository repository, Guid id) =>
        {
            var trade = await repository.Get(id);

            return trade is null
                ? Results.NotFound(id)
                : Results.Ok(trade);

        });

        // Delete a fee
        //
        tradesGroup.MapDelete("/{id:guid}", async (ITradeRepository repository, Guid id) =>
        {
            var trade = await repository.Get(id);
            if (trade is null)
            {
                return Results.NotFound(id);
            }

            await repository.Remove(trade);
            return Results.Ok(trade);

        });
    }
}