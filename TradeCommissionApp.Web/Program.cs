using TradeCommissionApp.Web;
using TradeCommissionApp.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.AddRedisOutputCache("cache");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient<ApiServiceClient>(client => client.BaseAddress = new("https://apiservice"))
    .AddStandardResilienceHandler(); ;

builder.Services.AddHttpClient<CalculationServiceClient>(client => client.BaseAddress = new("https://calculationservice"))
    .AddStandardResilienceHandler(); ;


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();

app.UseAntiforgery();

app.UseOutputCache();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
