using CalculationService;
using Domain.Contracts;
using Domain.Objects;
using Domain.Types;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddScoped<CommissionCalculationService>();
builder.Services.AddScoped<IFeeRepository, FeeRepository>();
builder.Services.AddPersistence();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers().AddJsonOptions(configure =>
{
    configure.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    configure.JsonSerializerOptions.AllowTrailingCommas = true;
    configure.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
app.UseExceptionHandler(configure =>
{
    configure.Run(async context =>
    {
        var ex = context.Features.Get<IExceptionHandlerFeature>()!.Error;

        var details = new ProblemDetails()
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error has occurred on the server",
            Detail = ex.Message
        };

        switch (ex)
        {
            case BadHttpRequestException:
                details.Title = $"{ex.GetType()}";
                details.Status = StatusCodes.Status400BadRequest;
                details.Detail = ex.InnerException?.Message ?? details.Detail;
                break;
        }

        await Results.Problem(details).ExecuteAsync(context);
    });
});

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

//app.MapGet("/weatherforecast", () =>
//{
//    var forecast = Enumerable.Range(1, 5).Select(index =>
//        new WeatherForecast
//        (
//            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//            Random.Shared.Next(-20, 55),
//            summaries[Random.Shared.Next(summaries.Length)]
//        ))
//        .ToArray();
//    return forecast;
//});

app.MapPost("/calculatecommision", async (CommissionCalculationService calculationService, List<Trade> trades) =>
{
    var result = await calculationService.Calculate(trades);

    //var forecast = Enumerable.Range(1, 5).Select(index =>
    //        new WeatherForecast
    //        (
    //            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
    //            Random.Shared.Next(-20, 55),
    //            summaries[Random.Shared.Next(summaries.Length)]
    //        ))
    //    .ToArray();
    //return forecast;
});

// Add a new fee 
//
app.MapPost("/fees", async (IFeeRepository repository, AddFeeRequest feeRequest) =>
{
    var fee = feeRequest.ToFee();
    fee = await repository.Add(fee);
    return Results.Accepted($"/fees/{fee.Id}", fee);

}).WithTags("Fees");

// Get all fees with matching security and transaction type when applicable
//
app.MapGet("/fees", async (IFeeRepository repository, string? securityType = default, TransactionType? transactionType = default) =>
{
    return await repository.Get(securityType, transactionType).ToListAsync();
}).WithTags("Fees");

// Delete a fee
//
app.MapDelete("/fees/{feeId:guid}", async (IFeeRepository repository, Guid feeId) =>
{
 //   var fee = feeRequest.ToFee();
    var fee = await repository.Get(feeId);
    if (fee is null)
    {
        return Results.NotFound(feeId);
    }

    await repository.Remove(fee);
    return Results.Ok(fee);

}).WithTags("Fees");



app.MapPost("/fees/resetdefault", async (IFeeRepository repository) =>
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



app.MapDefaultEndpoints();

app.Run();


record AddFeeRequest(
    string Description,
    string SecurityType,
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    TransactionType TransactionType,
    double PercentageOfTotal = 0,
    double FlatFee = 0,
    double? MinThreshold = default,
    double? MaxThreshold = default)
{
    public Fee ToFee()
    {
        return new Fee(Description, SecurityType, TransactionType, PercentageOfTotal, FlatFee, MinThreshold, MaxThreshold);
    }

    //public void Deconstruct(out string Description, out string SecurityType, out TransactionType TransactionType, out double PercentageOfTotal, out double FlatFee, out double? MinThreshold, out double? MaxThreshold)
    //{
    //    Description = this.Description;
    //    SecurityType = this.SecurityType;
    //    TransactionType = this.TransactionType;
    //    PercentageOfTotal = this.PercentageOfTotal;
    //    FlatFee = this.FlatFee;
    //    MinThreshold = this.MinThreshold;
    //    MaxThreshold = this.MaxThreshold;
    //}
}

//public class ExceptionFilter : IEndpointFilter
//{
//    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
//    {
//        try
//        {
//            return await next(context);
//        }
//        catch (Exception exception)
//        {
//            //Catch all exceptions and respond with the error message
//            return Results.Json(exception.Message, statusCode: 506);
//        }
//    }
//}
