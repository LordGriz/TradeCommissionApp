var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var apiService = builder.AddProject<Projects.TradeCommissionApp_ApiService>("apiservice");

var calculationService = builder.AddProject<Projects.TradeCommissionApp_CalculationService>("calculationservice")
    .WithReference(apiService);

builder.AddProject<Projects.TradeCommissionApp_Web>("webfrontend")
    .WithReference(cache)
    .WithReference(apiService)
    .WithReference(calculationService);


builder.Build().Run();
