using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;
using System.Linq;

namespace OTelDemo.InternalApiService.Controllers;

public class GetEnvironmentVariablesEndpoint
{
    public static RouteHandlerBuilder RegisterApiEndpoint(WebApplication app)
    {
        return app.MapGet("/otel-environment-variables",
        () =>
            {
                var allVariables = Environment.GetEnvironmentVariables();

                var keys = new List<string>();
                foreach (var key in allVariables.Keys)
                {
                    var stringKey = (string)key;
                    if (stringKey.StartsWith("OTEL", StringComparison.OrdinalIgnoreCase))
                    {
                        keys.Add(stringKey);
                    }
                }

                var orderedKeys = keys.OrderBy(x => x).ToImmutableArray();

                var builder = ImmutableDictionary.CreateBuilder<string, string>();
                foreach (var key in orderedKeys)
                {
                    builder.Add(key, allVariables[key]!.ToString()!);
                }

                var result = new GetEnvironmentVariablesEndpointResponse(builder.ToImmutableDictionary());
                return TypedResults.Ok(result);
            })
            .WithSummary($"GETs all Environment Variables that start with OTEL");
    }
}

public record GetEnvironmentVariablesEndpointResponse(ImmutableDictionary<string, string> Variables);
