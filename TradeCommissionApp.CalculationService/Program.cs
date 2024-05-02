using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using Domain.Contracts;
using TradeCommissionApiTypes;
using TradeCommissionApp.CalculationService;
using TradeCommissionApp.CalculationService.Repositories;

var builder = WebApplication.CreateBuilder(args);


// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers().AddJsonOptions(configure =>
{
    configure.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    configure.JsonSerializerOptions.AllowTrailingCommas = true;
    configure.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddHttpClient<IFeeRepository, FeeRepository>(client => client.BaseAddress = new("http://apiservice"));
//builder.Services.AddScoped<IFeeRepository, FeeRepository>();
builder.Services.AddScoped<CommissionCalculationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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

// Add the endpoint for calculating the commission
app.MapPost("/commission/calculate", async (CommissionCalculationService calculationService, CalculateCommissionRequest request) =>
{
    var result = await calculationService.Calculate(request.ToTradeList());
    return Results.Ok(result);
}).WithTags("Commission Service"); ;

app.MapDefaultEndpoints();


app.Run();

