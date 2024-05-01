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
using TradeCommissionApp.ApiService;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddScoped<CommissionCalculationService>();
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

app.AddFeesRoutes();
app.AddTradesRoutes();

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

app.MapPost("/commision/calculate", async (CommissionCalculationService calculationService, CalculateCommissionRequest request) =>
{

    var result = await calculationService.Calculate(request.ToTradeList());

    return Results.Ok(result);
});


app.MapDefaultEndpoints();

app.Run();

