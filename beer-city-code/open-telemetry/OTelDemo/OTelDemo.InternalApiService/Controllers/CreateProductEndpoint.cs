using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using OTelDemo.InternalApiService.DB.Repositories;

namespace OTelDemo.InternalApiService.Controllers;

public class CreateProductEndpoint
{
    public static RouteHandlerBuilder RegisterApiEndpoint(WebApplication app)
    {
        return app.MapPost("/create-product",
        async ([FromBody, Required] CreateProductEndpointRequestBody request,
            IProductsRepository productsRepository) =>
            {
                var newId = Guid.NewGuid().ToString();

                await productsRepository.CreateProductAsync(newId, request.Name, request.Cost.Value, request.CurrencyCountry);

                var result = new CreateProductEndpointResponse(newId);
                return TypedResults.Ok(result);
            })
            .WithSummary($"Creates a new product");
    }
}

public class CreateProductEndpointRequestBody
{
    [FromBody, Required, NotNull]
    public string? Name { get; set; }

    [FromBody, Required, NotNull]
    public int? Cost { get; set; }

    [FromBody, Required, NotNull]
    public string? CurrencyCountry { get; set; }
}

public record CreateProductEndpointResponse(string Id);
