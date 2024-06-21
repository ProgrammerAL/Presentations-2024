using System.Diagnostics;

using OTelDemo.PublicApiService.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.ConfigureOpenTelemetry();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(x => x.FullName?.Replace("+", ".").ToString() ?? Guid.NewGuid().ToString());
});


builder.Services.AddHttpClient("internal-api-client",
    static client => client.BaseAddress = new("https+http://internal-api"));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

GetProductsEndpoint.RegisterApiEndpoint(app);
app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
