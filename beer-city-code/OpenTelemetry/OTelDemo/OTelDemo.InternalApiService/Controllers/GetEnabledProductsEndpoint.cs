using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using OTelDemo.InternalApiService.DB.Repositories;

namespace OTelDemo.InternalApiService.Controllers;

public class GetEnabledProductsEndpoint
{
    public static RouteHandlerBuilder RegisterApiEndpoint(WebApplication app)
    {
        return app.MapGet("/enabled-products",
        async (IProductsRepository productsRepository) =>
            {
                var products = await productsRepository.AllProductsAsync();
                var responseProducts = products
                    .Select(p => new EnabledProductsEndpointResponse.ProductResponse(p.Id, p.Name, p.Cost, p.CurrencyCountry))
                    .ToImmutableArray();

                var result = new EnabledProductsEndpointResponse(responseProducts);
                return TypedResults.Ok(result);
            })
            .WithSummary($"Gets all enabled products");
    }
}

public record EnabledProductsEndpointResponse(ImmutableArray<EnabledProductsEndpointResponse.ProductResponse> Products)
{
    public record ProductResponse(string Id, string Name, int Cost, string CurrencyCountry);
}
