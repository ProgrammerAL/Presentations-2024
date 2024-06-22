using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace OTelDemo.PublicApiService.Controllers;

public class GetProductsEndpoint
{
    public static RouteHandlerBuilder RegisterApiEndpoint(WebApplication app)
    {
        return app.MapGet("/products",
        async (IHttpClientFactory httpClientFactory) =>
            {
                var client = httpClientFactory.CreateClient("internal-api-client");
                var enabledProductsResult = await client.GetAsync("/enabled-products");

                enabledProductsResult.EnsureSuccessStatusCode();

                var enabledProductsResponse = await enabledProductsResult.Content.ReadFromJsonAsync<EnabledProductsEndpointResponse>();

                var responseProducts = enabledProductsResponse?.Products
                            .Select(p => new EnabledProductsEndpointResponse.ProductResponse(p.Id, p.Name, p.Cost, p.CurrencyCountry))
                            .ToImmutableArray()
                            ?? [];

                var result = new EnabledProductsEndpointResponse(responseProducts);
                return TypedResults.Ok(result);
            })
            .WithSummary($"Gets all products");
    }

    public record EnabledProductsEndpointResponse(ImmutableArray<EnabledProductsEndpointResponse.ProductResponse> Products)
    {
        public record ProductResponse(string Id, string Name, int Cost, string CurrencyCountry);
    }
}

public record GetProductsEndpointResponse(ImmutableArray<GetProductsEndpointResponse.ProductResponse> Products)
{
    public record ProductResponse(string Id, string Name, int Cost, string CurrencyCountry);
}
