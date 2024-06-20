using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using OTelDemo.InternalApiService.DB.Repositories;

namespace OTelDemo.InternalApiService.Controllers;

public class GetAllProductsEndpoint
{
    public static RouteHandlerBuilder RegisterApiEndpoint(WebApplication app)
    {
        return app.MapGet("/all-products",
        async (IProductsRepository productsRepository) =>
            {
                var products = await productsRepository.AllProductsAsync();
                var responseProducts = products
                    .Select(p => new AllProductsEndpointResponse.ProductResponse(p.Id, p.Enabled, p.Name, p.Cost, p.CurrencyCountry))
                    .ToImmutableArray();

                var result = new AllProductsEndpointResponse(responseProducts);
                return TypedResults.Ok(result);
            })
            .WithSummary($"Gets all products");
    }
}

public record AllProductsEndpointResponse(ImmutableArray<AllProductsEndpointResponse.ProductResponse> Products)
{
    public record ProductResponse(string Id, bool Enabled, string Name, int Cost, string CurrencyCountry);
}
