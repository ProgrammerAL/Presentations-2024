using OpenTelemetry.Trace;

using OTelDemo.InternalApiService;
using OTelDemo.InternalApiService.Controllers;
using OTelDemo.InternalApiService.DB;
using OTelDemo.InternalApiService.DB.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.ConfigureOpenTelemetry(ActivitySources.ActivitySourcesNameWildcard);

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(x => x.FullName?.Replace("+", ".").ToString() ?? Guid.NewGuid().ToString());
});

builder.AddSqlServerDbContext<ServiceDbContext>(connectionName: "sqldb",
    null,
    configureDbContextOptions: x =>
    { 
        x.AddInterceptors(new OTelSpanInterceptor());
    });

builder.Services.AddScoped<IProductsRepository, ProductsRepository>();

builder.Services.AddSingleton(x =>
{
    var tracerProvider = x.GetRequiredService<TracerProvider>();
    return tracerProvider.GetTracer(ActivitySources.AppActivitySource.Name);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

GetEnvironmentVariablesEndpoint.RegisterApiEndpoint(app);
CreateProductEndpoint.RegisterApiEndpoint(app);
GetAllProductsEndpoint.RegisterApiEndpoint(app);
GetEnabledProductsEndpoint.RegisterApiEndpoint(app);

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ServiceDbContext>();
        context.Database.EnsureCreated();
    }
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days.
    // You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.Run();
