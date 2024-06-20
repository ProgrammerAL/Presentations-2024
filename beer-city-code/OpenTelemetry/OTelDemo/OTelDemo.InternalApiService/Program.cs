using Microsoft.EntityFrameworkCore;

using OTelDemo.InternalApiService.Controllers;
using OTelDemo.InternalApiService.DB;
using OTelDemo.InternalApiService.DB.Repositories;

using ProgrammerAl.Presentations.OTel.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(x => x.FullName?.ToString() ?? Guid.NewGuid().ToString());
});

builder.AddSqlServerSqlClientConfig(
    static settings => settings.DisableMetrics = true);

builder.Services.AddPooledDbContextFactory<ServiceDbContext>((serviceProvider, optionsBuilder) =>
{
    var sqlDbConnectionString = "TODO: Set This";
    optionsBuilder
    .UseSqlServer(sqlDbConnectionString)
    .EnableServiceProviderCaching(cacheServiceProvider: true)
        .LogTo(DatabaseOpenTelemetryHelpers.TraceSqlServerExecutedQueryInfo,
            events: ServiceDbContext.LoggingEventIds,
            minimumLevel: Microsoft.Extensions.Logging.LogLevel.Information)
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

builder.Services.AddSingleton<IProductsRepository, ProductsRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

CreateProductEndpoint.RegisterApiEndpoint(app);
GetAllProductsEndpoint.RegisterApiEndpoint(app);
GetEnabledProductsEndpoint.RegisterApiEndpoint(app);

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
