using CalculationService;
using Infrastructure;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TradeCommissionApp.ApiService;
using TradeCommissionApiTypes;

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

// Add the endpoint for calculating the commission
app.MapPost("/commission/calculate", async (CommissionCalculationService calculationService, CalculateCommissionRequest request) =>
{
    var result = await calculationService.Calculate(request.ToTradeList());
    return Results.Ok(result);
}).WithTags("Commission Service"); ;


app.MapDefaultEndpoints();

app.Run();

