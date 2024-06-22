using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;

namespace OTelDemo.InternalApiService.Controllers;

public class GetEnvironmentVariablesEndpoint
{
    public static RouteHandlerBuilder RegisterApiEndpoint(WebApplication app)
    {
        return app.MapGet("/environment-variables",
        () =>
            {
                var allVariables = Environment.GetEnvironmentVariables();

                var builder = ImmutableDictionary.CreateBuilder<string, string>();
                foreach (var key in allVariables.Keys)
                {
                    var keyString = (string)key;
                    builder.Add(keyString, allVariables[keyString]!.ToString()!);
                }

                var result = new GetEnvironmentVariablesEndpointResponse(builder.ToImmutableDictionary());
                return TypedResults.Ok(result);
            })
            .WithSummary($"Loads all Environment Variables");
    }
}

public record GetEnvironmentVariablesEndpointResponse(ImmutableDictionary<string, string> Variables);
